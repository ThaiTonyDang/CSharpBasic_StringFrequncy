using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public interface IDataAnalyser
{
    string Author { get; }
    string Path { get; }
    IEnumerable<string> GetTopTenFrequentStrings();
}

public class NoahAnalyser : IDataAnalyser
{
    public string Author => "Alex";
    public string Path { get; private set; }

    public NoahAnalyser(string path)
    {
        Path = path;
    }

    public IEnumerable<string> GetTopTenFrequentStrings()
    {
        var stringCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var filePath in Directory.GetFiles(Path, "*.dat"))
        {
            foreach (var line in File.ReadLines(filePath))
            {
                var parts = line.Split(';');
                foreach (var part in parts)
                {
                    var normalizedString = part.Trim().ToLowerInvariant();
                    if (!string.IsNullOrEmpty(normalizedString))
                    {
                        int hashCode = normalizedString.GetHashCode();
                        string originalString = normalizedString;
                        if (stringCounts.ContainsKey(hashCode.ToString()))
                            originalString = stringCounts[hashCode.ToString()].ToString();

                        if (stringCounts.TryGetValue(originalString, out int count))
                            stringCounts[originalString] = count + 1;
                        else
                            stringCounts[originalString] = 1;
                    }
                }
            }
        }

        var topTenFrequentStrings = stringCounts
            .OrderByDescending(pair => pair.Value)
            .Take(10)
            .Select(pair => pair.Key);

        return topTenFrequentStrings;
    }
}

class Program
{
    static void Main(string[] args)
    {
        string dataFolderPath = @"C:\Data";
        IDataAnalyser analyser = new NoahAnalyser(dataFolderPath);

        // Measure memory consumption
        using (Process process = Process.GetCurrentProcess())
        {
            long memoryBefore = process.PrivateMemorySize64;

            var topTenStrings = analyser.GetTopTenFrequentStrings();

            long memoryAfter = process.PrivateMemorySize64;
            long memoryUsedMB = (memoryAfter - memoryBefore) / (1024 * 1024);

            Console.WriteLine($"Top 10 most frequent strings:");
            foreach (var str in topTenStrings)
            {
                Console.WriteLine(str);
            }

            Console.WriteLine($"Memory used: {memoryUsedMB}MB");
        }
    }
}