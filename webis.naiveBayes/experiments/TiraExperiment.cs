using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using webis.naiveBayes.logic;
using webis.naiveBayes.processing;

namespace webis.naiveBayes.experiments
{
    public class TiraExperiment
    {
        public void Start(string mainFolder, string outputDir, ISmoothingTechnique smoothing, int nGramSize, ILanguageProcessor processor)
        {
            var resultSet = new ResultSet();
            var bayesClassifier = new BayesTextClassifier();

            var docReader = new ReadDocumentFromTiraFile();
            
            var categories = new List<TextSource>();

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

            int n = nGramSize;

            Console.WriteLine("Scanned {1} documents in {0} categories", categories.Count, categories.Select(el => el.Documents.Count).Aggregate((el1, el2) => el1 + el2));

            var allInOne = new TextSource();
            allInOne.Documents.AddRange(categories.SelectMany(el => el.Documents));

            Console.WriteLine("Building hash tables ..", n);
            
            Parallel.ForEach(categories, category =>
            {
                for (int i = 1; i <= n; i++)
                {
                    category.BuildSegmentTable(i);
                    Console.WriteLine("hashed {0} with n={1}", category.Name, i);
                }
            });

            allInOne.SetNGramCache(NGramCache.Aggregate(categories.Select(el => el.GetNGramCache())));
            Console.WriteLine("aggregated hashing");

            Console.WriteLine("Getting smoothing ready ..");
            smoothing.Init(allInOne, nGramSize);
            var categoriesToTest = new Dictionary<TextSource, CategoryProbabilityDistribution>();

            foreach (var cat in categories)
            {
                categoriesToTest[cat] = new CategoryProbabilityDistribution(cat, smoothing, n);
            }

            Console.WriteLine("Start classifying ..");
            int totalProgress = jsonConfig["unknown-texts"].Count * categoriesToTest.Count;
            int progress = 0;

            System.Timers.Timer t = new System.Timers.Timer(5000);
            t.Elapsed += (sender, eventArgs) =>
            {
                Console.Title = "Task is Running. Progress: " + Math.Round((((double)progress / (double)totalProgress) * 100.0), 2).ToString() + "%";
            };

            t.AutoReset = true;
            t.Start();

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

                    double p = bayesClassifier.P_c(catDist.Value, docSource, n, (double)catDist.Key.Documents.Count / (double)allInOne.Documents.Count);
                    
                    lock (probs)
                    {
                        probs.Add(p);

                        if (topCategory == null || p > maxProb)
                        {
                            topCategory = catDist.Key;
                            maxProb = p;
                        }
                    }

                    Interlocked.Increment(ref progress);
                });

                // getting the score
                probs.Remove(maxProb);
                double pre_score = 0.0;
                double max_sub_score = 0.0;

                foreach (var p in probs)
                {
                    var subScore = Math.Abs((maxProb - p) / maxProb) * Math.Pow(Math.E, 3); // normalized difference
                    var eSubScore = Math.Exp(-subScore);
                    pre_score += eSubScore;

                    if (eSubScore > max_sub_score)
                        max_sub_score = eSubScore;
                }

                double score = Math.Round(1.0 - (0.5 * (pre_score / probs.Count) + 0.5 * max_sub_score), 2);

                Console.WriteLine("Classified {0} as author {1} with score {2}", textName, topCategory.Name, score);
                resultSet.answers.Add(new Result(textName, topCategory.Name, score));

                Console.WriteLine("writing data to file ...");
                string data = JsonConvert.SerializeObject(resultSet, Formatting.Indented);
                using (StreamWriter sw = new StreamWriter(Path.Combine(outputDir, "answers.json"), false))
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