using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class ImportTable : UserPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/ImportTable.aspx";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

                foreach (var df in dfs)
                {
                    if (df.CanRead)
                    {
                        var li = new ListItem(df.DisplayName, df.Extension);
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

            while (FederationContext.MyDBDataset.GetObject(FederationContext.MyDBDataset.DatabaseName, schemaName, newname) != null)
            {
                newname = String.Format("{0}_{1}", tableName, q);
                q++;
            }

            tableName = newname;
        }

        private DestinationTable CreateDestination(string schemaName, string tableName)
        {
            GetUniqueTableName(schemaName, ref tableName);

            var destination = new DestinationTable(
                FederationContext.MyDBDataset,
                FederationContext.MyDBDataset.DatabaseName,
                schemaName,
                tableName,
                Graywulf.Schema.TableInitializationOptions.Create);

            return destination;
        }

        private Uri GetUploadedFileUri()
        {
            // Now this is tricky here because we don't know anything about the
            // file path format the browser sends us. Lets's assume it has a '/', '\' or ':'
            // in the path before the file name, or if not, the whole string is
            // a file name.

            var filename = ImportedFile.PostedFile.FileName;
            int i;

            i = filename.LastIndexOf('/');
            i = Math.Max(i, filename.LastIndexOf('\\'));
            i = Math.Max(i, filename.LastIndexOf(':'));

            if (i >= 0)
            {
                filename = filename.Substring(i + 1);
            }

            // TODO: test this method with all browses and various types
            // containing fancy characters

            return new Uri(filename, UriKind.Relative);
        }

        private IO.Tasks.ImportTable CreateImporterSimple()
        {
            string filename, extension;
            DataFileCompression compression;

            // Determine file format and create a file
            var source = FederationContext.FileFormatFactory.CreateFile(
                GetUploadedFileUri(),
                out filename,
                out extension,
                out compression);

            source.BaseStream = ImportedFile.PostedFile.InputStream;
            //source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination("dbo", filename.Replace(".", "_")); // TODO: get 'dbo' from dataset description

            return new IO.Tasks.ImportTable()
            {
                Source = source,
                Destination = destination
            };
        }

        private IO.Tasks.ImportTable CreateImporterAdvanced()
        {
            throw new NotImplementedException();

            // TODO this simply needs more testing

            /*
            var format = FileFormatFactory.GetFileFormatDescription(FileFormat.SelectedValue);
            var source = FileFormatFactory.CreateFile(format);

            DataFileCompression compression;
            Enum.TryParse<DataFileCompression>(CompressionMethod.SelectedValue, out compression);

            // TODO: check if workssource.Compression = compression;
            source.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read);

            var destination = CreateDestination(SchemaName.Text, TableName.Text);

            return new IO.Tasks.ImportTable()
            {
                Source = source,
                Destination = destination
            };
             * */
        }

        protected void RefreshForm()
        {
            var format = FederationContext.FileFormatFactory.CreateFileFromExtension(FileFormat.SelectedValue).Description;

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
                IO.Tasks.ImportTable importer = null;

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