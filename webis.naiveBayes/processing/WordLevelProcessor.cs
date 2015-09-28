using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webis.naiveBayes.processing
{
    public class WordLevelProcessor : ILanguageProcessor
    {
        public TextSource Process(IEnumerable<string> documents)
        {
            return Process(documents, string.Empty);
        }

        public TextSource Process(IEnumerable<string> documents, string name)
        {
            TextSource result = new TextSource();

            foreach (var item in documents)
            {
                var doc = new DocumentSource();
                var sItem = item.Replace(".", " . ")
                                .Replace(",", " , ");

                foreach (var segment in sItem.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    doc.LanguageSegments.Add(segment);
                }

                result.Documents.Add(doc);
            }

            result.Name = name;
            return result;
        }
    }
}