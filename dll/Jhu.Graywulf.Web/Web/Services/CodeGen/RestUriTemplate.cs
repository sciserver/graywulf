using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestUriTemplate
    {
        private string value;
        private string path;
        private string query;
        private string[] pathParts;
        private string[] queryKeys;
        private string[] queryValues;
        private HashSet<string> allNames;

        public string Value
        {
            get { return Value; }
        }

        public string Path
        {
            get { return path; }
        }

        public string Query
        {
            get { return query; }
        }

        public string[] PathParts
        {
            get { return pathParts; }
        }

        public string[] QueryKeys
        {
            get { return queryKeys; }
        }

        public string[] QueryValues
        {
            get { return queryValues; }
        }

        public RestUriTemplate(string uriTemplate)
        {
            this.value = uriTemplate;
            Parse();
        }

        public bool IsPathParameter(int i)
        {
            return IsParameter(pathParts[i]);
        }

        public bool IsPathParameter(string name)
        {
            name = "{" + name + "}";

            for (int i = 0; i < pathParts.Length; i++)
            {
                if (pathParts[i] == name)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsQueryParameter(int i)
        {
            return IsParameter(queryValues[i]);
        }

        public bool IsQueryParameter(string name)
        {
            name = "{" + name + "}";

            for (int i = 0; i < queryValues.Length; i++)
            {
                if (queryValues[i] == name)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsBodyParameter(string name)
        {
            return !allNames.Contains(name);
        }

        private bool IsParameter(string part)
        {
            return (part.StartsWith("{") && part.EndsWith("}"));
        }

        private string GetParameterName(string part)
        {
            return part.TrimStart('{').TrimEnd('}');
        }

        private void Parse()
        {
            allNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            var parts = this.value.Split('?');

            if (parts.Length > 0 && parts[0] != null)
            {
                path = parts[0];
                query = parts.Length > 1 ? parts[1] : null;

                pathParts = parts[0].Split(new char[] { '/', '.' }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < pathParts.Length; i++)
                {
                    if (IsPathParameter(i))
                    {
                        allNames.Add(GetParameterName(pathParts[i]));
                    }
                }
            }
            else
            {
                pathParts = new string[0];
                path = null;
                value = null;
            }

            if (parts.Length > 1 && parts[1] != null)
            {
                var queryParts = parts[1].Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                queryKeys = new string[queryParts.Length];
                queryValues = new string[queryParts.Length];

                for (int i = 0; i < queryParts.Length; i++)
                {
                    var idx = queryParts[i].IndexOf('=');

                    if (idx < 0)
                    {
                        queryKeys[i] = queryParts[i];
                        queryValues[i] = "";
                    }
                    else if (idx > 0)
                    {
                        queryKeys[i] = queryParts[i].Substring(0, idx);
                        queryValues[i] = queryParts[i].Substring(idx + 1);

                        if (IsQueryParameter(i))
                        {
                            allNames.Add(GetParameterName(queryValues[i]));
                        }
                    }
                }
            }
            else
            {
                queryKeys = new string[0];
                queryValues = new string[0];
            }
        }
    }
}
