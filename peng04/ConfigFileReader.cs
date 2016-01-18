using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using peng04.experiments;
using peng04.logic;
using peng04.processing;

namespace peng04
{
    public class ConfigFileReader
    {
        public void LoadAndRunExperiment(string[] args)
        {
            Console.WriteLine("---------------------- Prepare for execution [Reading config]");
            CalculationConstants.SmoothingEpsilon = double.Parse(ConfigurationManager.AppSettings["smoothingEpsilon"]);
            Console.WriteLine("smoothEpsilon:   {0}", CalculationConstants.SmoothingEpsilon);

            var smoothing = ConfigurationManager.AppSettings["smoothing"];
            ISmoothingTechnique sm = null;
            if (smoothing == "absolute")
            {
                sm = new AbsoluteSmoothing();
                Console.WriteLine("smoothing:       absolute smoothing");
            }
            else if (smoothing == "linear")
            {
                sm = new LinearSmoothing();
                Console.WriteLine("smoothing:       linear smoothing");
            }

            if(ConfigurationManager.AppSettings.AllKeys.Contains("preInitSmoothingParameter"))
            {
                var param = double.Parse(ConfigurationManager.AppSettings["preInitSmoothingParameter"]);
                sm.Init(param);
                Console.WriteLine("smoothing:       pre-init with param {0}", param);
            }
            else
            {
                Console.WriteLine("smoothing:       no pre-init, getting parameters from data");
            }

            var processor = ConfigurationManager.AppSettings["processor"];
            ILanguageProcessor lp = null;
            if (processor == "word")
            {
                lp = new WordLevelProcessor();
                Console.WriteLine("processor:       word level processor");
            }
            else if(processor == "char")
            {
                lp = new CharacterLevelProcessor();
                Console.WriteLine("processor:       character level processor");
            }

            var experiment = ConfigurationManager.AppSettings["experiment"];
            if (experiment == "tira")
            {
                var nGramSize = int.Parse(ConfigurationManager.AppSettings["nGramSize"]);
                Console.WriteLine("n-gram size:     {0}", nGramSize);

                if (args.Any() && args[0] == "test")
                {
                    // this is the test case
                    Console.WriteLine("setting:         test case");
                    Console.WriteLine("---------------------- Starting execution [TIRA Experiment]");

                    new TiraExperiment().Start(ConfigurationManager.AppSettings["tiraTestFolder"], ConfigurationManager.AppSettings["tiraTestFolder"], sm, nGramSize, lp);
                }
                else if (!args.Any())
                {
                    Console.WriteLine("\n\nyou must specify a directory!");
                }
                else if (!Directory.Exists(args[0]))
                {
                    Console.WriteLine("\n\nDirectory {0} not found!", args[0]);
                }
                else if (!Directory.Exists(args[1]))
                {
                    Console.WriteLine("\n\nDirectory {0} not found!", args[1]);
                }
                else
                {
                    Console.WriteLine("setting:         real run on folder: ", Path.GetDirectoryName(args[0]));
                    Console.WriteLine("Starting execution ---------------------- [TIRA Experiment]");

                    new TiraExperiment().Start(args[0], args[1], sm, nGramSize, lp);
                }
            }
            else if (experiment == "expOne")
            {
                Console.WriteLine("---------------------- Starting execution [GREEK Authors]");
                new ExperimentOne().Start(lp, sm);
            }
        }
    }
}