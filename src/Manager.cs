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

            //TODO: Check TCP/IP v6 protocol
            //TODO: Enable Teredo in the registry
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
    }
} 
