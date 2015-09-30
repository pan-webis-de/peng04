using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.logic
{
    public class NGramCache
    {
        public NGramCache()
        {
            this.Value = 0;
            this.NextSegment = new Dictionary<string, NGramCache>();
        }

        public void Increment(IEnumerable<string> ngram, double amount)
        {
            Increment(ngram, amount, ngram);
        }

        public void Increment(IEnumerable<string> ngram, double amount, IEnumerable<string> originalNGram)
        {
            if (!ngram.Any())
            {
                this.Value += amount;
                this.NGram = originalNGram;
                return;
            }

            var nextSegmentKey = ngram.First();
            if (!this.NextSegment.ContainsKey(nextSegmentKey))
            {
                this.NextSegment.Add(nextSegmentKey, new NGramCache());
            }

            this.NextSegment[nextSegmentKey].Increment(ngram.Skip(1), amount, originalNGram);
        }

        public bool TryFindValue(IEnumerable<string> ngram, out double data)
        {
            if(!ngram.Any())
            {
                data = this.Value;
                return true;
            }

            var nextSegmentKey = ngram.First();
            if (this.NextSegment.ContainsKey(nextSegmentKey))
            {
                return this.NextSegment[nextSegmentKey].TryFindValue(ngram.Skip(1), out data);
            }

            data = 0.0;
            return false;
        }

        public IEnumerable<string> NGram { get; set; }
        public double Value { get; set; }
        public Dictionary<string, NGramCache> NextSegment { get; set; }

        public static NGramCache Aggregate(IEnumerable<NGramCache> nGramCaches)
        {
            var result = new NGramCache();

            foreach (var item in nGramCaches)
            {
                IEnumerable<NGramCache> nGrams = item.NextSegment.Values;

                while(nGrams.Count() > 0)
                {
                    foreach (var xGram in nGrams)
                    {
                        result.Increment(xGram.NGram, xGram.Value);
                    }

                    // next n
                    nGrams = nGrams.SelectMany(el => el.NextSegment.Values);
                }
            }

            return result;
        }
    }
}