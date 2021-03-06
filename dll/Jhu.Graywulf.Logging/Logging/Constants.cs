﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Logging
{
    public static class Constants
    {
        public const string ConfigSectionGroupName = "jhu.graywulf/logging";

        public const string UserDataEntityGuid = "EntityGuid";
        public const string UserDataEntityGuidFrom = "EntityGuidFrom";
        public const string UserDataEntityGuidTo = "EntityGuidTo";

        public const string ActivityRecordDataItemEvent = "Event";

        public const int DefaultLogWriterAsyncQueueSize = 100;
        public const int DefaultLogWriterAsyncTimeout = 10000; // ms
    }
}
