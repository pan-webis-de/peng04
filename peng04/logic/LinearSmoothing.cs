using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using peng04.processing;

namespace peng04.logic
{
    public class LinearSmoothing : ISmoothingTechnique
    {
        private double _factor;
        private bool _initPassed;

        public void Init(double factor)
        {
            if (_initPassed) return;
            _initPassed = true;

            _factor = factor;
        }

        public void Init(TextSource referenceSource, int n)
        {
            if (_initPassed) return;
            _initPassed = true;

            if (n < 1)
            {
                _factor = 1;
                return;
            }
            
            int n1 = 0;

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
            }

            _factor = 1.0 - ((double)n1 / (double)referenceSource.GetAllSegments().Count());
        }

        public double Discount(int frequency)
        {
            return frequency * _factor;
        }
    }
}