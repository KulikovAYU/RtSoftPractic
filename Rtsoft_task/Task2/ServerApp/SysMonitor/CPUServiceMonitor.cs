namespace SysMonitor
{
    //    //https://weblog.west-wind.com/posts/2014/Sep/27/Capturing-Performance-Counter-Data-for-a-Process-by-Process-Id
    //    public class CPUServiceMonitor
    //    {
    //        public static void Calc(string serviceName)
    //        {
    //#if DEBUG
    //            var reqProcess = Process.GetCurrentProcess();
    //#else
    //            var reqProcess = Process.GetProcessesByName(serviceName).FirstOrDefault();
    //#endif

    //            serviceName = reqProcess.ProcessName;
    //            if (reqProcess == null)
    //                return;

    //            PerformanceCounter process_cpu = new PerformanceCounter("Process", "% Processor Time", serviceName);
    //            // Initialize to start capturing
    //            process_cpu.NextValue();


    //            TimeSpan ts = TimeSpan.ParseExact("00:00:30,512",
    //                                  @"hh\:mm\:ss\,fff",
    //                                  CultureInfo.InvariantCulture);

    //            Timer tmr = new Timer(ts.TotalMilliseconds) { Enabled = true };
    //            tmr.Start();
    //            tmr.Elapsed += (s, args) => { tmr.Enabled = false; };
    //            while (tmr.Enabled) 
    //            {
    //                float cpu = process_cpu.NextValue() / Environment.ProcessorCount; ;
    //                Debug.WriteLine($"CPU usage {cpu }");

    //            }
    //        }
    //    }
}
