using System;
using System.Linq;
using System.Resources;

using TeredoTool.Properties;

namespace TeredoTool
{
    static class Manager
    {

        static ResourceManager resources = Resources.ResourceManager;
        static ProcessLauncher processLauncher = new ProcessLauncher();

        public static void Run()
        {
            //Check IPHelper service
            CheckIPHelperService();

            //Teredo state
            CheckTeredoState();

            //Check TCP/IP v6 protocol
            ReEnableIpV6OnAllInterfaces();

            //TODO: Enable Teredo in the registry
            RegistryQuery();
        }


        /// <summary>
        /// Restart IPHelper service and set auto start
        /// </summary>
        private static void CheckIPHelperService()
        {
            processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("IPHELPER_STOP"));
            processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("IPHELPER_START"));
            processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("IPHELPER_SET_AUTO"));
        }


        /// <summary>
        /// Check and fix teredo state
        /// </summary>
        private static void CheckTeredoState()
        {
            String result = processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("TEREDO_STATE_SHOW"));
            String[] teredoStateResult = result.Split('\n').Where(val => val.Contains(":")).ToArray(); // [type, server name, client refresh interval, client port, state, error]

            for (int i = 0; i < teredoStateResult.Length; i++)
            {
                teredoStateResult[i] = teredoStateResult[i].Split(':')[1].Trim();
            }

            if (teredoStateResult[0].Equals("disabled"))
            {
                new ProcessLauncher().Start(resources.GetString("CMD_START"), resources.GetString("TEREDO_STATE_TYPE_RESET"));
            }

            if (teredoStateResult[1].Contains("ipv6.microsoft.com"))
            {
                new ProcessLauncher().Start(resources.GetString("CMD_START"), resources.GetString("TEREDO_STATE_SERVERNAME_RESET"));
            }
        }


        /// <summary>
        /// Re-enable IpV6 on all interfaces
        /// </summary>
        private static void ReEnableIpV6OnAllInterfaces()
        {
            processLauncher.Start(resources.GetString("POWERSHELL_START"), resources.GetString("POWERSHELL_ENABLE_IPV6_ALL_INTERFACES"));
        }

        /// <summary>
        /// Check if Teredo is disabled in the Windows Registry
        /// </summary>
        private static void RegistryQuery()
        {
            try
            {
                if (processLauncher != null)
                {
                    string output = processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("TEREDO_STATE_REGQUERY_CMD"));

                    if (!string.IsNullOrEmpty(output) && output.Contains("DisabledComponents ") && output.Contains("0x8e"))
                    {
                        processLauncher.Start(resources.GetString("CMD_START"), resources.GetString("TEREDO_REGQUERY_ADD_CMD"));
                    }
                }
            }
            catch (System.ArgumentNullException)
            {
                throw;
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}
