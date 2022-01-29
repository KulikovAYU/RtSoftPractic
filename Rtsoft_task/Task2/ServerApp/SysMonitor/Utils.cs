using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SysMonitor
{
    class Utils
    {
        public static bool GetProcIdByServiceName(out int procId, string serviceName)
        {
            procId = -1;

            string getProcIdScript = $"systemctl show--property MainPID--value {serviceName}";
            string sOutput = "";
            if (ExecuteScript(out sOutput, getProcIdScript)) 
            {
                sOutput = Regex.Replace(sOutput, @"\s+", "");

                return int.TryParse(sOutput, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out procId);
            }

            return false;
        }

        public static bool GetCPULoadingPercentage(out float cpuPercs, int procId)
        {
            cpuPercs = 0.0f;
            //ps -p 22534 -o %cpu 
            string getCpuPercScript = $"- c \"ps -p {procId} -o %cpu\"";

            string sOutput = "";
            if (ExecuteScript(out sOutput, getCpuPercScript))
            {

                sOutput = sOutput.Remove(0, 5);
                sOutput = sOutput.Substring(0, sOutput.IndexOf("\n"));
                sOutput = Regex.Replace(sOutput, @"\s+", "");

                return float.TryParse(sOutput, System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out cpuPercs);
            }

            return false;
        }

        public static bool GetCPUTemperature(out float cpuTemp)
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
                    if (float.TryParse(stringBuilder.ToString(), System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.NumberFormatInfo.InvariantInfo, out cpuTemp))
                    {
                        cpuTemp /= 1000;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool ExecuteScript(out string sSresult, string sScript)
        {
            sSresult = "";

            using (Process proc = new Process(){
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{sScript}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                }})
            {
                var execRes = proc.Start();

                if (execRes)
                {
                    try
                    {
                        sSresult = proc.StandardOutput.ReadToEnd();
                        return proc.WaitForExit(10000);
                    }
                    catch (Exception)
                    {
                    }
                }
                return execRes;
            }
        }
    }
}
