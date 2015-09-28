using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using webis.naiveBayes.logic;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.experiments
{
    public class TiraExperiment
    {
        public void Start(string mainFolder, bool skipSmoothing)
        {
            var resultSet = new ResultSet();
            var bayesClassifier = new BayesTextClassifier();

            var docReader = new ReadDocumentFromTiraFile();
            var docPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
            var authors = new DirectoryInfo(docPath).GetDirectories();

            var categories = new List<TextSource>();
            var processor = new WordLevelProcessor();

            Console.WriteLine("Scanning...");
            dynamic jsonConfig;

            using (StreamReader sr = new StreamReader(Path.Combine(mainFolder, "meta-file.json")))
            {
                jsonConfig = JsonConvert.DeserializeObject(sr.ReadToEnd());
            }

            var unknownFolder = (string)jsonConfig.folder;
            var encodingString = (string)jsonConfig.encoding;
            var cultureString = (string)jsonConfig.language;

            CultureInfo ci = null;
            switch (cultureString)
            {
                case "EN":
                    ci = new CultureInfo("en-US");
                    break;
                default:
                    throw new ApplicationException("culture not found");
            }

            Encoding encoding = null;
            switch (encodingString)
            {
                case "UTF8":
                    encoding = Encoding.UTF8;
                    break;
                case "ASCII":
                    encoding = Encoding.ASCII;
                    break;
                default:
                    throw new ApplicationException("encoding not found");
            }

            foreach(var item in jsonConfig["candidate-authors"])
            {
                var authorName = (string)item["author-name"];
                var docs = new DirectoryInfo(Path.Combine(mainFolder, authorName)).GetFiles();
                var dataSource = new List<string>();

                foreach (var doc in docs)
                {
                    try
                    {
                        dataSource.Add(docReader.ReadDocumentText(doc.FullName, encoding, ci));
                    }
                    catch
                    {
                        Console.WriteLine("Document {0} unreadable", doc.FullName);
                    }
                }

                categories.Add(processor.Process(dataSource, authorName));
            }

            int n = 2; // choose n=2

            Console.WriteLine("Scanned {1} documents in {0} categories", categories.Count, categories.Select(el => el.Documents.Count).Aggregate((el1, el2) => el1 + el2));

            var allInOne = new TextSource();
            allInOne.Documents.AddRange(categories.SelectMany(el => el.Documents));

            Console.WriteLine("Building hash tables ..", n);

            Parallel.ForEach(categories, category =>
            {
                category.BuildSegmentTable(n);
            });

            Console.WriteLine("Getting smoothing ready ..");
            var smoothing = skipSmoothing ? new AbsoluteSmoothing(0.6) : new AbsoluteSmoothing(allInOne, n);
            var categoriesToTest = new Dictionary<TextSource, CategoryProbabilityDistribution>();

            foreach (var cat in categories)
            {
                categoriesToTest[cat] = new CategoryProbabilityDistribution(cat, smoothing, n);
            }

            Console.WriteLine("Start classifying ..");

            foreach (var item in jsonConfig["unknown-texts"])
            {
                TextSource topCategory = null;
                var maxProb = 0.0;
                var textName = (string)item["unknown-text"];
                var probs = new List<double>();

                Parallel.ForEach(categoriesToTest, catDist =>
                {
                    var docText = new string[] { docReader.ReadDocumentText(Path.Combine(mainFolder, unknownFolder, textName), encoding, ci) };
                    var docSource = processor.Process(docText, "unknown").Documents.First();

                    double p = bayesClassifier.P_c(catDist.Value, docSource, n, 1.0 / categories.Count);
                    probs.Add(p);

                    if (topCategory == null || p > maxProb)
                    {
                        topCategory = catDist.Key;
                        maxProb = p;
                    }
                });

                // getting the score
                probs.Remove(maxProb);
                double pre_score = 0.0;

                foreach (var p in probs)
                {
                    var subScore = (maxProb - p) / maxProb; // normalized difference
                    Math.Exp(-subScore);
                }


                Console.WriteLine("Classified {0} as author {1}", textName, topCategory.Name);
                resultSet.answers.Add(new Result(textName, topCategory.Name, 1));

                Console.WriteLine("writing data to file ...");
                string data = JsonConvert.SerializeObject(resultSet, Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(Path.Combine(mainFolder, "results.json"), false))
                {
                    sw.Write(data);
                    sw.Flush();
                }
            }
        }
        
        public class ResultSet
        {
            public ResultSet()
            {
                answers = new List<Result>();
            }

            public List<Result> answers { get; set; }
        }

        public class Result
        {
            public Result()
            {
            }

            public Result(string unknown_text, string author, double score)
            {
                this.unknown_text = unknown_text;
                this.author = author;
                this.score = score;
            }

            public string unknown_text { get; set; }
            public string author { get; set; }
            public double score { get; set; }
        }
    }
}