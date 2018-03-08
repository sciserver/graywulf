using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Web.Services;
using Jhu.Graywulf.Test;
using Jhu.Graywulf.Scheduler;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Api.V1
{
    public class ApiTestBase : TestClassBase
    {
        protected ApiTestBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
        }

        protected void AuthenticateTestUser(RestClientSession session)
        {
            AuthenticateUser(session, "test");
        }

        protected void AuthenticateUser(RestClientSession session, string user)
        {
            var auth = new AuthRequest()
            {
                Credentials = new Credentials()
                {
                    Username = user,
                    Password = "almafa"
                }
            };

            var authuri = String.Format("http://{0}{1}/api/v1/auth.svc", Environment.MachineName, Jhu.Graywulf.Test.AppSettings.WebAuthPath);
            var client = session.CreateClient<IAuthService>(new Uri(authuri));

            client.Authenticate(auth);
        }

        protected IJobsService CreateJobServiceClient(RestClientSession session)
        {
            AuthenticateTestUser(session);
            var uri = String.Format("http://{0}{1}/api/v1/jobs.svc", Environment.MachineName, Jhu.Graywulf.Test.AppSettings.WebUIPath);
            var client = session.CreateClient<IJobsService>(new Uri(uri));
            return client;
        }

        protected virtual void ExportFileHelper(string dataset, string table, string uri, string mimeType, string comments)
        {
            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    using (var session = new RestClientSession())
                    {
                        var client = CreateJobServiceClient(session);

                        var job = new ExportJob()
                        {
                            Source = new SourceTable()
                            {
                                Dataset = dataset,
                                Table = table,
                            },
                            Uri = new Uri(uri),
                            Comments = comments,
                            FileFormat = new FileFormat()
                            {
                                MimeType = mimeType,
                            }
                        };

                        var request = new JobRequest()
                        {
                            ExportJob = job
                        };

                        var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                        // Try to get newly scheduled job
                        var nj = client.GetJob(response.ExportJob.Guid.ToString());
                        var guid = nj.ExportJob.Guid;

                        WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                        var ji = LoadJob(guid);
                        Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                    }
                }
            }
        }

        protected virtual void ImportFileHelper(string uri, bool generateIdentityColumn)
        {
            ImportFileHelper(uri, Registry.Constants.UserDbName, null, generateIdentityColumn);
        }

        protected virtual void ImportFileHelper(string uri, string dataset, string table, bool generateIdentityColumn)
        {
            using (SchedulerTester.Instance.GetToken())
            {
                PurgeTestJobs();

                SchedulerTester.Instance.EnsureRunning();

                using (RemoteServiceTester.Instance.GetToken())
                {
                    RemoteServiceTester.Instance.EnsureRunning();

                    using (var session = new RestClientSession())
                    {
                        var client = CreateJobServiceClient(session);

                        var job = new ImportJob()
                        {
                            Uri = new Uri(uri),
                            Comments = GetTestUniqueName(),
                            Destination = new DestinationTable()
                            {
                                Dataset = dataset,
                                Table = table,
                            },
                            /*Options = new ImportOptions()
                            {
                                GenerateIdentityColumn = generateIdentityColumn
                            }*/
                        };

                        var request = new JobRequest()
                        {
                            ImportJob = job
                        };

                        var response = client.SubmitJob(JobQueue.Quick.ToString(), request);

                        // Try to get newly scheduled job
                        var nj = client.GetJob(response.ImportJob.Guid.ToString());
                        var guid = nj.ImportJob.Guid;

                        WaitJobComplete(guid, TimeSpan.FromSeconds(10));

                        var ji = LoadJob(guid);
                        Assert.AreEqual(JobExecutionState.Completed, ji.JobExecutionStatus);
                    }
                }
            }
        }
    }
}
