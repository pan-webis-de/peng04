using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.logic
{
    public class CategoryProbabilityDistribution
    {
        private Dictionary<string, double> _betaCache = new Dictionary<string, double>(); 
        private TextSource _referenceSource;
        private ISmoothingTechnique _smoothing;

        public CategoryProbabilityDistribution(TextSource referenceSource, ISmoothingTechnique smoothing, int n)
        {
            _smoothing = smoothing;
            _referenceSource = referenceSource;
        }

        public double GetProbability(IEnumerable<string> ngram)
        {
            if (ngram.Count() == 0) throw new ArgumentException("ngram");
            int frequency = _referenceSource.FindOccurrences(ngram);

            // if we cannot find the word at all, give it at least one count (laplace, add one)
            if (ngram.Count() == 1) return GetProbabilityIfPresent(ngram, frequency + 1);

            if(frequency > 0)
            {
                return GetProbabilityIfPresent(ngram, frequency);
            }
            else
            {
                return GetBeta(ngram) * GetProbability(ngram.Skip(1));
            }
        }

        private double GetBeta(IEnumerable<string> ngram)
        {
            string[] ngramArray = ngram.ToArray();
            var hash = NGramHashing.Hash(ngramArray.Take(ngramArray.Length - 1));
            if (_betaCache.ContainsKey(hash)) return _betaCache[hash];

            double a = 0.0, b = 0.0;

            foreach (var item in _referenceSource.GetAllSegments())
            {
                ngramArray[ngramArray.Length - 1] = item; // replace last segment
                int frequencyA = _referenceSource.FindOccurrences(ngramArray);

                if(frequencyA > 0)
                {
                    int frequencyB = _referenceSource.FindOccurrences(ngramArray.Skip(1).ToArray());

                    a += GetProbabilityIfPresent(ngramArray, frequencyA);
                    b += GetProbabilityIfPresent(ngramArray.Skip(1).ToArray(), frequencyB);
                }
            }

            var beta = (1.0 - a) / (1.0 - b);
            _betaCache[hash] = beta;
            return beta;
        }

        private double GetProbabilityIfPresent(IEnumerable<string> ngram, int frequency)
        {
            return _smoothing.Discount(frequency) / _referenceSource.FindOccurrences(ngram.Take(ngram.Count() - 1));
        }
    }
}