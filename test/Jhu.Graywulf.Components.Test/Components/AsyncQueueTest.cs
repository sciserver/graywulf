using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Components
{
    [TestClass]
    public class AsyncQueueTest
    {
        private int batchStartCalled;
        private int batchEndCalled;
        private int processingCalled;
        private int unhandledExceptionCalled;
        private List<int> output;

        private void AsyncQueue_BatchStart(object sender, EventArgs e)
        {
            batchStartCalled++;
        }


        private void AsyncQueue_BatchEnd(object sender, EventArgs e)
        {
            batchEndCalled++;
        }

        private void Queue_OnItemProcessing(object sender, AsyncQueueItemProcessingEventArgs<int> e)
        {
            output.Add(e.Item);

            processingCalled++;

            if (processingCalled > 5)
            {
                throw new Exception("Test exception");
            }
        }

        private void Queue_OnUnhandledException(object sender, AsyncQueueUnhandledExceptionEventArgs<int> e)
        {
            unhandledExceptionCalled++;
        }

        private AsyncQueue<int> CreateQueue()
        {
            batchStartCalled = 0;
            batchEndCalled = 0;
            processingCalled = 0;
            unhandledExceptionCalled = 0;
            output = new List<int>();

            var queue = new AsyncQueue<int>();

            queue.OnBatchStart += AsyncQueue_BatchStart;
            queue.OnBatchEnd += AsyncQueue_BatchEnd;
            queue.OnItemProcessing += Queue_OnItemProcessing;
            queue.OnUnhandledException += Queue_OnUnhandledException;

            return queue;
        }

        [TestMethod]
        public void CreateDisposeTest()
        {
            using (var queue = CreateQueue())
            {
            }

            Assert.AreEqual(0, batchStartCalled);
            Assert.AreEqual(0, batchEndCalled);
            Assert.AreEqual(0, processingCalled);
            Assert.AreEqual(0, unhandledExceptionCalled);
            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void StartStopTest()
        {
            using (var queue = CreateQueue())
            {
                queue.Start();
                queue.Stop();
            }

            Assert.AreEqual(0, batchStartCalled);
            Assert.AreEqual(0, batchEndCalled);
            Assert.AreEqual(0, processingCalled);
            Assert.AreEqual(0, unhandledExceptionCalled);
            Assert.AreEqual(0, output.Count);
        }

        [TestMethod]
        public void EnqueueTest()
        {
            using (var queue = CreateQueue())
            {
                queue.Start();
                queue.Enqueue(1);
                queue.Stop();
            }

            Assert.AreEqual(1, batchStartCalled);
            Assert.AreEqual(1, batchEndCalled);
            Assert.AreEqual(1, processingCalled);
            Assert.AreEqual(0, unhandledExceptionCalled);
            Assert.AreEqual(1, output.Count);
        }

        [TestMethod]
        public void HandledExceptionTest()
        {
            using (var queue = CreateQueue())
            {
                queue.Start();

                for (int i = 0; i < 10; i++)
                {
                    queue.Enqueue(i);
                }

                queue.Stop();
            }

            Assert.AreEqual(1, batchStartCalled);
            Assert.AreEqual(1, batchEndCalled);
            Assert.AreEqual(10, processingCalled);
            Assert.AreEqual(5, unhandledExceptionCalled);
            Assert.AreEqual(10, output.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void UnhandledExceptionTest()
        {
            using (var queue = CreateQueue())
            {
                queue.OnUnhandledException -= Queue_OnUnhandledException;
                queue.Start();

                for (int i = 0; i < 10; i++)
                {
                    queue.Enqueue(i);
                }

                queue.Stop();
            }
        }

        // TODO: test exceptions from batch start and end handlers
        // TODO: test abort logic
    }
}
