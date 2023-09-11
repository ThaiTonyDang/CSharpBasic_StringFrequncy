using Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlexDataAnalyser
{
    public class NoahAnalyser : IDataAnalyser
    {
        private const int TopCount = 10;

        public NoahAnalyser(string path)
        {
            this.Path = path;
        }

        public string Author => "Noah";

        public string Path { get; }

        public IEnumerable<string> GetTopTenStrings(string path)
        {
            var listString = new Dictionary<int, int>();
            var files = Directory.GetFiles(path, "*.dat");
            var topHasList = new Dictionary<int, int>();

            var count = 1;
            foreach (var filePath in Directory.GetFiles(path, "*.dat"))
            {
                Console.WriteLine($"Reading file ${count} {filePath}");

                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var strings = line.Split(';').Where(x => !string.IsNullOrEmpty(x));

                        foreach (var str in strings)
                        {
                            var hashCodeString = str.Trim().ToLower().GetHashCode();

                            if (listString.ContainsKey(hashCodeString))
                            {
                                listString[hashCodeString]++;
                            }
                            else
                            {
                                listString[hashCodeString] = 1;
                            }

                            // add to topHashList
                            AddToHashString(hashCodeString, listString[hashCodeString], topHasList);
                        }
                    }
                }

                count++;
            }

            //var topHashString = listString.OrderByDescending(x => x.Value)
            //                               .Take(10).OrderByDescending(x=>x.Value)
            //                               .ToDictionary(x => x.Key, x => x.Value);

            var topSort = topHasList.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);

            var topListString = GetListStringHash(path, topSort);
            return topListString.Select(x => x);
        }

        public void AddToHashString(int hashCode, int count, Dictionary<int, int> topHasList)
        {
            var max = 0;
            var min = 0;
            if (topHasList.Any())
            {
                max = topHasList.Max(x => x.Value);
                min = topHasList.Min(x => x.Value);
            }

            if (count >= max)
            {
                if (topHasList.Count <= 10)
                {
                    if (topHasList.ContainsKey(hashCode))
                    {
                        topHasList[hashCode] = count;
                    }
                    else
                    {
                        topHasList.Add(hashCode, count);
                    }
                    return;
                }

                if (topHasList.ContainsKey(hashCode))
                {
                    topHasList[hashCode] += 1;
                }
                else
                {
                    var item = topHasList.First(kvp => kvp.Value == min);
                    topHasList.Remove(item.Key);
                    topHasList.Add(hashCode, count);
                }
                return;
            }

            if (count >= min)
            {
                if (topHasList.Count <= 10)
                {
                    if (topHasList.ContainsKey(hashCode))
                    {
                        topHasList[hashCode] = count;
                    }
                    else
                    {
                        topHasList.Add(hashCode, count);
                    }
                    return;
                }

                
                if (topHasList.ContainsKey(hashCode))
                {
                    topHasList[hashCode] = count;
                }
                else
                {
                    var item = topHasList.First(kvp => kvp.Value == min);
                    topHasList.Remove(item.Key);
                    topHasList.Add(hashCode, count);
                }
                return;
            }

            if (count == min)
            {
                var item = topHasList.Where(kvp => kvp.Value == min).OrderBy(x => x.Key).First(); // 3

                if (hashCode > item.Key)
                {
                    topHasList.Remove(item.Key);
                    topHasList.Add(hashCode, count);
                }
            }
        }

        public List<string> GetListStringHash(string path, Dictionary<int, int> topHashCode)
        {
            var count = 1;
            var listString = new List<string>();
            var dict = new Dictionary<string, int>();
            foreach (var filePath in Directory.GetFiles(path, "*.dat"))
            {
                Console.WriteLine($"Reading file ${count} {filePath}");

                using (var reader = new StreamReader(filePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        var strings = line.Split(';').Where(x => !string.IsNullOrEmpty(x));
                        foreach (var str in strings)
                        {
                            var cleanedString = str.Trim().ToLower().GetHashCode();
                            foreach (var item in topHashCode)
                            {
                                if (cleanedString == item.Key)
                                {
                                    listString.Add(str);
                                    topHashCode.Remove(item.Key);
                                    dict.Add(str, item.Value);
                                    break;
                                }
                            }
                        }
                    }
                }

                count++;
            }

            var list = dict.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();

            return list;
        }
    }
}
