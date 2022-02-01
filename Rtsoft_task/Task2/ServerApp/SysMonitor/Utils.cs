using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SysMonitor
{
    public static class Utils
    {
        public static bool GetProcIdByServiceName(out int procId, string serviceName)
        {
            procId = 0;

            string getProcIdScript = $"systemctl show --property MainPID --value {serviceName}";
            if (ExecuteScript(out var sOutput, getProcIdScript))
            {
                sOutput = Regex.Replace(sOutput, @"\s+", "");

                return int.TryParse(sOutput, NumberStyles.AllowDecimalPoint,
                    NumberFormatInfo.InvariantInfo, out procId) && procId != 0;
            }

            return false;
        }

        public static bool GetCpuLoadingPercentage(out float cpuPercs, int procId)
        {
            cpuPercs = 0.0f;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Process.GetProcessById(processId: procId) == null)
                return false;

            //ps -p 22534 -o %cpu 
            string getCpuPercScript = $"ps -p {procId} -o %cpu";

            if (ExecuteScript(out var sOutput, getCpuPercScript))
            {
                sOutput = sOutput.Remove(0, 5);
                sOutput = sOutput.Substring(0, sOutput.IndexOf("\n", StringComparison.Ordinal));
                sOutput = Regex.Replace(sOutput, @"\s+", "");

                return float.TryParse(sOutput, NumberStyles.AllowDecimalPoint,
                    NumberFormatInfo.InvariantInfo, out cpuPercs);
            }

            return false;
        }

        public static bool GetCpuTemperature(out float cpuTemp)
        {
            cpuTemp = 0.0f;

            string filePath = "/sys/devices/virtual/thermal/thermal_zone0/temp";

            if (File.Exists(filePath))
            {
                var stringBuilder = new StringBuilder();

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    stringBuilder.Append(reader.ReadToEnd());
                    cpuTemp = float.Parse(stringBuilder.ToString());
                    cpuTemp /= 1000;
                    return true;
                }
            }

            return false;
        }

        public static bool ExecuteScript(out string sSresult, string sScript)
        {
            sSresult = "";

            try
            {
                using Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "/bin/bash",
                        Arguments = $"-c \"{sScript}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    }
                };
                var execRes = proc.Start();
                
                if (execRes)
                {

                    sSresult = proc.StandardOutput.ReadToEnd();
                    return proc.WaitForExit(10000);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
