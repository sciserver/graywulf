using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class ImportTable : PageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/ImportTable.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // TODO: file format factory goes into federation settings
                var dfs = FileFormatFactory.GetFileFormatDescriptions();

                foreach (var df in dfs)
                {
                    var li = new ListItem(df.Value.DisplayName, df.Key);
                    FileFormat.Items.Add(li);
                }

                RefreshForm();
            }
        }

        private void GetUniqueTableName(string schemaName, ref string tableName)
        {
            string newname = tableName;
            int q = 1;
            
            while (MyDBDataset.GetObject(MyDBDataset.DatabaseName, schemaName, newname) != null)
            {
                newname = String.Format("{0}_{1}", tableName, q);
                q++;
            }

            tableName = newname;
        }

        private DestinationTableParameters CreateDestination(string schemaName, string tableName)
        {
            GetUniqueTableName(schemaName, ref tableName);

            var destination = new DestinationTableParameters();
            destination.Table = new Graywulf.Schema.Table()
            {
                Dataset = MyDBDatabaseInstance.GetDataset(),
                SchemaName = schemaName,
                TableName = tableName
            };
            destination.Operation = DestinationTableOperation.Create;

            return destination;
        }

        private DataFileImporter CreateImporterSimple()
        {
            string filename, extension;
            CompressionMethod compression;

            // Determine file format
            var format = FileFormatFactory.GetFileFormatDescription(
                new Uri(ImportedFile.PostedFile.FileName),
                out filename,
                out extension,
                out compression);

            var source = FileFormatFactory.CreateFile(format);

            source.Compression = compression;
            source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination("dbo", filename.Replace(".", "_"));

            return new DataFileImporter(source, destination);
        }

        private DataFileImporter CreateImporterAdvanced()
        {
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);
            var source = FileFormatFactory.CreateFile(format);
            
            CompressionMethod compression;
            Enum.TryParse<CompressionMethod>(CompressionMethod.SelectedValue, out compression);

            source.Compression = compression;
            source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination(SchemaName.Text, TableName.Text);

            return new DataFileImporter(source, destination);
        }

        protected void RefreshForm()
        {
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);
            if (format.Type.IsSubclassOf(typeof(TextDataFile)))
            {
                ColumnNamesInFirstLineRow.Visible = true;
                GenerateIdentityRow.Visible = true;
                CompressedRow.Visible = true;
            }
            else
            {
                ColumnNamesInFirstLineRow.Visible = false;
                GenerateIdentityRow.Visible = false;
                CompressedRow.Visible = false;
            }

            if (DetailsTable.Visible)
            {
                ToggleDetails.Text = "simple mode";
            }
            else
            {
                ToggleDetails.Text = "advanced mode";
            }
        }

        protected void ToggleDetails_Click(object sender, EventArgs e)
        {
            DetailsTable.Visible = !DetailsTable.Visible;
            RefreshForm();
        }

        protected void FileFormat_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshForm();
        }

        protected void Ok_Click(object sender, EventArgs e)
        {
            if (IsValid)
            {
                DataFileImporter importer = null;

                if (DetailsTable.Visible)
                {
                    // Advanced mode
                    importer = CreateImporterAdvanced();
                }
                else
                {
                    // Simple mode
                    importer = CreateImporterSimple();
                }

                // Run import
                importer.Execute();

                Response.Redirect(Jhu.Graywulf.Web.UI.MyDB.Tables.GetUrl());
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }
    }
}