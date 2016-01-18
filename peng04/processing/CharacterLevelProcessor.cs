using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace peng04.processing
{
    public class CharacterLevelProcessor : ILanguageProcessor
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

                foreach (var segment in item)
                {
                    doc.LanguageSegments.Add(segment.ToString());
                }

                result.Documents.Add(doc);
            }

            result.Name = name;
            return result;
        }
    }
}