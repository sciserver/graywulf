using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO.Tasks
{
    public abstract class ImportTableTestBase : TestClassBase
    {
        protected virtual FileFormatFactory CreateFileFormatFactory()
        {
            return FileFormatFactory.Create(null);
        }

        protected ServiceModel.ServiceProxy<IImportTable> GetImportTableTask(
            CancellationContext cancellationContext, string path, bool remote, bool generateIdentityColumn,
            out DataFileBase source, out DestinationTable destination, out TableCopySettings settings)
        {
            var ds = IOTestDataset;
            ds.IsMutable = true;

            var table = GetTestUniqueName();
            var ff = CreateFileFormatFactory();
            source = ff.CreateFile(new Uri(path, UriKind.RelativeOrAbsolute));
            source.GenerateIdentityColumn = generateIdentityColumn;
            destination = new DestinationTable()
            {
                Dataset = ds,
                DatabaseName = ds.DatabaseName,
                SchemaName = ds.DefaultSchemaName,
                TableNamePattern = table,
                Options = Sql.Schema.TableInitializationOptions.Create | Sql.Schema.TableInitializationOptions.GenerateUniqueName
            };
            settings = new TableCopySettings()
            {
            };

            ServiceModel.ServiceProxy<IImportTable> it = null;
            if (remote)
            {
                it = RemoteServiceHelper.CreateObject<IImportTable>(cancellationContext, Test.Constants.Localhost, false);
            }
            else
            {
                it = new ServiceModel.ServiceProxy<IImportTable>(new ImportTable(cancellationContext));
            }

            return it;
        }

        protected async Task<Jhu.Graywulf.Sql.Schema.Table> ExecuteImportTableTaskAsync(IImportTable it, DataFileBase source, DestinationTable destination, TableCopySettings settings)
        {
            var t = destination.GetTable();
            DropTable(t);

            await it.ExecuteAsyncEx(source, destination, settings);

            var table = destination.GetTable();

            return table;
        }
    }
}
