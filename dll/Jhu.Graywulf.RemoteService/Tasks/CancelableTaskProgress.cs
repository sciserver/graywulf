using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Jhu.Graywulf.ServiceModel;

namespace Jhu.Graywulf.Tasks
{
    [Serializable]
    public class CancelableTaskProgress
    {
        private int currentValue;
        private int maximumValue;

        public int CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
        }

        public int MaximumValue
        {
            get { return maximumValue; }
            set { maximumValue = value; }
        }

        public double Percent
        {
            get { return (double)currentValue / maximumValue; }
        }

        public CancelableTaskProgress()
        {
            InitializeMembers(new StreamingContext());
        }

        public CancelableTaskProgress(int maximumValue)
        {
            InitializeMembers(new StreamingContext());

            this.maximumValue = maximumValue;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.currentValue = 0;
            this.maximumValue = 100;
        }

        private void CopyMembers(CancelableTaskProgress old)
        {
            this.currentValue = old.currentValue;
            this.maximumValue = old.maximumValue;
        }
    }
}
