using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.MyDB
{
    public class CopyTablePage : CustomPageBase
    {
        protected ListBox TableList;
        protected LinkButton ToggleAdvanced;
        protected Panel DetailsPanel;
        protected TextBox TableNamePrefix;
        protected TextBox SchemaName;
        protected BulletedList SupportedFormatsList;
        protected DropDownList FileFormatList;
        protected CheckBox GenerateIdentity;
        protected CheckBox AutoDetectColumns;

        #region Event handlers

        protected void ToggleAdvanced_Click(object sender, EventArgs e)
        {
            DetailsPanel.Visible = !DetailsPanel.Visible;

            if (DetailsPanel.Visible)
            {
                ToggleAdvanced.Text = "simple mode";
            }
            else
            {
                ToggleAdvanced.Text = "advanced mode";
            }
        }

        #endregion

        protected void RefreshFileFormatLists(bool canRead, bool canWrite)
        {
            var dfs = FederationContext.FileFormatFactory.EnumerateFileFormatDescriptions();

            foreach (var df in dfs)
            {
                if (canRead && df.CanRead || canWrite && df.CanWrite)
                {
                    var li = new ListItem(df.DisplayName, df.Extension);

                    if (FileFormatList != null)
                    {
                        FileFormatList.Items.Add(li);
                    }

                    if (SupportedFormatsList != null)
                    {
                        SupportedFormatsList.Items.Add(li);
                    }
                }
            }
        }

        protected void RefreshTableList()
        {
            FederationContext.MyDBDataset.Tables.LoadAll();

            foreach (var table in FederationContext.MyDBDataset.Tables.Values.OrderBy(t => t.UniqueKey))
            {
                TableList.Items.Add(new ListItem(table.DisplayName, table.UniqueKey));
            }
        }

        protected SourceTableQuery[] GetSourceTables(string[] tableKeys)
        {
            var sources = new SourceTableQuery[tableKeys.Length];

            for (int i = 0; i < tableKeys.Length; i++)
            {
                var table = FederationContext.MyDBDataset.Tables[tableKeys[i]];

                // TODO: maybe set a row maximum here
                sources[i] = SourceTableQuery.Create(table);
            }

            return sources;
        }

        protected DataFileBase[] GetDestinationFiles(string[] tableKeys, string format)
        {
            var destinations = new DataFileBase[tableKeys.Length];

            for (int i = 0; i < tableKeys.Length; i++)
            {
                var table = FederationContext.MyDBDataset.Tables[tableKeys[i]];

                destinations[i] = FederationContext.FileFormatFactory.CreateFileFromExtension(format);

                // TODO: maybe change file naming logic here?
                destinations[i].Uri = Util.UriConverter.FromFilePath(table.TableName + destinations[i].Description.Extension);
            }

            return destinations;
        }

        protected DataFileBase GetSourceDataFile(Uri uri)
        {
            DataFileBase file;

            if (DetailsPanel.Visible)
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

            return file;
        }
        
        protected DestinationTable GetDestinationTable()
        {
            DestinationTable destination;

            if (DetailsPanel.Visible)
            {
                destination = new DestinationTable(
                    FederationContext.MyDBDataset,
                    FederationContext.MyDBDataset.DatabaseName,
                    SchemaName.Text,
                    TableNamePrefix.Text + "_" + IO.Constants.ResultsetNameToken,
                    TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName);
            }
            else
            {
                // TODO: move unique name logic to importer class
                //var tableName = Util.UriConverter.ToFileNameWithoutExtension(uri).Replace('.', '_');
                //GetUniqueTableName(FederationContext.MyDBDataset.DefaultSchemaName, ref tableName);

                destination = new DestinationTable(
                    FederationContext.MyDBDataset,
                    FederationContext.MyDBDataset.DatabaseName,
                    FederationContext.MyDBDataset.DefaultSchemaName,
                    IO.Constants.ResultsetNameToken,        // generate table names automatically
                    TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName);
            }

            return destination;
        }
    }
}