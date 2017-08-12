using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Check
{
    public class CheckRoutineStatus
    {
        private DateTime dateTime;
        private string message;
        private CheckResult result;
        private Exception error;

        public DateTime DateTime
        {
            get { return dateTime; }
            set { dateTime = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        public CheckResult Result
        {
            get { return result; }
            set { result = value; }
        }

        public Exception Error
        {
            get { return error; }
            set { error = value; }
        }

        public CheckRoutineStatus()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.dateTime = DateTime.UtcNow;
            this.message = null;
            this.result = CheckResult.Unknown;
            this.error = null;
        }
    }
}
