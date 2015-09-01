using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class TextSource
    {
        public TextSource()
        {
            this.Documents = new List<DocumentSource>();
        }

        public List<DocumentSource> Documents { get; private set; }

        public int FindOccurrences(IEnumerable<string> ngram)
        {
            if(ngram.Count() == 0)
            {
                // considering unigrams, we need to fit prob 1 total
                // this means we have to divide by number of segments total

                return Documents.SelectMany(el => el.LanguageSegments).Count();
            }

            int result = 0;

            foreach (var item in Documents)
            {
                result += item.FindOccurrences(ngram);
            }

            return result;
        }

        public IEnumerable<string> GetAllSegments()
        {
            return Documents.SelectMany(el => el.LanguageSegments).Distinct();
        }

        public string Name { get; set; }
    }
}