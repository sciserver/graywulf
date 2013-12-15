/* Copyright */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements static utility function used by the library
    /// </summary>
    public static class Util
    {

        // TODO: consider splitting this class into nice small ones

        private static Regex contextValueRegex = new Regex(@"\[\@([a-zA-Z]+)\]");        // Matches [@word]
        private static Regex entityNameRegex = new Regex(@"\[\$([a-zA-Z\.]+)\]");        // Matches [$word.word.word]

        /// <summary>
        /// Creates a deep copy of an array of type <b>T</b>
        /// </summary>
        /// <typeparam name="T">Type of the array elements</typeparam>
        /// <param name="oldArray">Old array</param>
        /// <param name="newArray">New array</param>
        public static void CopyArray<T>(T[] oldArray, out T[] newArray)
        {
            if (oldArray == null)
                newArray = null;
            else
            {
                newArray = new T[oldArray.Length];
                oldArray.CopyTo(newArray, 0);
            }
        }

        public static string ResolveExpression(Entity entity, string value)
        {
            return ResolveExpression(entity, value, 0);
        }

        private static string ResolveExpression(Entity entity, string value, int level)
        {
            if (level > 5)
            {
                throw new ArgumentException("Too deep recursion in expressions.");
            }

            string res = value;

            foreach (Match m in contextValueRegex.Matches(value))
            {
                string key = m.Groups[1].Value;

                if (StringComparer.InvariantCultureIgnoreCase.Compare(key, "username") == 0)
                {
                    res = res.Replace(m.Value, entity.Context.UserName);
                }
            }

            foreach (Match m in entityNameRegex.Matches(value))
            {
                Entity ee = entity;
                string[] parts = m.Groups[1].Value.Split('.');  // splits into parts along dots

                for (int i = 0; i < parts.Length; i++)
                {
                    if (ee == null)
                    {
                        throw new ArgumentNullException(ExceptionMessages.EntityNullException);
                    }

                    System.Reflection.PropertyInfo prop = ee.GetType().GetProperty(parts[i]);

                    // If the expression is parent, load the parent entity
                    if (string.Compare(parts[i], "Parent", true) == 0)
                    {
                        ee = entity.Parent;
                    }
                    else if (prop == null)
                    {
                        throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value));
                    }
                    else if (i != parts.Length - 1)
                    {
                        try
                        {
                            ee = (Entity)prop.GetValue(ee, null);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value), ex);
                        }
                    }
                    else
                    {
                        try
                        {
                            string v = null;
                            object vv = ee.GetType().GetProperty(parts[i]).GetValue(ee, null);

                            if (vv is ExpressionProperty)
                            {
                                v = ResolveExpression(((ExpressionProperty)vv).Entity, ((ExpressionProperty)vv).Value, level + 1);
                            }
                            else if (vv is string)
                            {
                                v = (string)vv;
                            }
                            else
                            {
                                v = vv.ToString();
                            }

                            res = res.Replace(m.Value, v);
                        }
                        catch (Exception ex)
                        {
                            throw new ArgumentException(String.Format(ExceptionMessages.InvalidExpression, m.Value), ex);
                        }
                    }
                }
            }

            return res;
        }

        public static Dictionary<K, string> LoadSettings<K>(string settings)
        {
            var xml = new XmlDocument();
            xml.LoadXml(settings);

            var res = new Dictionary<K, string>();

            foreach (XmlNode node in xml.DocumentElement.ChildNodes)
            {
                res.Add(
                    (K)Enum.Parse(typeof(K), node.Name, true),
                    node.Attributes["value"].Value);
            }

            return res;
        }

        public static string SaveSettings(IDictionary settings)
        {
            var xml = new XmlDocument();
            xml.AppendChild(xml.CreateElement("settings"));

            foreach (var key in settings.Keys)
            {
                var node = xml.CreateElement(key.ToString());
                var attr = xml.CreateAttribute("value");
                attr.Value = settings[key].ToString();
                node.Attributes.Append(attr);
                xml.DocumentElement.AppendChild(node);
            }

            return xml.InnerXml;
        }

        public static void RunSqlServerDiagnostics(string connectionString, DiagnosticMessage message)
        {
            var csb = new SqlConnectionStringBuilder(connectionString);
            csb.ConnectTimeout = 5;

            try
            {
                using (SqlConnection cn = new SqlConnection(csb.ConnectionString))
                {
                    cn.Open();
                    cn.Close();
                }

                message.Status = DiagnosticMessageStatus.OK;
            }
            catch (System.Exception ex)
            {
                message.Status = DiagnosticMessageStatus.Error;
                message.ErrorMessage = ex.Message;
            }
        }
    }
}
