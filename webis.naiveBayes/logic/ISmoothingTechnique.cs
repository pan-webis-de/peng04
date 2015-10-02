using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.logic
{
    public interface ISmoothingTechnique
    {
        double Discount(int frequency);
        void Init(TextSource referenceSource, int n);
        void Init(double factor);
    }
}