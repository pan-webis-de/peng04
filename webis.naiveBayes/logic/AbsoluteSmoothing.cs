using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.logic
{
    public class AbsoluteSmoothing : ISmoothingTechnique
    {
        private TextSource _referenceSource;
        private double _b;

        public AbsoluteSmoothing(double b)
        {
            _b = b;
        }

        public AbsoluteSmoothing(TextSource referenceSource, int n)
        {
            _referenceSource = referenceSource;
            var segments = _referenceSource.GetAllSegments();

            int n1 = 0;
            int n2 = 0;

            List<IEnumerable<string>> checkedGrams = new List<IEnumerable<string>>();

            foreach (var item in _referenceSource.Documents)
            {
                for (int i = 0; i <= item.LanguageSegments.Count - n; i++)
                {
                    IEnumerable<string> ngram = item.LanguageSegments.Skip(i).Take(n).ToArray();
                    if (checkedGrams.Any(el => el.SequenceEqual(ngram))) continue;
                    
                    checkedGrams.Add(ngram);
                    int frequency = _referenceSource.FindOccurrences(ngram);

                    if (frequency == 1) n1++;
                    if (frequency == 2) n2++;
                }
            }

            _b = (double)n1 / ((double)n1 + 2 * (double)n2);
        }

        public double Discount(int frequency)
        {
            return frequency - _b;
        }
    }
}