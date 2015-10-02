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
            var ngramArray = ngram.ToArray();

            if (ngramArray.Length == 0) throw new ArgumentException("ngram");
            int frequency = _referenceSource.FindOccurrences(ngramArray);

            if (ngramArray.Length == 1) return GetProbabilityIfPresent(ngramArray, frequency);

            if(frequency > 0)
            {
                return GetProbabilityIfPresent(ngramArray, frequency);
            }
            else
            {
                return GetBeta(ngramArray.ToArray()) * GetProbability(ngramArray.GetNGram(1, ngramArray.Length - 1));
            }
        }

        private double GetBeta(string[] ngram)
        {
            string[] shortGram = ngram.GetNGram(0, ngram.Length - 1);

            double beta;
            if (_betaCache.TryFindValue(shortGram, out beta) && beta != 0) return beta;

            double a = 0.0, b = 0.0;

            foreach (var item in _referenceSource.GetAllSegments())
            {
                ngram[ngram.Length - 1] = item; // replace last segment
                int frequencyA = _referenceSource.FindOccurrences(ngram);

                if(frequencyA > 0)
                {
                    var bNGram = ngram.GetNGram(1, ngram.Length - 1);
                    int frequencyB = _referenceSource.FindOccurrences(bNGram);

                    a += GetProbabilityIfPresent(ngram, frequencyA);
                    b += GetProbabilityIfPresent(bNGram, frequencyB);
                }
            }
            
            beta = (1.0 - a) / (1.0 - b);
            _betaCache.Increment(shortGram, beta);
            return beta;
        }

        private double GetProbabilityIfPresent(string[] ngram, int frequency)
        {
            if (frequency == 0) return CalculationConstants.SmoothingEpsilon / _referenceSource.FindOccurrences(ngram.GetNGram(0, ngram.Length - 1));
            return _smoothing.Discount(frequency) / _referenceSource.FindOccurrences(ngram.GetNGram(0, ngram.Length - 1));
        }
    }
}