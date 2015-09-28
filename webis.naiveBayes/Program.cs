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
            if (!args.Any())
            {
                Console.WriteLine("you must specify a directory");
            }
            else if (!Directory.Exists(args[0]))
            {
                Console.WriteLine("Directory {0} not found", args[0]);
            }
            else
            {
                new TiraExperiment().Start(args[0], false);
            }

            Console.WriteLine("Press [ENTER] to continue ...");
            Console.ReadLine();
        }
    }
}