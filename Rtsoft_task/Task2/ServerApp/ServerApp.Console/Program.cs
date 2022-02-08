using ServerApp.Core;
using System;
using Autofac;
using ServerApp.Core.Interfaces;
using ServerApp.Core.Server;

namespace ServerApp.Cons
{
    class UI
    {
        public void Error(object sender, EventArgs args)
        {
            string message = ((MessageEventArgs)args).Message;
            ConsoleColor buffColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Write("[ERROR] \t");
            Console.WriteLine(message);

            Console.ForegroundColor = buffColor;
        }

        public void Print(object sender, EventArgs args)
        {
            string message = ((MessageEventArgs)args).Message;
            ConsoleColor buffColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;

            Console.Write("[INFO] \t");
            Console.WriteLine(message);

            Console.ForegroundColor = buffColor;
        }
    }

    class Logger
    {
        public void Error(object sender, EventArgs args)
        {
        }

        public void Print(object sender, EventArgs args)
        {
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            var coreContainer = ServerAppCoreEntryPointCfg.Configure();
            
            using var scope = coreContainer.BeginLifetimeScope();
            var evBus = scope.Resolve<IEventBus>();
            
            UI ui1 = new UI();
            evBus.ErrorHappened += ui1.Error;
            evBus.PrintHappened += ui1.Print;

            //just immitation
            Logger log1 = new Logger();
            evBus.ErrorHappened += log1.Error;
            evBus.PrintHappened += log1.Print;
                
            var serverApp = scope.Resolve<IServerApplication>();
                
            serverApp.Run();
                
            Console.ReadLine();
                
            serverApp.Stop();
        }
    }
}
