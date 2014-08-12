using System;
using System.IO;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public partial class Upload : CustomPageBase
    {
        public static string GetUrl()
        {
            return "~/MyDb/Upload.aspx";
        }

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RefreshFileFormatLists();

                RefreshForm();
            }
        }

        protected void ToggleAdvanced_Click(object sender, EventArgs e)
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
                // Run import
                CopyTableBase importer = GetTableImporter();
                importer.Execute();

                Response.Redirect(Jhu.Graywulf.Web.UI.MyDB.Tables.GetUrl());
            }
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(OriginalReferer);
        }

        #endregion

        protected void RefreshForm()
        {
            if (DetailsTable.Visible)
            {
                ToggleAdvanced.Text = "simple mode";
            }
            else
            {
                ToggleAdvanced.Text = "advanced mode";
            }
        }

        private void RefreshFileFormatLists()
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (df.CanRead)
                {
                    var li = new ListItem(df.DisplayName, df.Extension);
                    FileFormatList.Items.Add(li);
                    SupportedFormatsList.Items.Add(li);
                }
            }
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

        private DataFileBase GetSourceDataFile(Uri uri)
        {
            DataFileBase file;

            if (DetailsTable.Visible)
            {
                // Create a specific file type
                file = FederationContext.FileFormatFactory.CreateFileFromExtension(FileFormatList.SelectedValue);
                file.GenerateIdentityColumn = GenerateIdentity.Checked;

                // TODO: add compression

                if (file is TextDataFileBase)
                {
                    ((TextDataFileBase)file).AutoDetectColumns = AutoDetectColumns.Checked;
                }
            }
            else
            {
                // Create file type based on extension
                file = FederationContext.FileFormatFactory.CreateFile(uri);
                file.GenerateIdentityColumn = true;

                // AutoDetectColumns is turned on by default in this case
            }

            file.BaseStream = FederationContext.StreamFactory.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read, file.Compression, DataFileArchival.None);
            return file;
        }

        private DestinationTable GetDestinationTable(Uri uri)
        {
            DestinationTable destination;

            if (DetailsTable.Visible)
            {
                destination = new DestinationTable(
                    FederationContext.MyDBDataset,
                    FederationContext.MyDBDataset.DatabaseName,
                    SchemaName.Text,
                    TableNamePrefix.Text + "_" + IO.Constants.ResultsetNameToken,
                    Graywulf.Schema.TableInitializationOptions.Create);
            }
            else
            {
                var tableName = Util.UriConverter.ToFileNameWithoutExtension(uri).Replace('.', '_');
                GetUniqueTableName(FederationContext.MyDBDataset.DefaultSchemaName, ref tableName);

                destination = new DestinationTable(
                    FederationContext.MyDBDataset,
                    FederationContext.MyDBDataset.DatabaseName,
                    FederationContext.MyDBDataset.DefaultSchemaName,
                    tableName,
                    Graywulf.Schema.TableInitializationOptions.Create);
            }

            return destination;
        }

        private CopyTableBase GetTableImporter()
        {
            var uri = GetUploadedFileUri();

            // Check if uploaded file is an archive
            var archival = FederationContext.StreamFactory.GetArchivalMethod(uri);
            var compression = FederationContext.StreamFactory.GetCompressionMethod(uri);
            var batchName = Util.UriConverter.ToFileNameWithoutExtension(uri);

            if (archival == DataFileArchival.None)
            {
                // Use simple importer
                var task = new ImportTable()
                {
                    BatchName = batchName,
                    Source = GetSourceDataFile(uri),
                    Destination = GetDestinationTable(uri),
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                };

                task.Open();

                return task;
            }
            else
            {
                var task = new ImportTableArchive()
                {
                    BatchName = batchName,
                    Destination = GetDestinationTable(uri),
                    StreamFactoryType = RegistryContext.Federation.StreamFactory,
                    FileFormatFactoryType = RegistryContext.Federation.FileFormatFactory,
                };

                task.Open(FederationContext.StreamFactory.Open(ImportedFile.PostedFile.InputStream, DataFileMode.Read, compression, archival));

                return task;
            }
        }
    }
}