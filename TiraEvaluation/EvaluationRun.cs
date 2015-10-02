using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiraEvaluation
{
    public class EvaluationRun
    {
        public void Evaluate(IEnumerable<TiraEntry> resultPaths, string groundTruthPath)
        {
            dynamic groundTruth;
            using (StreamReader sr = new StreamReader(groundTruthPath))
            {
                groundTruth = JsonConvert.DeserializeObject(sr.ReadToEnd());
            }

            Dictionary<string, string> correctMapping = new Dictionary<string, string>();
            foreach (var item in groundTruth["ground-truth"])
            {
                var textName = (string)item["unknown-text"];
                var trueAuthor = (string)item["true-author"];

                correctMapping.Add(textName, trueAuthor);
            }

            foreach (var item in resultPaths)
            {
                int right = 0;
                int wrong = 0;

                dynamic result;
                using (StreamReader sr = new StreamReader(item.Path))
                {
                    result = JsonConvert.DeserializeObject(sr.ReadToEnd());
                }

                foreach (var answer in result["answers"])
                {
                    var textName = (string)answer["unknown_text"];
                    var author = (string)answer["author"];

                    if(correctMapping[textName] == author)
                    {
                        right++;
                    }
                    else
                    {
                        wrong++;
                    }
                }

                item.OverallAccuracy = Math.Round((double)right / ((double)wrong + (double)right), 2);
            }
        }
    }
}