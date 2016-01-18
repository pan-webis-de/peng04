using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using peng04.processing;

namespace peng04.logic
{
    public interface ISmoothingTechnique
    {
        double Discount(int frequency);
        void Init(TextSource referenceSource, int n);
        void Init(double factor);
    }
}