using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.logic
{
    public class LinearSmoothing : ISmoothingTechnique
    {
        double factor;

        public LinearSmoothing(TextSource referenceSource, int n)
        {
            var segments = referenceSource.GetAllSegments();

            int n1 = 0;

            List<IEnumerable<string>> checkedGrams = new List<IEnumerable<string>>();

            foreach (var item in referenceSource.Documents)
            {
                for (int i = 0; i <= item.LanguageSegments.Count - n; i++)
                {
                    IEnumerable<string> ngram = item.LanguageSegments.Skip(i).Take(n).ToArray();
                    if (checkedGrams.Any(el => el.SequenceEqual(ngram))) continue;

                    checkedGrams.Add(ngram);
                    int frequency = referenceSource.FindOccurrences(ngram);

                    if (frequency == 1) n1++;
                }
            }

            factor = 1.0 - ((double)n1 / (double)referenceSource.GetAllSegments().Count());
        }

        public double Discount(int frequency)
        {
            return frequency * factor;
        }
    }
}