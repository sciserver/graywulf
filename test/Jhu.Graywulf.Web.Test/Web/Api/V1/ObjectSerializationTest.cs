using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Web.Api.V1
{
    [TestClass]
    public class ObjectSerializationTest
    {
        private void TryXmlSerializer(object value)
        {
            var ms = new MemoryStream();
            var serializer = new DataContractSerializer(value.GetType());

            serializer.WriteObject(ms, value);
        }

        private void TryJsonSerializer(object value)
        {
            var ms = new MemoryStream();
            var serializer = new DataContractJsonSerializer(value.GetType());

            serializer.WriteObject(ms, value);
        }

        [TestMethod]
        public void SerializeTableTest()
        {
            var value = new Table();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeTableListTest()
        {
            var value = new TableListResponse();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeColumnTest()
        {
            var value = new Column();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeColumnListTest()
        {
            var value = new ColumnListResponse();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeDatasetTest()
        {
            var value = new Dataset();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeDatasetListTest()
        {
            var value = new DatasetListResponse();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeQueueTest()
        {
            var value = new Queue();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }

        [TestMethod]
        public void SerializeQueueListTest()
        {
            var value = new QueueListResponse();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }
        
        [TestMethod]
        public void SerializeQueryJobTest()
        {
            var job = new QueryJob();

            TryXmlSerializer(job);
            TryJsonSerializer(job);
        }

        [TestMethod]
        public void SerializeExportJobTest()
        {
            var job = new ExportJob();

            TryXmlSerializer(job);
            TryJsonSerializer(job);
        }

        [TestMethod]
        public void SerializeImportJobTest()
        {
            var job = new ImportJob();

            TryXmlSerializer(job);
            TryJsonSerializer(job);
        }

        [TestMethod]
        public void SerializeJobListTest()
        {
            var value = new JobListResponse();

            TryXmlSerializer(value);
            TryJsonSerializer(value);
        }
    }
}
