using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Jhu.Graywulf.SimpleRestClient;

namespace Jhu.Graywulf.Keystone
{
    [JsonObject(MemberSerialization.OptIn)]
    internal class ProjectRequest
    {
        [JsonProperty("project")]
        public Project Project { get; private set; }

        public static ProjectRequest Create(Project project)
        {
            return new ProjectRequest()
            {
                Project = project
            };
        }

        public static RestMessage<ProjectRequest> CreateMessage(Project project)
        {
            return new RestMessage<ProjectRequest>(Create(project));
        }
    }
}
