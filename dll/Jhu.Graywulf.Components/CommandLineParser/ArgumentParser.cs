using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.CommandLineParser
{
    public class ArgumentParser
    {
        public static object Parse(string[] args, List<Type> verbTypes)
        {
            ArgumentParser parser = new ArgumentParser();
            return parser.ParseInternal(args, verbTypes);
        }

        public static void PrintUsage(List<Type> verbTypes, TextWriter output)
        {
            ArgumentParser parser = new ArgumentParser();
            parser.PrintUsageInternal(verbTypes, output);
        }

        // ----

        List<string> args;
        Dictionary<string, Type> verbs;

        private bool ignoreCase;
        private bool acceptPartial;
        private bool verbRequired;
        private bool customParameters;
        private readonly char[] parameterMarkers = { '-', '/' };

        protected ArgumentParser()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.ignoreCase = true;
            this.acceptPartial = true;
            this.verbRequired = true;
            this.customParameters = false;
        }

        public void PrintUsageInternal(List<Type> verbTypes, TextWriter output)
        {
            output.WriteLine("Syntax:");
            output.WriteLine();
            output.WriteLine("  {0} <verb> [-parameter <value>] [-option]", Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
            output.WriteLine();
            output.WriteLine("Verbs:");

            foreach (Type t in verbTypes)
            {
                VerbAttribute a = (VerbAttribute)t.GetCustomAttributes(typeof(VerbAttribute), false)[0];

                output.WriteLine("  {0}: {1}", a.Name, a.Description);
                output.WriteLine();
                output.WriteLine("    Parameters and options:");
                output.WriteLine();

                Dictionary<string, PropertyInfo> pars;
                Dictionary<string, ParameterAttribute> atts;
                ReflectVerbType(t, out pars, out atts);

                foreach (string key in pars.Keys)
                {
                    if (atts[key] is OptionAttribute)
                    {
                        output.WriteLine(
                            "      -{0,-25}{1}",
                            atts[key].Name,
                            atts[key].Description);
                    }
                    else if (pars[key].PropertyType.IsEnum)
                    {
                        output.WriteLine(
                            "      -{0,-25}{1}{2}",
                            String.Format("{0} <enum>",
                                atts[key].Name,
                                pars[key].PropertyType.Name),
                                atts[key].Description,
                                atts[key].Required ? " Required." : "");

                        var names = Enum.GetNames(pars[key].PropertyType);
                        var values = Enum.GetValues(pars[key].PropertyType);

                        for (int i = 0; i < names.Length; i++)
                        {
                            output.WriteLine(
                                "          {0} {1}",
                                Convert.ToInt64(values.GetValue(i)),
                                names[i]);
                        }
                    }
                    else
                    {
                        output.WriteLine(
                            "      -{0,-25}{1}{2}",
                            String.Format("{0} <{1}>", 
                                atts[key].Name,  
                                pars[key].PropertyType.Name),
                                atts[key].Description,
                                atts[key].Required ? " Required." : "");
                    }
                }

                output.WriteLine();
            }
        }

        public object ParseInternal(string[] argsa, List<Type> verbTypes)
        {
            args = new List<string>(argsa);
            verbs = InitializeVerbTypes(verbTypes);

            object verbobj = GetVerbTypeObject(GetVerb());

            ParseParameters(verbobj);

            return verbobj;
        }

        private Dictionary<string, Type> InitializeVerbTypes(List<Type> verbTypes)
        {
            Dictionary<string, Type> res = new Dictionary<string, Type>(GetComparer());

            foreach (Type t in verbTypes)
            {
                object[] attrs = t.GetCustomAttributes(typeof(VerbAttribute), false);
                if (attrs.Length != 1)
                {
                    throw new ArgumentParserException(
                        String.Format(ExceptionMessages.VerbAttributeError, t.FullName));
                }

                VerbAttribute a = (VerbAttribute)attrs[0];
                if (res.ContainsKey(a.Name))
                {
                    throw new ArgumentParserException(
                        String.Format(ExceptionMessages.VerbDuplicate, t.FullName));
                }

                res.Add(a.Name, t);
            }

            return res;
        }

        protected string GetVerb()
        {
            if (args.Count == 0 && verbRequired)
            {
                throw new ArgumentParserException(ExceptionMessages.VerbRequired);
            }

            string res = args[0];
            args.RemoveAt(0);

            return res;
        }

        protected object GetVerbTypeObject(string name)
        {
            if (!verbs.ContainsKey(name))
            {
                throw new ArgumentParserException(
                    String.Format(ExceptionMessages.VerbInvalid, name));
            }

            Type t = verbs[name];
            return Activator.CreateInstance(t);
        }

        protected void ParseParameters(object verbobj)
        {
            StringComparer comp = GetComparer();

            // Reflect verb type
            Dictionary<string, PropertyInfo> pars;
            Dictionary<string, ParameterAttribute> atts;

            ReflectVerbType(verbobj.GetType(), out pars, out atts);

            string par = null;

            // Read all arguments and options
            HashSet<string> setvals = new HashSet<string>(GetComparer());
            while ((par = GetNextParameterName()) != null)
            {
                // Find key matches
                string[] keys = pars.Keys.Where(x => comp.Equals(x, par)).ToArray();

                if (keys.Length == 0)
                {
                    if (!customParameters)
                    {
                        throw new ArgumentParserException(
                            String.Format(ExceptionMessages.ParameterInvalid, par));
                    }
                    else
                    {
                        throw new NotImplementedException();

                        // Implement logic to parse a custom set of arguments and options
                    }
                }
                else if (keys.Length > 1)
                {
                    throw new ArgumentParserException(
                        String.Format(ExceptionMessages.ParameterAmbigous, par));
                }
                else
                {
                    string key = keys[0];

                    if (setvals.Contains(atts[key].Name))
                    {
                        throw new ArgumentParserException(
                            String.Format(ExceptionMessages.ParameterDuplicate, key));
                    }

                    if (atts[key] is OptionAttribute)
                    {
                        SetOptionValue(verbobj, pars[key], true);
                    }
                    else if (atts[par] is ParameterAttribute)
                    {
                        string val = GetNextParameterValue();
                        if (val == null)
                        {
                            throw new ArgumentParserException(
                                String.Format(ExceptionMessages.ParameterValueRequired, par));
                        }

                        SetParameterValue(verbobj, pars[key], val);
                    }

                    setvals.Add(atts[key].Name);
                }
            }

            // Check for parameters not set
            foreach (ParameterAttribute a in atts.Values)
            {
                if (a.Required && !setvals.Contains(a.Name))
                {
                    throw new ArgumentParserException(
                        String.Format(ExceptionMessages.ParameterRequired, a.Name));
                }
            }
        }

        protected string GetNextParameterName()
        {
            if (args.Count == 0)
            {
                // No more parameters
                return null;
            }

            string res;

            res = args[0];
            if (parameterMarkers.Contains(res[0]))
            {
                args.RemoveAt(0);
                res = res.TrimStart(parameterMarkers);
                return res;
            }
            else
            {
                throw new ArgumentParserException(
                    String.Format(ExceptionMessages.ParameterExpected, res));
            }
        }

        protected string GetNextParameterValue()
        {
            if (args.Count == 0)
            {
                return null;
            }

            string res = args[0];
            args.RemoveAt(0);

            return res;
        }

        protected void ReflectVerbType(Type t, out Dictionary<string, PropertyInfo> pars, out Dictionary<string, ParameterAttribute> atts)
        {
            pars = new Dictionary<string, PropertyInfo>(GetComparer());
            atts = new Dictionary<string, ParameterAttribute>(GetComparer());

            PropertyInfo[] props = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < props.Length; i++)
            {
                object[] aa = props[i].GetCustomAttributes(true);

                bool found = false;
                for (int k = 0; k < aa.Length; k++)
                {
                    if (aa[k] is ParameterAttribute)
                    {
                        ParameterAttribute a = (ParameterAttribute)aa[k];

                        if (found)
                        {
                            throw new ArgumentParserException(
                                String.Format(ExceptionMessages.ParameterDuplicateName, a.Name, t.FullName));
                        }

                        found = true;
                        MethodInfo pm;

                        // Check if option is boolean
                        if (a is OptionAttribute)
                        {
                            if (props[i].PropertyType != typeof(bool))
                            {
                                throw new ArgumentParserException(
                                    String.Format(ExceptionMessages.OptionNotBool, a.Name, t.FullName));
                            }
                        }
                        else if (props[i].PropertyType != typeof(string)
                            && !HasParseMethod(props[i].PropertyType, out pm)
                            && !props[i].PropertyType.IsSubclassOf(typeof(Enum)))
                        {
                            throw new ArgumentParserException(
                                String.Format(ExceptionMessages.UnsupportedType, props[i].PropertyType.FullName, a.Name, t.FullName));
                        }

                        pars.Add(a.Name, props[i]);
                        atts.Add(a.Name, a);
                    }
                }
            }
        }

        protected void SetOptionValue(object verbobj, PropertyInfo prop, bool value)
        {
            prop.SetValue(verbobj, value, null);
        }

        protected void SetParameterValue(object verbobj, PropertyInfo prop, string value)
        {
            MethodInfo parseMethod;

            if (prop.PropertyType == typeof(string))
            {
                prop.SetValue(verbobj, value, null);
            }
            else if (prop.PropertyType.IsSubclassOf(typeof(Enum)))
            {
                prop.SetValue(verbobj, Enum.Parse(prop.PropertyType, value, ignoreCase), null);
            }
            else if (HasParseMethod(prop.PropertyType, out parseMethod))
            {
                object v = Activator.CreateInstance(prop.PropertyType);
                object vv = parseMethod.Invoke(v, new object[] { value, System.Globalization.CultureInfo.InvariantCulture });
                prop.SetValue(verbobj, vv, null);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected bool HasParseMethod(Type t, out MethodInfo parseMethod)
        {
            parseMethod = t.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });

            return (parseMethod != null);
        }

        private StringComparer GetComparer()
        {
            StringComparer res;

            if (ignoreCase)
            {
                res = StringComparer.InvariantCultureIgnoreCase;
            }
            else
            {
                res = StringComparer.InvariantCulture;
            }

            if (acceptPartial)
            {
                res = new PartialStringComparer(res);
            }

            return res;
        }
    }
}
