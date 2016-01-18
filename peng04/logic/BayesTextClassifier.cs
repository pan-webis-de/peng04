using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using peng04.processing;

namespace peng04.logic
{
    public class BayesTextClassifier
    {
        public double P_c(CategoryProbabilityDistribution trainingDistribution, DocumentSource testData, int n, double prob_c)
        {
            var result = Math.Log10(prob_c);
            var source = testData.LanguageSegments.ToArray();

            for (int i = 0; i <= source.Length - n; i++)
            {
                string[] ngram = source.GetNGram(i, n);
                var logProb = Math.Log10(trainingDistribution.GetProbability(ngram));                
                result += logProb;
            }

            return result;
        }
    }
}