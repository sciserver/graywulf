using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Jhu.Graywulf.Metadata
{
    public class Parser
    {
        private const string SingleLineComment = "--";
        private const string MultiLineCommentStart = "/*";
        private const string MultiLineCommentEnd = "*/";
        private const string MagicToken = "/";

        private const string Identifier = @"(?:[a-z_][a-z_0-9]*)";
        private const string QuotedIdentifier = @"(@*(?:\[[^]]+\])|@*" + Identifier + @")";
        private const string ObjectName = QuotedIdentifier + @"(?:\." + QuotedIdentifier + @")*";
        private static readonly Regex ObjectNameRegex = new Regex(ObjectName, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex CreateObjectRegex = new Regex(@"CREATE\s+([a-z]+)\s+(" + ObjectName + @")\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex ReturnsTableRegex = new Regex(@"\s*RETURNS\s+" + QuotedIdentifier + @"\s+TABLE\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string XmlTag = @"<\s*([a-z]+)(\s+[a-z]+\s*=\s*((""[^""]*"")|(\S+)))*>";
        private static readonly Regex XmlTagRegex = new Regex(XmlTag, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex ParamNameRegex = new Regex(@"\s*" + QuotedIdentifier, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Extracts xml comments from the source file.
        /// XML lines are marked with a single line comment (--)
        /// followed immediately by a magic token (/).
        /// Also adds special tags to the xml for tables, functions,
        /// columns, paramters, etc. based on the location of the
        /// xml comments
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Parse(string input)
        {
            StringWriter buffer = new StringWriter();
            StringWriter buffer2 = new StringWriter();
            StringWriter buffer3 = new StringWriter();

            RemoveMultilineComments(new StringReader(input), buffer);
            RemoveSinglelineComments(new StringReader(buffer.ToString()), buffer2);

            StringBuilder script = new StringBuilder(buffer2.ToString());
            InsertObjectTags(script);

            ExtractXmlComments(new StringReader(script.ToString()), buffer3);

            return buffer3.ToString();
        }

        /// <summary>
        /// Removes multi-line comments from the source
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void RemoveMultilineComments(TextReader input, TextWriter output)
        {
            bool inside = false;
            string line;
            int pos;

            while ((line = input.ReadLine()) != null)
            {
                pos = 0;

                while (pos < line.Length)
                {
                    if (inside)
                    {
                        int i = line.IndexOf(MultiLineCommentEnd, pos);
                        if (i >= 0)
                        {
                            // Skip comment part
                            pos = i + MultiLineCommentEnd.Length;
                            inside = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        int i = line.IndexOf(MultiLineCommentStart, pos);

                        // Make sure that no single line comment token preceding the multi line comment
                        int j = line.IndexOf(SingleLineComment);
                        if (j != -1 && j < i)
                            break;


                        if (i >= 0)
                        {
                            // Write out string from 'start' to 'i'
                            output.Write(line.Substring(pos, i - pos));
                            pos = i + MultiLineCommentStart.Length;
                            inside = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (!inside && pos < line.Length)
                {
                    // Write out until the end of the line
                    output.WriteLine(line.Substring(pos));
                }
            }
        }

        /// <summary>
        /// Removes single line comments from the source
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void RemoveSinglelineComments(TextReader input, TextWriter output)
        {
            string line;

            while ((line = input.ReadLine()) != null)
            {
                int i = line.IndexOf(SingleLineComment);
                if (i != -1)
                {
                    // Look for magic token after comment token
                    if (i + SingleLineComment.Length + MagicToken.Length < line.Length && line.Substring(i + SingleLineComment.Length, MagicToken.Length) == MagicToken)
                    {
                        // Keep whole line
                        output.WriteLine(line);
                    }
                    else
                    {
                        // No magic token, skip after single line comment token
                        output.WriteLine(line.Substring(0, i));
                    }
                }
                else
                {
                    // No comment token, keep whole line
                    output.WriteLine(line);
                }
            }
        }

        /// <summary>
        /// Extract xml from source by looking for comment + magic token combinations.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        private void ExtractXmlComments(TextReader input, TextWriter output)
        {
            output.WriteLine("<{0}>", Constants.TagMetadata);

            string line;

            while ((line = input.ReadLine()) != null)
            {
                int i = line.IndexOf(SingleLineComment + MagicToken);
                if (i != -1)
                    output.WriteLine(line.Substring(i + SingleLineComment.Length + MagicToken.Length));
            }

            output.WriteLine("</{0}>", Constants.TagMetadata);
        }

        /// <summary>
        /// Adds xml tags for tables, functions etc.
        /// </summary>
        /// <remarks>
        /// This is for MSSQL scripts only.
        /// </remarks>
        /// <param name="script"></param>
        private void InsertObjectTags(StringBuilder script)
        {
            int i = 0;
            Match m;

            while ((m = CreateObjectRegex.Match(script.ToString(), i)).Success)
            {
                string tagname = m.Groups[1].Value.ToLower();
                ObjectType ot = (ObjectType)Enum.Parse(typeof(ObjectType), tagname, true);      //*** TODO: support
                string tag = "\r\n" + SingleLineComment + MagicToken + " <" + tagname + " name=\"" + QuoteObjectName(m.Groups[2].Value) + "\">\r\n";
                script.Insert(m.Index + m.Length, tag);

                // Find matching closing bracket
                int ct = FindClosingBracket(script, m.Index + m.Length);

                i = UpdateParamTags(script, Constants.TagPairs[ot], m.Index + m.Length, ct);

                // Look for RETURN @table TABLE sequence right after the closing bracket in case of table valued functions
                Match mm = ReturnsTableRegex.Match(script.ToString(), i + 1);
                if (mm.Success && mm.Index == i + 1)
                {
                    ct = FindClosingBracket(script, mm.Index + mm.Length);
                    i = UpdateParamTags(script, Constants.TagPairs[ObjectType.Table], mm.Index + mm.Length, ct);
                }

                tag = "\r\n" + SingleLineComment + MagicToken + " </" + tagname + ">\r\n";
                script.Insert(i, tag);
                i += tag.Length;
            }
        }

        /// <summary>
        /// Adds xml tags for columns, paramters etc.
        /// </summary>
        /// <remarks>
        /// This is for MSSQL scripts only.
        /// </remarks>
        /// <param name="script"></param>
        /// <param name="tagname"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private int UpdateParamTags(StringBuilder script, List<ParameterType> tagnames, int start, int end)
        {
            int i = start;
            Match m;

            while ((m = XmlTagRegex.Match(script.ToString(), i, end - i)).Success)
            {
                i = m.Index + m.Length;

                ParameterType pt;

                if (Enum.TryParse<ParameterType>(m.Groups[1].Value, true, out pt))
                {
                    if (tagnames.Contains(pt))
                    {
                        // Find first token in the row, that's the name of the column

                        // Find end of the line
                        int j = script.ToString().LastIndexOf("\r\n", i);
                        if (j == -1)
                            j = 0;
                        else
                            j += 2; // skip \r\n

                        Match mn = ParamNameRegex.Match(script.ToString(), j);

                        string attribute = " name=\"" + mn.Groups[1].Value + "\"";

                        script.Insert(m.Groups[1].Index + m.Groups[1].Length, attribute);

                        end += attribute.Length;
                        i += attribute.Length;
                    }
                }
            }

            return end;
        }

        private int FindClosingBracket(StringBuilder script, int start)
        {
            int bcount = 1;

            int i = start;
            while (i < script.Length)
            {
                // Skip comments
                if (script[i] == SingleLineComment[0]
                    && i + SingleLineComment.Length < script.Length
                    && script.ToString().Substring(i, SingleLineComment.Length) == SingleLineComment)
                {
                    i = script.ToString().IndexOf("\r\n", i);
                    if (i == -1)
                    {
                        // end reached, everything was comment
                        return -1;
                    }
                    else
                    {
                        // Skip \r\n (two characters)
                        i += 2;
                    }
                }


                if (script[i] == '(')
                    bcount++;
                else if (script[i] == ')')
                    bcount--;

                // closing bracket found
                if (bcount == 0)
                    return i;

                i++;
            }

            return -1;
        }

        private string QuoteObjectName(string name)
        {
            string res = string.Empty;
            Match m = ObjectNameRegex.Match(name);

            for (int i = 1; i < m.Groups.Count; i++)
            {
                if (m.Groups[i].Value != string.Empty)
                {
                    if (i > 1) res += ".";

                    if (m.Groups[i].Value.StartsWith("["))
                    {
                        res += m.Groups[i].Value;
                    }
                    else
                    {
                        res += "[" + m.Groups[i].Value + "]";
                    }
                }
            }

            return res;
        }
    }
}
