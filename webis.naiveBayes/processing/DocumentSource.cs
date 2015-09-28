using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class DocumentSource
    {
        private Dictionary<int, Dictionary<string, int>> _segmentTable = new Dictionary<int, Dictionary<string, int>>();

        public DocumentSource()
        {
            LanguageSegments = new List<string>();
        }

        public List<string> LanguageSegments { get; private set; }

        public void BuildSegmentTable(int n)
        {
            var result = new Dictionary<string, int>();

            for (int i = 0; i <= this.LanguageSegments.Count - n; i++)
            {
                IEnumerable<string> ngram = this.LanguageSegments.Skip(i).Take(n).ToArray();
                var hash = NGramHashing.Hash(ngram);
                if (!result.ContainsKey(hash)) result[hash] = 0;

                result[hash]++;
            }

            _segmentTable[n] = result;
        }

        public int FindOccurrences(IEnumerable<string> ngram)
        {
            int n = ngram.Count();
            if (_segmentTable.ContainsKey(n))
            {
                var hash = NGramHashing.Hash(ngram);
                return _segmentTable[n].ContainsKey(hash) ? _segmentTable[n][hash] : 0;
            }

            int result = 0;

            string startSegment = ngram.First();
            string[] segments = LanguageSegments.ToArray();

            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i] == startSegment)
                {
                    int j = i;
                    bool foundMatch = true;

                    foreach (var item in ngram)
                    {
                        if (j >= segments.Length || segments[j] != item)
                        {
                            foundMatch = false;
                            break;
                        }

                        j++;
                    }

                    if (foundMatch) result++;
                }
            }

            return result;
        }
    }
}