using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.ServiceModel
{
    public interface IServiceProxy<out T> : IDisposable
    {
        bool IsInProcess { get; }
        bool IsFaulted { get; }
        T Value { get; }
        void Close();
    }

    /// <summary>
    /// Wraps a remote proxy to carry around the ChannelFactory and
    /// communication channel associated with it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ServiceProxy<T> : IServiceProxy<T>, IDisposable
    {
        #region Private member variables

        private ChannelFactory channelFactory;
        private T channel;
        private bool isInProcess;

        #endregion
        #region Properties

        public bool IsInProcess
        {
            get { return isInProcess; }
            internal set { isInProcess = value; }
        }

        public bool IsFaulted
        {
            get
            {
                if (isInProcess)
                {
                    return false;
                }
                else
                {
                    return ((IChannel)channel).State == CommunicationState.Faulted;
                }
            }
        }

        public T Value
        {
            get { return channel; }
        }

        #endregion
        #region Constructors and initializers

        public ServiceProxy(T channel)
        {
            this.channelFactory = null;
            this.channel = channel;
            this.isInProcess = true;
        }

        internal ServiceProxy(ChannelFactory<T> channelFactory, T channel)
        {
            this.channelFactory = channelFactory;
            this.channel = channel;
            this.isInProcess = false;
        }

        public void Dispose()
        {
            Close();

            if (channel != null)
            {
                channel = default(T);
            }

            if (channelFactory != null)
            {
                channelFactory = null;
            }
        }

        #endregion

        public void Close()
        {
            if (!isInProcess && channel != null && ((IChannel)channel).State == CommunicationState.Opened)
            {
                try
                {
                    ((IChannel)channel).Close();
                }
                catch (TimeoutException)
                {
                    ((IChannel)channel).Abort();
                }
                catch (CommunicationException)
                {
                    ((IChannel)channel).Abort();
                }
            }

            if (channelFactory != null && channelFactory.State == CommunicationState.Opened)
            {
                try
                {
                    channelFactory.Close();
                }
                catch (TimeoutException)
                {
                    channelFactory.Abort();
                }
                catch (CommunicationException)
                {
                    channelFactory.Abort();
                }
            }
        }
    }
}
