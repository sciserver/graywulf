using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Types;

namespace Jhu.Graywulf.Types.Test
{
    [TestClass]
    public class SwapBitConverterTest
    {
        protected string ByteArrayToString(byte[] ba)
        {
            return System.BitConverter.ToString(ba);
        }

        [TestMethod]
        public void ToBooleanTest()
        {
            var bc = new SwapBitConverter();

            Assert.IsTrue(bc.ToBoolean(new byte[] { 1 }, 0));
            Assert.IsTrue(bc.ToBoolean(new byte[] { 8 }, 0));
            Assert.IsFalse(bc.ToBoolean(new byte[] { 0 }, 0));
        }

        [TestMethod]
        public void ToSByteTest()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(0, bc.ToSByte(new byte[] { 0 }, 0));
            Assert.AreEqual(1, bc.ToSByte(new byte[] { 1 }, 0));
            Assert.AreEqual(127, bc.ToSByte(new byte[] { 127 }, 0));
            Assert.AreEqual(-128, bc.ToSByte(new byte[] { 128 }, 0));
            Assert.AreEqual(-1, bc.ToSByte(new byte[] { 255 }, 0));
        }

        [TestMethod]
        public void ToCharTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ToInt16Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1, bc.ToInt16(new byte[] { 0, 1 }, 0));
            Assert.AreEqual(256, bc.ToInt16(new byte[] { 1, 0 }, 0));
            Assert.AreEqual(257, bc.ToInt16(new byte[] { 1, 1 }, 0));
            Assert.AreEqual(-1, bc.ToInt16(new byte[] { 255, 255 }, 0));
            Assert.AreEqual(-32768, bc.ToInt16(new byte[] { 128, 0 }, 0));
            Assert.AreEqual(32767, bc.ToInt16(new byte[] { 127, 255 }, 0));
        }

        [TestMethod]
        public void ToUInt16Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1U, bc.ToUInt16(new byte[] { 0, 1 }, 0));
            Assert.AreEqual(256U, bc.ToUInt16(new byte[] { 1, 0 }, 0));
            Assert.AreEqual(257U, bc.ToUInt16(new byte[] { 1, 1 }, 0));
            Assert.AreEqual(65535U, bc.ToUInt16(new byte[] { 255, 255 }, 0));
        }

        [TestMethod]
        public void ToInt32Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1, bc.ToInt32(new byte[] { 0, 0, 0, 1 }, 0));
            Assert.AreEqual(65536, bc.ToInt32(new byte[] { 0, 1, 0, 0 }, 0));
            Assert.AreEqual(-1, bc.ToInt32(new byte[] { 255, 255, 255, 255 }, 0));
        }

        [TestMethod]
        public void ToUInt32Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1U, bc.ToUInt32(new byte[] { 0, 0, 0, 1 }, 0));
            Assert.AreEqual(65536U, bc.ToUInt32(new byte[] { 0, 1, 0, 0 }, 0));
            Assert.AreEqual(4294967295U, bc.ToUInt32(new byte[] { 255, 255, 255, 255 }, 0));
        }

        [TestMethod]
        public void ToInt64Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1L, bc.ToInt64(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, 0));
            Assert.AreEqual(65536L, bc.ToInt64(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 }, 0));
            Assert.AreEqual(4294967296L, bc.ToInt64(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 }, 0));
            Assert.AreEqual(281474976710656L, bc.ToInt64(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 }, 0));
            Assert.AreEqual(-1L, bc.ToInt64(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 0));
        }

        [TestMethod]
        public void ToUInt64Test()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1UL, bc.ToUInt64(new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }, 0));
            Assert.AreEqual(65536UL, bc.ToUInt64(new byte[] { 0, 0, 0, 0, 0, 1, 0, 0 }, 0));
            Assert.AreEqual(4294967296UL, bc.ToUInt64(new byte[] { 0, 0, 0, 1, 0, 0, 0, 0 }, 0));
            Assert.AreEqual(281474976710656UL, bc.ToUInt64(new byte[] { 0, 1, 0, 0, 0, 0, 0, 0 }, 0));
            Assert.AreEqual(18446744073709551615UL, bc.ToUInt64(new byte[] { 255, 255, 255, 255, 255, 255, 255, 255 }, 0));
        }

        [TestMethod]
        public void ToSingleTest()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1.0f, bc.ToSingle(new byte[] { 63, 128, 0, 0 }, 0));
            Assert.AreEqual(-1.0f, bc.ToSingle(new byte[] { 191, 128, 0, 0 }, 0));
        }

        [TestMethod]
        public void ToDoubleTest()
        {
            var bc = new SwapBitConverter();

            Assert.AreEqual(1.0, bc.ToDouble(new byte[] { 63, 240, 0, 0, 0, 0, 0, 0 }, 0));
            Assert.AreEqual(-1.0, bc.ToDouble(new byte[] { 191, 240, 0, 0, 0, 0, 0, 0 }, 0));
        }

        [TestMethod]
        public void ToSingleComplexTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ToDoubleComplexTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GetBytesFromBooleanTest()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[1];

            bc.GetBytes(true, bytes, 0);
            Assert.AreEqual("01", ByteArrayToString(bytes));

            bc.GetBytes(false, bytes, 0);
            Assert.AreEqual("00", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromSByteTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GetBytesFromCharTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GetBytesFromInt16Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[2];

            bc.GetBytes((Int16)0, bytes, 0);
            Assert.AreEqual("00-00", ByteArrayToString(bytes));

            bc.GetBytes((Int16)1, bytes, 0);
            Assert.AreEqual("00-01", ByteArrayToString(bytes));

            bc.GetBytes((Int16)(-1), bytes, 0);
            Assert.AreEqual("FF-FF", ByteArrayToString(bytes));

            bc.GetBytes(Int16.MinValue, bytes, 0);
            Assert.AreEqual("80-00", ByteArrayToString(bytes));

            bc.GetBytes(Int16.MaxValue, bytes, 0);
            Assert.AreEqual("7F-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromUInt16Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[2];

            bc.GetBytes((UInt16)0, bytes, 0);
            Assert.AreEqual("00-00", ByteArrayToString(bytes));

            bc.GetBytes((UInt16)1, bytes, 0);
            Assert.AreEqual("00-01", ByteArrayToString(bytes));

            bc.GetBytes(UInt16.MinValue, bytes, 0);
            Assert.AreEqual("00-00", ByteArrayToString(bytes));

            bc.GetBytes(UInt16.MaxValue, bytes, 0);
            Assert.AreEqual("FF-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromInt32Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[4];

            bc.GetBytes((Int32)0, bytes, 0);
            Assert.AreEqual("00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes((Int32)1, bytes, 0);
            Assert.AreEqual("00-00-00-01", ByteArrayToString(bytes));

            bc.GetBytes((Int32)(-1), bytes, 0);
            Assert.AreEqual("FF-FF-FF-FF", ByteArrayToString(bytes));

            bc.GetBytes(Int32.MinValue, bytes, 0);
            Assert.AreEqual("80-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(Int32.MaxValue, bytes, 0);
            Assert.AreEqual("7F-FF-FF-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromUInt32Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[4];

            bc.GetBytes((UInt32)0, bytes, 0);
            Assert.AreEqual("00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes((UInt32)1, bytes, 0);
            Assert.AreEqual("00-00-00-01", ByteArrayToString(bytes));

            bc.GetBytes(UInt32.MinValue, bytes, 0);
            Assert.AreEqual("00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(UInt32.MaxValue, bytes, 0);
            Assert.AreEqual("FF-FF-FF-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromInt64Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[8];

            bc.GetBytes((Int64)0, bytes, 0);
            Assert.AreEqual("00-00-00-00-00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes((Int64)1, bytes, 0);
            Assert.AreEqual("00-00-00-00-00-00-00-01", ByteArrayToString(bytes));

            bc.GetBytes((Int64)(-1), bytes, 0);
            Assert.AreEqual("FF-FF-FF-FF-FF-FF-FF-FF", ByteArrayToString(bytes));

            bc.GetBytes(Int64.MinValue, bytes, 0);
            Assert.AreEqual("80-00-00-00-00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(Int64.MaxValue, bytes, 0);
            Assert.AreEqual("7F-FF-FF-FF-FF-FF-FF-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromUInt64Test()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[8];

            bc.GetBytes(0UL, bytes, 0);
            Assert.AreEqual("00-00-00-00-00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(1UL, bytes, 0);
            Assert.AreEqual("00-00-00-00-00-00-00-01", ByteArrayToString(bytes));

            bc.GetBytes(UInt64.MinValue, bytes, 0);
            Assert.AreEqual("00-00-00-00-00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(UInt64.MaxValue, bytes, 0);
            Assert.AreEqual("FF-FF-FF-FF-FF-FF-FF-FF", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromSingleTest()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[4];

            bc.GetBytes(1.0f, bytes, 0);
            Assert.AreEqual("3F-80-00-00", ByteArrayToString(bytes));

            bc.GetBytes(-1.0f, bytes, 0);
            Assert.AreEqual("BF-80-00-00", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromDoubleTest()
        {
            var bc = new SwapBitConverter();
            var bytes = new byte[8];

            bc.GetBytes(1.0, bytes, 0);
            Assert.AreEqual("3F-F0-00-00-00-00-00-00", ByteArrayToString(bytes));

            bc.GetBytes(-1.0, bytes, 0);
            Assert.AreEqual("BF-F0-00-00-00-00-00-00", ByteArrayToString(bytes));
        }

        [TestMethod]
        public void GetBytesFromSingleComplexTest()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void GetBytesFromDoubleComplexTest()
        {
            Assert.Inconclusive();
        }
    }
}
