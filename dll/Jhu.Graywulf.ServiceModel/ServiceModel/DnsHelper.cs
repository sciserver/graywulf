using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;

namespace Jhu.Graywulf.ServiceModel
{
    public static class DnsHelper
    {
        private static readonly string localhost;
        private static readonly string localhostName;
        private static readonly string localhostFqdn;
        private static readonly HashSet<string> localhostIPs;

        public static string Localhost
        {
            get { return localhost; }
        }

        public static string LocalhostName
        {
            get { return localhostName; }
        }

        public static string LocalhostFqdn
        {
            get { return localhostFqdn; }
        }

        public static IEnumerable<string> LocalhostIPs
        {
            get { return localhostIPs; }
        }

        static DnsHelper()
        {
            localhost = "localhost";
            localhostName = GetHostName();
            localhostFqdn = GetFullyQualifiedDnsName();
            localhostIPs = GetIPs();
        }

        /// <summary>
        /// Returns the host name of the machine.
        /// </summary>
        /// <returns></returns>
        public static string GetHostName()
        {
            var ipprop = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            return ipprop.HostName;
        }

        private static HashSet<string> GetIPs()
        {
            var res = new HashSet<string>();
            res.Add("127.0.0.1");

            var query = "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'";
            var moSearch = new ManagementObjectSearcher(query);
            var moCollection = moSearch.Get();

            // Every record in this collection is a network interface
            foreach (ManagementObject mo in moCollection)
            {
                // IPAddresses, probably have more than one value
                string[] addresses = (string[])mo["IPAddress"];

                for (int i = 0; i < addresses.Length; i++)
                {
                    if (!res.Contains(addresses[i]))
                    {
                        res.Add(addresses[i]);
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Returns the DNS name of the current machine.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This function should return a fully and well qualified domain name,
        /// otherwise authentication wont work. To set up DNS names correctly on
        /// machines outside a windows domain, the primary DNS suffix must be set
        /// in the computer name configurations. ipconfig /all should give the
        /// currently set suffix.
        /// </remarks>
        public static string GetFullyQualifiedDnsName()
        {
            var ipprop = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
            string fqdn;

            try
            {
                fqdn = String.Format("{0}.{1}", ipprop.HostName, ipprop.DomainName);
            }
            catch (Exception)
            {
                fqdn = ipprop.HostName;
            }

            return fqdn;
        }

        /// <summary>
        /// Returns the DNS name of any host identified by its host name.
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public static string GetFullyQualifiedDnsName(string host)
        {
            // If host is localhost, simply replace it with the known host name,
            // otherwise windows authentication will not work within the local
            // machine

            // TODO: reverse lookup to get FQDN, it fails on current GW config at JHU!

            string name;

            if (StringComparer.InvariantCultureIgnoreCase.Compare(host, "localhost") == 0)
            {
                name = localhostFqdn;
            }
            else if (host.IndexOf('.') > -1)
            {
                // Assume it's FQDN
                name = host;
            }
            else
            {
                name = System.Net.Dns.GetHostEntry(host).HostName;
            }

            return name;
        }

        /// <summary>
        /// Returns true if the host is the local host
        /// </summary>
        /// <param name="fdqn"></param>
        /// <returns></returns>
        public static bool IsLocalhost(string fqdn)
        {
            var comparer = StringComparer.InvariantCultureIgnoreCase;

            if (comparer.Compare(localhost, fqdn) == 0)
            {
                return true;
            }
            else if (comparer.Compare(localhostFqdn, fqdn) == 0)
            {
                return true;
            }
            else if (fqdn.IndexOf('.') > -1 && comparer.Compare(localhostName, fqdn) == 0)
            {
                // localhostName might not be a fully qualified domain name, hence we check
                // for the dot above
                return true;
            }
            else if (localhostIPs.Contains(fqdn))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
