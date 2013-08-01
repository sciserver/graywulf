using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
//using Jhu.Graywulf.Registry;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Metadata;


namespace Jhu.Graywulf.Metadata.CmdLineUtil
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            List<Type> verbs = new List<Type>() { typeof(Parse), typeof(Import) };

            Verb v = null;

            try
            {
                PrintHeader();
                v = (Verb)ArgumentParser.Parse(args, verbs);
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine();

                ArgumentParser.PrintUsage(verbs, Console.Out);
            }

            if (v != null)
            {
                v.Run();
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine(
@"JHU Graywulf Metadata Command Line Utility
(c) 2008-2013 László Dobos dobos@pha.jhu.edu
Department of Physics and Astronomy, The Johns Hopkins University

");
        }

#if false
        enum CommandType
        {
            Create,
            Verify,
            Validate,
            Delete,
            Export,
            Parse,
        }

        class Parameters
        {
            public CommandType CommandType;
            public SqlConnectionStringBuilder ConnectionString = new SqlConnectionStringBuilder();
            public string InputFile;
            public string OutputFile;
        }



        static void Main(string[] args)
        {
            PrintHeader();

            List<string> argsl = new List<string>(args);
            Parameters par = ProcessArgs(argsl);

            switch (par.CommandType)
            {
                case CommandType.Create:
                    CreateMeta(par);
                    break;
                case CommandType.Verify:
                    VerifyMeta(par);
                    break;
                case CommandType.Validate:
                    ValidateMeta(par);
                    break;
                case CommandType.Delete:
                    DeleteMeta(par);
                    break;
                case CommandType.Export:
                    ExportMeta(par);
                    break;
                case CommandType.Parse:
                    ParseMeta(par);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static Parameters ProcessArgs(List<string> args)
        {
            try
            {
                Parameters par = new Parameters();
                string op;

                // Command
                op = args[0];
                par.CommandType = (CommandType)Enum.Parse(typeof(CommandType), op, true);
                args.RemoveAt(0);

                par.InputFile = GetArgument(args, "-i");
                par.OutputFile = GetArgument(args, "-o");
                par.ConnectionString.UserID = GetArgument(args, "-U");
                par.ConnectionString.Password = GetArgument(args, "-P");
                par.ConnectionString.DataSource = GetArgument(args, "-S");
                par.ConnectionString.IntegratedSecurity = HasOption(args, "-E");
                par.ConnectionString.InitialCatalog = GetArgument(args, "-d");

                return par;
            }
            catch (Exception ex)
            {
                PrintUsage();
                throw ex;
            }
        }

        private static string GetArgument(List<string> args, string name)
        {
            int i = args.IndexOf(name);
            if (i >= 0)
            {
                string value = args[i + 1];

                args.RemoveAt(i);
                args.RemoveAt(i);

                return value;
            }
            else
            {
                return string.Empty;
            }
        }

        private static bool HasOption(List<string> args, string name)
        {
            int i = args.IndexOf(name);
            if (i >= 0)
            {
                args.RemoveAt(i);
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine(
@"JHU Graywulf Metadata Command Line Utility
(c) 2008-2011 László Dobos dobos@pha.jhu.edu
Department of Physics and Astronomy, The Johns Hopkins University

");
        }

        private static void PrintUsage()
        {
            Console.WriteLine(
@"Usage: metautil <command> [parameter1] [parameter2] ...
  command:
    create:   Stores metadata in the database.
    verify:   Verifies metadata in the database base on a script.
    validate: Validates metadata and reports problems.
    delete:   Deletes metadata from the database.
    export:   Exports metadata to xml from the database.
    parse:    Parses a sql script and extracts metadata
    
  parameters:
    -i Input file               -o Output file
    -U User ID                  -P Password
    -S Database server          -E Integrated security
    -d Database name
");
        }

        /*
        private static SqlConnectionStringBuilder GetGraywulfDatabaseDefinitionConnectionString(string itemName)
        {
            // Read connection string from cluster schema
            using (Context context = ContextManager.Instance.CreateContext(true))
            {
                EntityFactory f = new EntityFactory(context);
                DatabaseDefinition dd = (DatabaseDefinition)f.LoadStronglyTypedEntity(itemName);

                return dd.GetConnectionString();
            }
        }
         * */

        private static void CreateMeta(Parameters par)
        {
            string meta = File.ReadAllText(par.InputFile);

            // Load input
            if (Path.GetExtension(par.InputFile) == ".sql")
            {
                Console.Write("Parsing SQL script... ");
                Parser p = new Parser();
                meta = p.Parse(meta);
                Console.WriteLine("done.");
            }

            Generator g = new Generator(par.ConnectionString);

            Console.Write("Loading XML... ");
            g.LoadXml(meta);
            Console.WriteLine("done.");

            Console.Write("Creating metadata... ");
            g.CreateMetadata();
            Console.WriteLine("done.");
        }

        private static void VerifyMeta(Parameters par)
        {
            throw new NotImplementedException();
        }

        private static void ValidateMeta(Parameters par)
        {
            throw new NotImplementedException();
        }

        private static void DeleteMeta(Parameters par)
        {
            throw new NotImplementedException();
        }

        private static void ExportMeta(Parameters par)
        {
            Console.Write("Dumping metadata... ");

            Extractor x = new Extractor(par.ConnectionString);
            File.WriteAllText(par.OutputFile, x.ExtractMetadata());

            Console.WriteLine("done.");
        }

        private static void ParseMeta(Parameters par)
        {
            string meta = File.ReadAllText(par.InputFile);

            // Load input
            Console.Write("Parsing SQL script... ");
            Parser p = new Parser();
            meta = p.Parse(meta);
            Console.WriteLine("done.");

            string filename = (par.OutputFile == string.Empty) ? par.InputFile += ".xml" : par.OutputFile;

            Console.Write("Saving XML... ");
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(meta);
            xml.Save(filename);
            Console.WriteLine("done.");
        }

        /*
        private static void Parse(string fileName, string outFileName)
        {
            string buffer;

            // Load script file
            using (StreamReader infile = new StreamReader(fileName))
            {
                buffer = infile.ReadToEnd();
            }

            MetadataParser p = new MetadataParser();

            string parsed = p.Parse(buffer);

            // Write out XML
            using (StreamWriter outfile = new StreamWriter(outFileName))
            {
                outfile.Write(parsed);
            }

            Console.WriteLine("Parsed metadata comments written into file {0}.", outFileName);

        }

        private static void Meta(string itemName, string fileName, bool parse)
        {
            string buffer;
            string parsed;
            string cstr;

            // Read connection string from cluster schema
            using (Context context = ContextManager.Instance.CreateContext(true))
            {
                EntityFactory f = new EntityFactory(context);
                DatabaseDefinition dd = (DatabaseDefinition)f.LoadStronglyTypedEntity(itemName);

                cstr = dd.GetConnectionString().ConnectionString;
            }

            // Load script file
            using (StreamReader infile = new StreamReader(fileName))
            {
                buffer = infile.ReadToEnd();
            }

            if (parse)
            {
                MetadataParser p = new MetadataParser();
                parsed = p.Parse(buffer);
            }
            else
            {
                parsed = buffer;    // just copy, it's already xml
            }

            // Create metadata
            using (MetadataGenerator f = new MetadataGenerator(cstr))
            {
                f.LoadXml(parsed);
                f.CreateMetadata();
            }

            Console.WriteLine("Metadata generated successfully.");
        }

        private static void IndexMap(string itemName)
        {
            string cstr;
            // Read connection string from cluster schema
            using (Context context = ContextManager.Instance.CreateContext(true))
            {
                EntityFactory f = new EntityFactory(context);
                DatabaseDefinition dd = (DatabaseDefinition)f.LoadStronglyTypedEntity(itemName);

                cstr = dd.GetConnectionString().ConnectionString;
            }

            // Create metadata
            using (MetadataGenerator f = new MetadataGenerator(cstr))
            {
                // **** TODO f.PopulateIndexMapTable();
            }

            Console.WriteLine("IndexMap generated successfully.");
        }
         * */

        private static string[] SplitSqlScript(string script)
        {
            return script.Split(new string[] { "\r\nGO" }, StringSplitOptions.RemoveEmptyEntries);
        }
#endif
    }
}
