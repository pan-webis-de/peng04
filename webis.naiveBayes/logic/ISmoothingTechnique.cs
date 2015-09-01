using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.logic
{
    public interface ISmoothingTechnique
    {
        double Discount(int frequency);
    }
}