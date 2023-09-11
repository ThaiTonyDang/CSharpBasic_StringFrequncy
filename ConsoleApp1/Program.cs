using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

public interface IDataAnalyser
{
    string Author { get; }
    string Path { get; }
    IEnumerable<string> GetTopTenFrequentStrings(string path);
}

public class NoahAnalyser : IDataAnalyser
{
    public string Author => "Alex";
    public string Path { get; }

    public NoahAnalyser(string path)
    {
        Path = path;
    }

    public IEnumerable<string> GetTopTenFrequentStrings(string path)
    {
        var files = Directory.GetFiles(path, "*.dat");
        var lines = new List<string>();

        Parallel.ForEach(files, file =>
        {
            using (var stream = File.OpenRead(file))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line != null)
                    {
                        lines.Add(line);
                    }
                }
            }
        });

        var query = lines.AsParallel()
            .SelectMany(line => (line ?? "").Split(';'))
            .Select(word => word.Trim().ToLower())
            .Where(word => word.Length == 10)
            .GroupBy(word => word)
            .OrderByDescending(group => group.Count())
            .Select(group => group.Key)
            .Take(10);

        return query;
    }
}

public static class StreamReaderExtensions
{
    public static IEnumerable<string> Lines(this StreamReader reader)
    {
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            yield return line;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        string folderPath = @"C:\Data";
        List<IDataAnalyser> Analysers = new List<IDataAnalyser>();
        Analysers.Add(new NoahAnalyser(folderPath));
        foreach (IDataAnalyser analyser in Analysers)
        {
            Console.WriteLine($"Author is {analyser.Author} ");
            var sw = Stopwatch.StartNew();
            var result = analyser.GetTopTenFrequentStrings(analyser.Path).ToList(); // Materialize the query
            sw.Stop();

            Console.WriteLine("Top 10 frequent strings:");
            foreach (string str in result)
            {
                Console.WriteLine(str);
            }
            Console.WriteLine($"Elapsed time: {sw.ElapsedMilliseconds} ms");
        }
        Console.WriteLine("Press any key to exit.");
        Console.ReadKey();
    }
}