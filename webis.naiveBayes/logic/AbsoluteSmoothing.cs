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
        private bool _initPassed;

        public void Init(double b)
        {
            if (_initPassed) return;
            _initPassed = true;

            _b = b;
        }

        public void Init(TextSource referenceSource, int n)
        {
            if (_initPassed) return;
            _initPassed = true;

            if (n < 1)
            {
                _b = 0;
                return;
            }

            _referenceSource = referenceSource;

            int n1 = 0;
            int n2 = 0;

            IEnumerable<NGramCache> nGrams = referenceSource.GetNGramCache().NextSegment.Values;

            while (n > 1)
            {
                n--;
                nGrams = nGrams.SelectMany(el => el.NextSegment.Values);
            }

            foreach (var item in nGrams)
            {
                var frequency = Convert.ToInt32(item.Value);
                if (frequency == 1) n1++;
                if (frequency == 2) n2++;
            }

            _b = (double)n1 / ((double)n1 + 2 * (double)n2);
        }

        public double Discount(int frequency)
        {
            return frequency - _b;
        }
    }
}