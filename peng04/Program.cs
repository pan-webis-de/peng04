using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using peng04.experiments;

namespace peng04
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConfigFileReader().LoadAndRunExperiment(args);

            Console.WriteLine("---------------------- Shutdown [100%]");
            Console.WriteLine("Press [ENTER] to shutdown fast");

            var wh = new AutoResetEvent(false);
            new Task(() =>
            {
                Console.ReadLine();
                wh.Set();
            }).Start();

            int i = 10;
            new Task(() =>
            {
                while (i >= 0)
                {
                    i--;
                    Task.Delay(1000).Wait();
                    Console.WriteLine("{0} seconds left", i);
                }
            }).Start();

            wh.WaitOne(TimeSpan.FromSeconds(10));
        }
    }
}