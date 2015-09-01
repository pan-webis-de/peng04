using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.logic;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.experiments
{
    public class ExperimentOne
    {
        public void Start()
        {
            var bayesClassifier = new BayesTextClassifier();

            var docReader = new ReadDocumentFromFile();
            var docPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var authors = new DirectoryInfo(docPath).GetDirectories();

            var categories = new List<TextSource>();
            var processor = new WordLevelProcessor();

            foreach (var item in authors)
            {
                var docs = item.GetFiles();
                var dataSource = new List<string>();

                foreach (var doc in docs)
                {
                    try
                    {
                        dataSource.Add(docReader.ReadDocumentText(doc.FullName));
                    }
                    catch
                    {
                        Console.WriteLine("Document {0} unreadable", doc.FullName);
                    }
                }

                categories.Add(processor.Process(dataSource, item.Name));
            }

            Console.WriteLine("Scanned {1} documents in {0} categories", categories.Count, categories.Select(el => el.Documents.Count).Aggregate((el1, el2) => el1 + el2));

            Console.WriteLine("Set n=2");
            Console.WriteLine("classifying documents of category 1");

            int x = 0;
            foreach (var item in new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test")).GetDirectories()[0].GetFiles())
            {
                x++;
                Console.WriteLine("\n-----------------------");
                Console.WriteLine("Classifying document {0} of 10", x);

                double maxP = -100000;
                int maxI = 0;

                Parallel.For(0, 10, i =>
                {
                    double p = bayesClassifier.P_c(categories[i], categories[0].Documents[0], 2, 1.0 / 10.0);
                    Console.WriteLine("Prob cat {1}: {0}", p, i + 1);

                    if(p > maxP)
                    {
                        maxP = p;
                        maxI = i;
                    }
                });

                Console.WriteLine("Classified document as category {0}", maxI + 1);
            }
        }
    }
}