using System;
using System.Diagnostics;
using System.Text;

namespace TeredoTool
{
    internal class ProcessLauncher
    {
        public string Start(string filename, string arguments)
        {
            Process process = this.BuildProcessInfo(filename, arguments);
            string output = this.RunProcess(process);
            return output;
        }

        private Process BuildProcessInfo(string filename, string arguments)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,

                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    FileName = filename
                };
                if (!string.IsNullOrEmpty(arguments))
                    startInfo.Arguments = "/C " + arguments;

                Process process = new Process {
                    StartInfo = startInfo
                };

                return process;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string RunProcess(Process process)
        {
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) =>
            stdOutput.AppendLine(args.Data);

            string stdError = null;

            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e) { }


            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(Format(process.StartInfo.FileName, process.StartInfo.Arguments) + " finished with exit code = "
                    + process.ExitCode + ": " + message);
            }
        }

        private static string Format(string filename, string arguments)
        {
            return "'" + filename +
                ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                "'";
        }
    }
}
