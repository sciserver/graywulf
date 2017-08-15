using System;
using System.Security.Principal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Logging
{
    public abstract class LogWriterTestBase
    {
        protected void WriteErrorTestHelper()
        {
            throw new Exception("Test exception");
        }

        protected abstract LogWriterBase CreateLogWriter(bool isasync);

        protected virtual IPrincipal CreateTestprincipal()
        {
            var identity = new GenericIdentity("test");
            var principal = new GenericPrincipal(identity, null);
            return principal;
        }

        protected Event CreateTestEvent()
        {
            var guid = new Guid("413738E0-1111-1111-2222-D86E6A1AD098");

            var e = new Logging.Event()
            {
                BookmarkGuid = Guid.NewGuid(),
                Client = "127.0.0.1",
                Server = Environment.MachineName,
                ContextGuid = Guid.Empty,
                DateTime = DateTime.UtcNow,
                ExecutionStatus = ExecutionStatus.Closed,
                JobGuid = guid,
                JobName = "job_name",
                Message = "This is a unit test.",
                Operation = "Jhu.Graywulf.Logging.SciServerLogWriterTest.WriteEventTest",
                Order = 0,
                Principal = CreateTestprincipal(),
                Request = "GET /test",
                SessionGuid = guid,
                Severity = EventSeverity.Operation,
                Source = EventSource.Test,
                TaskName = "test_task",
                UserName = "test",
                UserGuid = guid,
            };

            e.UserData.Add("test_key", "test_value");

            return e;
        }

        protected Event CreateTestExceptionEvent(Exception ex)
        {
            var guid = new Guid("413738E0-1111-1111-2222-D86E6A1AD098");

            var e = Logging.LoggingContext.Current.CreateEvent(EventSeverity.Error, EventSource.Test, null, null, ex, null);

            e.BookmarkGuid = Guid.NewGuid();
            e.Client = "127.0.0.1";
            e.Server = Environment.MachineName;
            e.ContextGuid = Guid.Empty;
            e.DateTime = DateTime.UtcNow;
            e.ExecutionStatus = ExecutionStatus.Closed;
            e.JobGuid = guid;
            e.JobName = "job_name";
            e.Order = 0;
            e.Principal = CreateTestprincipal();
            e.Request = "GET /test";
            e.SessionGuid = guid;
            e.Source = EventSource.Test;
            e.TaskName = "test_task";
            e.UserName = "test";
            e.UserGuid = guid;

            e.UserData.Add("test_key", "test_value");

            return e;
        }

        protected void WriteEventTestHelper(bool isasync)
        {
            var e = CreateTestEvent();
            var w = CreateLogWriter(isasync);

            w.Start();
            w.WriteEvent(e);
            w.Stop();
        }

        protected void WriteErrorTestHelper(bool isasync)
        {
            var w = CreateLogWriter(false);

            try
            {
                WriteErrorTestHelper();
            }
            catch (Exception ex)
            {
                var e = CreateTestExceptionEvent(ex);

                w.Start();
                w.WriteEvent(e);
                w.Stop();
            }
        }
    }
}
