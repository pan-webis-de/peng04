using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public static class NGramHashing
    {
        public static string Hash(IEnumerable<string> ngram)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in ngram)
            {
                sb.Append(item);
                sb.Append(" ");
            }

            return sb.ToString();
        }
    }
}