using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.experiments;

namespace webis.naiveBayes
{
    class Program
    {
        static void Main(string[] args)
        {
            new ConfigFileReader().LoadAndRunExperiment(args);

            Console.WriteLine("Press [ENTER] to continue ...");
            Console.ReadLine();
        }
    }
}