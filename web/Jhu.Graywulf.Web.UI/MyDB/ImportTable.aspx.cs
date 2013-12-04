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
                var dfs = FileFormatFactory.GetFileFormatDescriptions();

                foreach (var df in dfs)
                {
                    if (df.Value.CanRead)
                    {
                        var li = new ListItem(df.Value.DisplayName, df.Key);
                        FileFormat.Items.Add(li);
                    }
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
            DataFileCompression compression;

            // Determine file format
            var format = FileFormatFactory.GetFileFormatDescription(
                new Uri(ImportedFile.PostedFile.FileName),
                out filename,
                out extension,
                out compression);

            var source = FileFormatFactory.CreateFile(format);

            // TODO: check and delete if works source.Compression = compression;
            source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination("dbo", filename.Replace(".", "_")); // TODO: get 'dbo' from dataset description
            

            return new DataFileImporter(source, destination);
        }

        private DataFileImporter CreateImporterAdvanced()
        {
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);
            var source = FileFormatFactory.CreateFile(format);
            
            DataFileCompression compression;
            Enum.TryParse<DataFileCompression>(CompressionMethod.SelectedValue, out compression);

            // TODO: check if workssource.Compression = compression;
            source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination(SchemaName.Text, TableName.Text);

            return new DataFileImporter(source, destination);
        }

        protected void RefreshForm()
        {
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);

            DetectColumnNamesRow.Visible = format.CanDetectColumnNames;
            CompressedRow.Visible = !format.IsCompressed;

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