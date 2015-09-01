using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.logic
{
    public class BayesTextClassifier
    {
        public double P_c(TextSource trainingData, DocumentSource testData, int n, double prob_c)
        {
            var dist = new CategoryProbabilityDistribution(trainingData, n);
            var result = Math.Log10(prob_c);

            for (int i = 0; i <= testData.LanguageSegments.Count - n; i++)
            {
                IEnumerable<string> ngram = testData.LanguageSegments.Skip(i).Take(n);
                result += Math.Log10(dist.GetProbability(ngram));
            }

            return result;
        }
    }
}