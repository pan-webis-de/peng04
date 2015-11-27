using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.logic;

namespace webis.naiveBayes.processing
{
    public class TextSource
    {
        private NGramCache _segmentTable;
        private IEnumerable<string> _segmentCache;

        public TextSource()
        {
            this.Documents = new List<DocumentSource>();
        }

        public List<DocumentSource> Documents { get; private set; }

        public int FindOccurrences(IEnumerable<string> ngram)
        {
            if (ngram.Count() == 0)
            {
                // considering unigrams, we need to fit prob 1 total
                // this means we have to divide by number of segments total

                return Documents.Aggregate(0, (count, doc) => count + doc.LanguageSegments.Count);
            }

            if (_segmentTable != null)
            {
                double data;
                if (_segmentTable.TryFindValue(ngram, out data))
                {
                    return Convert.ToInt32(data);
                }
                else
                {
                    return 0;
                }
            }
            
            int result = 0;

            foreach (var item in Documents)
            {
                result += item.FindOccurrences(ngram);
            }

            return result;
        }

        public void BuildSegmentTable(int n)
        {
            if(_segmentTable == null) _segmentTable = new NGramCache();

            foreach (var item in Documents)
            {
                var array = item.LanguageSegments.ToArray();
                for (int i = 0; i <= array.Length - n; i++)
                {                    
                    _segmentTable.Increment(array.GetNGram(i, n), 1); // inc count
                }
            }
        }

        public IEnumerable<string> GetAllSegments()
        {
            if (_segmentCache == null) _segmentCache = Documents.SelectMany(el => el.LanguageSegments).Distinct().ToArray();
            return _segmentCache;
        }

        public NGramCache GetNGramCache()
        {
            return _segmentTable;
        }

        public void SetNGramCache(NGramCache value)
        {
            _segmentTable = value;
        }

        public string Name { get; set; }
    }
}