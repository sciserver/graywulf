using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Data;

namespace Jhu.Graywulf.Format
{
    [TestClass]
    public class DelimitedTextDataFileTest : Jhu.Graywulf.Test.TestClassBase
    {
        #region Reader tests

        FileDataReader OpenSimpleReader(string csv)
        {
            var f = new DelimitedTextDataFile(new StringReader(csv))
            {
                ColumnNamesInFirstLine = false,
                GenerateIdentityColumn = false,
            };
            var b = new DelimitedTextDataFileBlock(f);

            b.Columns.Add(new Column("one", typeof(int), 4));
            b.Columns.Add(new Column("two", typeof(int), 4));
            b.Columns.Add(new Column("three", typeof(int), 4));

            f.AppendBlock(b);

            var cmd = new FileCommand(f);

            return cmd.ExecuteReader();
        }

        [TestMethod]
        public void DefaultSettingsTest()
        {
            var csv =
@"1,2,3
4,5,6";

            var dr = OpenSimpleReader(csv);

            dr.Read();

            Assert.AreEqual(1, dr.GetInt32(0));
            Assert.AreEqual(2, dr.GetInt32(1));
            Assert.AreEqual(3, dr.GetInt32(2));

            dr.Read();

            Assert.AreEqual(4, dr.GetInt32(0));
            Assert.AreEqual(5, dr.GetInt32(1));
            Assert.AreEqual(6, dr.GetInt32(2));

            Assert.IsFalse(dr.Read());
            Assert.IsFalse(dr.NextResult());
        }

        [TestMethod]
        public void EmptyLineTest()
        {
            var csv =
@"
1,2,3
4,5,6";

            var dr = OpenSimpleReader(csv);

            dr.Read();

            Assert.AreEqual(1, dr.GetInt32(0));
            Assert.AreEqual(2, dr.GetInt32(1));
            Assert.AreEqual(3, dr.GetInt32(2));

            dr.Read();

            Assert.AreEqual(4, dr.GetInt32(0));
            Assert.AreEqual(5, dr.GetInt32(1));
            Assert.AreEqual(6, dr.GetInt32(2));

            Assert.IsFalse(dr.Read());
            Assert.IsFalse(dr.NextResult());
        }

        [TestMethod]
        public void CommentedLineTest()
        {
            var csv =
@"1,2,3
#comment
4,5,6";

            var dr = OpenSimpleReader(csv);

            dr.Read();

            Assert.AreEqual(1, dr.GetInt32(0));
            Assert.AreEqual(2, dr.GetInt32(1));
            Assert.AreEqual(3, dr.GetInt32(2));

            dr.Read();

            Assert.AreEqual(4, dr.GetInt32(0));
            Assert.AreEqual(5, dr.GetInt32(1));
            Assert.AreEqual(6, dr.GetInt32(2));

            Assert.IsFalse(dr.Read());
            Assert.IsFalse(dr.NextResult());
        }

        [TestMethod]
        public void AutoDetectColumnsTest()
        {
            var csv =
@"1, 16000, 12345678, 12345678901234, 123.45, 1.1234567e5, 0.123456789012e-12,text";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.ColumnNamesInFirstLine = false;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = true;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            dr.Read();

            int o = 0;

            Assert.AreEqual(1, dr.GetInt64(o++));
            Assert.AreEqual(1, dr.GetInt32(o++));
            Assert.AreEqual(16000, dr.GetInt32(o++));
            Assert.AreEqual(12345678, dr.GetInt32(o++));
            Assert.AreEqual(12345678901234, dr.GetInt64(o++));
            Assert.AreEqual(123.45, dr.GetDouble(o++));
            Assert.AreEqual(112345.67, dr.GetDouble(o++));
            Assert.AreEqual(1.23456789012e-13, dr.GetDouble(o++));
            Assert.AreEqual("text", dr.GetString(o++));

            Assert.AreEqual(o, dr.FieldCount);
        }

        [TestMethod]
        public void AutoDetectColumnsTest2()
        {
            var csv =
@"#Spectral_Value,Flux_Value,Flux_Accuracy_StatErrLow,Flux_Accuracy_StatErrHigh,Flux_Accuracy_Quality
3801.01864554767,8.89645957946777,0,0,16777216
3801.89396320561,8.88807010650635,0,0,16777216
3802.7694824361,8.87969970703125,0,0,16777216
3803.64520328556,8.87135982513428,0,0,16777216
3804.5211258004,8.8630199432373,0,0,16777216
3805.39725002709,8.85470962524414,0,0,16777216
3806.27357601206,8.84642028808594,0,0,16777216
3807.15010380177,8.83813953399658,0,0,16777216
3808.02683344271,8.82987976074219,0,0,16777216
3808.90376498135,8.82164001464844,0,0,218103808
3809.78089846419,8.81342029571533,0,0,83886080
3810.65823393773,8.80521011352539,0,0,83886080
3811.53577144849,8.79703044891357,0,0,83886080
3812.41351104299,8.78886032104492,0,0,83886080
3813.29145276777,8.78069972991943,0,0,83886080
3814.16959666939,8.77256965637207,0,0,16777216
3815.04794279438,8.76445007324219,0,0,16777216
3815.92649118934,8.75636005401611,4.35990858078003,4.35990858078003,0
3816.80524190082,13.5115003585815,4.66361904144287,4.66361904144287,0
3817.68419497544,21.9514007568359,4.84443044662476,4.84443044662476,0
3818.56335045978,7.7454400062561,4.51452302932739,4.51452302932739,0
3819.44270840046,6.19890022277832,4.03657913208008,4.03657913208008,0
3820.32226884411,7.24296998977661,3.98828530311584,3.98828530311584,0
3821.20203183735,3.41108989715576,3.92595434188843,3.92595434188843,0
3822.08199742682,1.58747005462646,3.92526769638062,3.92526769638062,0
3822.96216565919,4.89786005020142,4.08808422088623,4.08808422088623,0
3823.84253658113,9.50407981872559,4.15539884567261,4.15539884567261,0
3824.72311023929,11.3456001281738,4.22903490066528,4.22903490066528,0
3825.60388668038,10.3824996948242,4.26791143417358,4.26791143417358,0
3826.48486595109,4.91490983963013,4.14181470870972,4.14181470870972,0
3827.36604809814,7.33786010742188,4.22164583206177,4.22164583206177,0
3828.24743316822,7.24986982345581,4.26109457015991,4.26109457015991,0
3829.1290212081,11.4675998687744,4.31576061248779,4.31576061248779,0
3830.01081226448,13.0429000854492,4.32072496414185,4.32072496414185,0
3830.89280638415,8.6926097869873,4.28437042236328,4.28437042236328,0
3831.77500361384,12.6611995697021,4.30809211730957,4.30809211730957,0
3832.65740400034,13.3259000778198,4.25758266448975,4.25758266448975,0
3833.54000759044,6.77347993850708,4.10115146636963,4.10115146636963,0";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.ColumnNamesInFirstLine = true;
            f.Culture = System.Globalization.CultureInfo.InvariantCulture;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = true;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            dr.Read();

            int o = 0;

            Assert.AreEqual(1, dr.GetInt64(o++));
            Assert.AreEqual(3801.01864554767, dr.GetDouble(o++));
            Assert.AreEqual(8.89645957946777, dr.GetDouble(o++));
            Assert.AreEqual(0.0, dr.GetDouble(o++));
            Assert.AreEqual(0.0, dr.GetDouble(o++));
            Assert.AreEqual(16777216, dr.GetInt32(o++));

            Assert.AreEqual(o, dr.FieldCount);
        }

        [TestMethod]
        public void AutoDetectColumnWithSkipLinesTest()
        {
            var csv =
@"
testline
1, 16000, 12345678, 12345678901234, 123.45, 1.1234567e5, 0.123456789012e-12,text";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.SkipLinesCount = 2;
            f.ColumnNamesInFirstLine = false;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = true;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            dr.Read();

            int o = 0;

            Assert.AreEqual(1, dr.GetInt64(o++));
            Assert.AreEqual(1, dr.GetInt32(o++));
            Assert.AreEqual(16000, dr.GetInt32(o++));
            Assert.AreEqual(12345678, dr.GetInt32(o++));
            Assert.AreEqual(12345678901234, dr.GetInt64(o++));
            Assert.AreEqual(123.45, dr.GetDouble(o++));
            Assert.AreEqual(112345.67, dr.GetDouble(o++));
            Assert.AreEqual(1.23456789012e-13, dr.GetDouble(o++));
            Assert.AreEqual("text", dr.GetString(o++));

            Assert.AreEqual(o, dr.FieldCount);
        }

        [TestMethod]
        public void DetectColumnNamesTest()
        {
            var csv =
@"#first, second, third, fourth
1,2,3,4";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.ColumnNamesInFirstLine = true;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = true;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            Assert.AreEqual("ID", dr.GetName(0));
            Assert.AreEqual("first", dr.GetName(1));
            Assert.AreEqual("second", dr.GetName(2));
            Assert.AreEqual("third", dr.GetName(3));
            Assert.AreEqual("fourth", dr.GetName(4));

            dr.Read();

            Assert.AreEqual(1, dr["first"]);
        }

        /// <summary>
        /// Detect column names from the first line and make
        /// sure the line is skipped. No hashmark in the first
        /// line, because it's skipped anyway.
        /// </summary>
        [TestMethod]
        public void DetectColumnNamesNohashTest()
        {
            var csv =
@"first, second, third, fourth
1,2,3,4";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.ColumnNamesInFirstLine = true;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = true;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            Assert.AreEqual("ID", dr.GetName(0));
            Assert.AreEqual("first", dr.GetName(1));
            Assert.AreEqual("second", dr.GetName(2));
            Assert.AreEqual("third", dr.GetName(3));
            Assert.AreEqual("fourth", dr.GetName(4));

            dr.Read();

            Assert.AreEqual(1, dr["first"]);
        }

        [TestMethod] 
        public void DetectAllTypesTest()
        {
            var csv =
@"#BigIntColumn,NumericColumn,BitColumn,SmallIntColumn,DecimalColumn,SmallMoneyColumn,IntColumn,TinyIntColumn,MoneyColumn,FloatColumn,RealColumn,DateColumn,DateTimeOffsetColumn,DateTime2Column,SmallDateTimeColumn,DateTimeColumn,TimeColumn,CharColumn,VarCharColumn,VarCharMaxColumn,TextColumn,NCharColumn,NVarCharColumn,NVarCharMaxColumn,NTextColumn,BinaryColumn,VarBinaryColumn,VarBinaryMaxColumn,ImageColumn,TimeStampColumn,UniqueIdentifierColumn
1234567890123,123,True,12345,1235,123456.8900,123456789,123,1234567.8900,123.456789,1.234568,01/01/2014 00:00:00,01/01/2014 11:59:59 +01:00,01/01/2014 11:59:59,01/01/2014 12:00:00,01/01/2014 11:59:59,11:59:59.1230000,""0123456789"",""012345"",""1234567890"",""1234567890"",""áíűőüöúóé "",""áíűőüöúóé"",""áíűőüöúóé"",""áíűőüöúóé"",0x1234567890,0x12345,0x1234567890,0x1234567890,0x0000007D2,a4466a24-5140-4efb-bb7d-0e4b4f34a75e
";

            var f = new DelimitedTextDataFile(new StringReader(csv));
            f.ColumnNamesInFirstLine = true;
            f.AutoDetectColumns = true;
            f.AutoDetectColumnsCount = 100;
            f.GenerateIdentityColumn = false;

            var cmd = new FileCommand(f);
            var dr = cmd.ExecuteReader();

            Assert.AreEqual(typeof(Int64), dr.GetFieldType(0));

            dr.Read();
        }


        // TODO: test nulls, string nulls, empty strings
        // multi-line strings
        // test with actual files

        #endregion
        #region Writer tests

        [TestMethod]
        public void SimpleWriterTest()
        {
            var w = new StringWriter();

            using (var cn = IOTestDataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                {
                    cmd.CommandText = "SELECT SampleData.* FROM SampleData";

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                    {
                        var csv = new DelimitedTextDataFile(w);
                        csv.WriteFromDataReader(dr);
                    }
                }
            }

            Assert.AreEqual(
@"#float,double,decimal,nvarchar(50),bigint,int,tinyint,smallint,bit,ntext,char,datetime,guid
1.234568,1.23456789,1.2346,""this is text"",123456789,123456,123,12345,True,""this is unicode text ő"",""A"",08/17/2012 00:00:00,68652251-c9e4-4630-80be-88b96d3258ce
",
                w.ToString());

        }

        [TestMethod]
        public void EmptyTableTest()
        {
            var w = new StringWriter();

            using (var cn = IOTestDataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                {
                    cmd.CommandText = "SELECT EmptyTable.* FROM EmptyTable";

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                    {
                        var csv = new DelimitedTextDataFile(w);
                        csv.WriteFromDataReader(dr);
                    }
                }
            }

            Assert.AreEqual(
@"#float,double,decimal,nvarchar(50),bigint,int,tinyint,smallint,bit,ntext,char,datetime,guid
",
                w.ToString());
        }

        [TestMethod]
        public void SimpleWriterAllTypesTest()
        {
            var w = new StringWriter();

            using (var cn = IOTestDataset.OpenConnection())
            {
                using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                {
                    cmd.CommandText = "SELECT * FROM SampleData_AllTypes";

                    using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                    {
                        var csv = new DelimitedTextDataFile(w);
                        csv.WriteFromDataReader(dr);
                    }
                }
            }

            // TODO: make sure timestamps column doesn't change

            Assert.IsTrue(
                w.ToString().StartsWith(
@"#BigIntColumn,NumericColumn,BitColumn,SmallIntColumn,DecimalColumn,SmallMoneyColumn,IntColumn,TinyIntColumn,MoneyColumn,FloatColumn,RealColumn,DateColumn,DateTimeOffsetColumn,DateTime2Column,SmallDateTimeColumn,DateTimeColumn,TimeColumn,CharColumn,VarCharColumn,VarCharMaxColumn,TextColumn,NCharColumn,NVarCharColumn,NVarCharMaxColumn,NTextColumn,BinaryColumn,VarBinaryColumn,VarBinaryMaxColumn,ImageColumn,TimeStampColumn,UniqueIdentifierColumn
1234567890,123,True,12345,1235,123456.8900,123456789,123,1234567.8900,123.456789,1.234568,01/01/2014 00:00:00,01/01/2014 11:59:59 +01:00,01/01/2014 11:59:59,01/01/2014 12:00:00,01/01/2014 11:59:59,11:59:59.1230000,""0123456789"",""012345"",""1234567890"",""1234567890"",""áíűőüöúóé "",""áíűőüöúóé"",""áíűőüöúóé"",""áíűőüöúóé"",0x1234567890,0x12345,0x1234567890,0x1234567890,"));

        }

        [TestMethod]
        public void CompressedWriterTest()
        {
            var uri = GetTestUniqueFileUri(".csv.gz");

            using (var csv = new DelimitedTextDataFile(uri, DataFileMode.Write))
            {
                using (var cn = IOTestDataset.OpenConnection())
                {
                    using (var cmd = new SmartCommand(IOTestDataset, cn.CreateCommand()))
                    {
                        cmd.CommandText = "SELECT SampleData.* FROM SampleData";

                        using (var dr = cmd.ExecuteReaderAsync(CommandBehavior.Default, CancellationToken.None).Result)
                        {
                            csv.WriteFromDataReader(dr);
                        }
                    }
                }
            }

            Assert.IsTrue(File.Exists(uri.ToString()));
            File.Delete(uri.ToString());
        }

        #endregion
    }
}
