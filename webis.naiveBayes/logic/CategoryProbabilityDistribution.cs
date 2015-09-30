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
        private NGramCache _betaCache = new NGramCache(); 
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

            if (ngram.Count() == 1) return GetProbabilityIfPresent(ngram, frequency);

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
            string[] shortGram = ngramArray.Take(ngramArray.Length - 1).ToArray();

            double beta;
            if (_betaCache.TryFindValue(shortGram, out beta) && beta != 0) return beta;

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
            
            beta = (1.0 - a) / (1.0 - b);
            _betaCache.Increment(shortGram, beta);
            return beta;
        }

        private double GetProbabilityIfPresent(IEnumerable<string> ngram, int frequency)
        {
            return _smoothing.Discount(frequency) / _referenceSource.FindOccurrences(ngram.Take(ngram.Count() - 1));
        }
    }
}