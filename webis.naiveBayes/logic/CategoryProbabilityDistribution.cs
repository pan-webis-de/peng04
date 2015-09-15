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
            if (ngram.Count() == 1) return _smoothing.Discount(_referenceSource.FindOccurrences(ngram) + 1) / (_referenceSource.FindOccurrences(new string[0]) + _referenceSource.GetAllSegments().Count());
            // if we cannot find the word at all, give it at least one count (laplace, add one)

            int frequency = _referenceSource.FindOccurrences(ngram);

            if(frequency > 0)
            {
                return _smoothing.Discount(frequency) / _referenceSource.FindOccurrences(ngram.Take(ngram.Count() - 1));
            }
            else
            {
                return GetBeta(ngram) * GetProbability(ngram.Skip(1));
            }
        }

        private double GetBeta(IEnumerable<string> ngram)
        {
            double a = 0.0, b = 0.0;
            string[] ngramArray = ngram.ToArray();

            foreach (var item in _referenceSource.GetAllSegments())
            {
                ngramArray[ngramArray.Length - 1] = item; // replace last segment
                int frequency = _referenceSource.FindOccurrences(ngramArray);

                if(frequency > 0)
                {
                    a += GetProbability(ngramArray);
                    b += GetProbability(ngramArray.Skip(1));
                }
            }

            return (1.0 - a) / (1.0 - b);
        }
    }
}