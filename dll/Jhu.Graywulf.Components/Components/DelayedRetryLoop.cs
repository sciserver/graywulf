using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Implements functionality to help retry on error logic
    /// with delays between retries
    /// </summary>
    public class DelayedRetryLoop
    {
        private int initialDelay;
        private int maxDelay;
        private double delayMultiplier;
        private int maxRetries;

        private int currentDelay;
        private int retries;

        #region Properties

        public int InitialDelay
        {
            get { return initialDelay; }
            set { initialDelay = value; }
        }

        public int MaxDelay
        {
            get { return maxDelay; }
            set { maxDelay = value; }
        }

        public double DelayMultiplier
        {
            get { return delayMultiplier; }
            set { delayMultiplier = value; }
        }

        public int MaxRetries
        {
            get { return maxRetries; }
            set { maxRetries = value; }
        }

        public int CurrentDelay
        {
            get { return currentDelay; }
        }

        public int Retries
        {
            get { return retries; }
        }

        #endregion
        #region Constructors and initializers

        public DelayedRetryLoop()
        {
            InitializeMembers();
        }

        public DelayedRetryLoop(int maxRetries)
        {
            InitializeMembers();

            this.maxRetries = maxRetries;
        }

        private void InitializeMembers()
        {
            this.initialDelay = 1000;       // 1 sec
            this.maxDelay = 10000;          // 10 sec
            this.delayMultiplier = 2;
            this.maxRetries = 5;
        }

        #endregion

        private bool IsRetryPossible()
        {
            retries++;

            if (maxRetries == -1 || retries < maxRetries)
            {
                Thread.Sleep(currentDelay);
                currentDelay = Math.Min(maxDelay, (int)(delayMultiplier * currentDelay));

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Execute(Action tryAction)
        {
            Execute(tryAction, null, null);
        }

        public void Execute(Action tryAction, Action<Exception> catchAction)
        {
            Execute(tryAction, catchAction, null);
        }

        public void Execute(Action tryAction, Action<Exception> catchAction, Action finallyAction)
        {
            this.currentDelay = initialDelay;
            this.retries = 0;

            while (true)
            {
                try
                {
                    tryAction();
                    
                    // All is good, so quit loop;
                    break;
                }
                catch (Exception ex)
                {
                    if (catchAction != null)
                    {
                        catchAction(ex);
                    }

                    if (!IsRetryPossible())
                    {
                        // All attempts failed, forward exception to caller
                        throw;
                    }
                }
                finally
                {
                    if (finallyAction != null)
                    {
                        finallyAction();
                    }
                }
            }
        }
    }
}
