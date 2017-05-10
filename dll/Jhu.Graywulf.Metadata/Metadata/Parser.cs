using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;

namespace Jhu.Graywulf.Metadata
{
    /// <summary>
    /// Implements functionality to scan a sql file for embedded XML comments.
    /// </summary>
    /// <remarks>
    /// The class works by scanning the SQL code for object definitions and
    /// tries to look up object and column names from the code which are
    /// automatically added to the XML tags as attributes. This way object names
    /// appear only once in the code and the corresponding xml tags always
    /// follow the objects' or the columns' definition.
    /// </remarks>
    public class Parser
    {
        private const string SingleLineComment = "--";
        private const string MultiLineCommentStart = "/*";
        private const string MultiLineCommentEnd = "*/";
        private const string MagicToken = "/";

        // Matches a single line starting with the magic token --/
        // group 1: the part containing xml tags
        private const string XmlComment = @"\G\s*--/(.*)";
        private static readonly Regex XmlCommentRegex = new Regex(XmlComment, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string AnyLine = @"\G.*";
        private static readonly Regex AnyLineRegex = new Regex(AnyLine, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string EmptyLine = @"\G\s*$";
        private static readonly Regex EmptyLineRegex = new Regex(EmptyLine, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string Identifier = @"(?:[a-z_][a-z_0-9]*)";
        private const string QuotedIdentifier = @"(@*(?:\[[^]]+\])|@*" + Identifier + @")";
        private const string ObjectName = QuotedIdentifier + @"(?:\." + QuotedIdentifier + @")*";
        private static readonly Regex ObjectNameRegex = new Regex(ObjectName, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex CreateObjectRegex = new Regex(@"\G\s*CREATE\s+([a-z]+)\s+(" + ObjectName + @")\s*\(", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex ReturnsTableRegex = new Regex(@"\s*RETURNS\s+" + QuotedIdentifier + @"\s+TABLE\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex GoRegex = new Regex(@"\G\s*GO\s*");

        private const string XmlTag = @"<([a-z]+)(?:(?:\s+[a-z]+)\s*=\s*(?:""[^""]*""))*\s*(/>|>.*</\1>)";
        private static readonly Regex XmlTagRegex = new Regex(XmlTag, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex ParamNameRegex = new Regex(@"\G\s*(" + QuotedIdentifier + @").*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        enum ExtractMode
        {
            Object,
            Column,
            Parameter
        }

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
        public XmlDocument Parse(string input)
        {
            var buffer = new StringWriter();
            var buffer2 = new StringWriter();
            var buffer3 = new StringWriter();

            RemoveMultilineComments(new StringReader(input), buffer);
            RemoveSinglelineComments(new StringReader(buffer.ToString()), buffer2);

            return ExtractXmlComments(buffer2.ToString());
        }

        #region Standard comment removal functions

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
                        {
                            break;
                        }


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

        #endregion

        private XmlDocument ExtractXmlComments(string script)
        {
            var doc = new XmlDocument();
            var metadata = doc.CreateElement("metadata");

            doc.AppendChild(metadata);

            ExtractXmlComments(script, 0, script.Length, ExtractMode.Object, metadata);

            return doc;
        }

        private void ExtractXmlComments(string script, int start, int end, ExtractMode mode, XmlElement element)
        {
            int i = start;
            Match m;

            var buffer = new StringBuilder();

            while (i < end)
            {
                // Try to match an xml comment line starting from the current character

                m = XmlCommentRegex.Match(script, i);

                if (m.Success)
                {
                    // the line contains an xml comment, add it to the buffer
                    buffer.AppendLine(m.Groups[1].Value.Trim());

                    i = m.Index + m.Length + 1;
                    continue;
                }

                if (mode == ExtractMode.Object)
                {
                    // Try to match a GO statement
                    // This means that all xml tags collected so far should
                    // be copied verbatim into the results
                    m = GoRegex.Match(script, i);

                    if (m.Success)
                    {
                        if (buffer.Length > 0)
                        {
                            var x = new XmlDocument();
                            x.LoadXml(buffer.ToString());
                            var nx = element.OwnerDocument.ImportNode(x.DocumentElement, true);
                            element.AppendChild(nx);

                            // Reset the buffer
                            buffer.Clear();
                        }

                        i = m.Index + m.Length + 1;
                        continue;
                    }

                    // Try to match a CREATE ... command
                    m = CreateObjectRegex.Match(script, i);

                    if (m.Success)
                    {
                        var r = FindClosingBracket(script, i + m.Length);

                        if (r < 0)
                        {
                            throw new Exception("Error in SQL script. Closing bracket was expected.");
                        }

                        // Create a tag around the object and parse the inner script
                        var tagname = m.Groups[1].Value.ToLower();
                        var name = QuoteObjectName(m.Groups[2].Value);

                        var ot = GetObjectType(tagname);
                        var tag = element.OwnerDocument.CreateElement(ot.ToString().ToLower());
                        tag.SetAttribute("name", name);
                        tag.InnerXml = buffer.ToString();

                        // Reset the buffer
                        buffer.Clear();

                        // Inner element parsing depends on the object type
                        ExtractMode innermode;

                        switch (ot)
                        {
                            case ObjectType.Table:
                            case ObjectType.View:
                                innermode = ExtractMode.Column;
                                break;
                            case ObjectType.Procedure:
                            case ObjectType.Function:
                                innermode = ExtractMode.Parameter;
                                break;
                            default:
                                throw new NotImplementedException();
                        }

                        // Call recursively for parameters of the object
                        ExtractXmlComments(script, i + m.Length + 1, r, innermode, tag);

                        element.AppendChild(tag);

                        i = r + 1;
                        continue;
                    }

                    // This is a line we can't do anything with, simply skip it
                    m = AnyLineRegex.Match(script, i);

                    if (m.Success)
                    {
                        i = m.Index + m.Length + 1;
                        continue;
                    }

                    // End of script is reached
                    break;
                }
                else if (mode == ExtractMode.Column || mode == ExtractMode.Parameter)
                {
                    // Try to match a CREATE ... command

                    // Skip all line containing any whitespace
                    m = EmptyLineRegex.Match(script, i);

                    if (m.Success)
                    {
                        i = m.Index + m.Length + 1;
                        continue;
                    }

                    // Match any line, the first token is the name of the parameter
                    m = ParamNameRegex.Match(script, i);

                    // We only consider those line which are preceded by xml comment tags
                    if (m.Success && buffer.Length > 0)
                    {
                        var tagname = mode.ToString().ToLower();
                        var name = m.Groups[1].Value;

                        var tag = element.OwnerDocument.CreateElement(tagname);
                        tag.SetAttribute("name", name);

                        try
                        {
                            tag.InnerXml = buffer.ToString();
                        }
                        catch (XmlException ex)
                        {
                            // Get line from xml
                            string line = null;
                            var r = new StringReader(buffer.ToString());
                            for (int q = 0; q < ex.LineNumber; q++)
                            {
                                line = r.ReadLine();
                            }

                            Console.WriteLine();
                            Console.WriteLine(line);
                            Console.WriteLine("{0}^", new String(' ', ex.LinePosition - 1));
                            throw new Exception(String.Format("Error in XML at {0}, {1}", ex.LineNumber, ex.LinePosition));
                        }

                        // Reset the buffer
                        buffer.Clear();

                        element.AppendChild(tag);

                        i = m.Index + m.Length + 1;
                        continue;
                    }

                    // This is a line we can't do anything with, simply skip it
                    m = AnyLineRegex.Match(script, i);

                    if (m.Success)
                    {
                        i = m.Index + m.Length + 1;
                        continue;
                    }

                    // End of script is reached
                    break;
                }
            }
        }

        private int FindClosingBracket(string script, int start)
        {
            int bcount = 1;

            int i = start;
            while (i < script.Length)
            {
                // We need to skip comments here because magic token
                // comments are still in the script

                if (script[i] == SingleLineComment[0]
                    && i + SingleLineComment.Length < script.Length
                    && script.Substring(i, SingleLineComment.Length) == SingleLineComment)
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
                {
                    bcount++;
                }
                else if (script[i] == ')')
                {
                    bcount--;
                }

                // closing bracket found
                if (bcount == 0)
                {
                    return i;
                }

                i++;
            }

            return -1;
        }

        private ObjectType GetObjectType(string tagname)
        {
            ObjectType ot;

            if (!Enum.TryParse(tagname, true, out ot))
            {
                if (StringComparer.InvariantCultureIgnoreCase.Compare("PROC", tagname) == 0)
                {
                    ot = ObjectType.Procedure;
                }
            }

            return ot;
        }

        private string QuoteObjectName(string name)
        {
            string res = string.Empty;
            Match m = ObjectNameRegex.Match(name);

            for (int i = 1; i < m.Groups.Count; i++)
            {
                if (m.Groups[i].Value != string.Empty)
                {
                    if (i > 1)
                    {
                        res += ".";
                    }

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
