#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

#endregion

namespace NodeNetwork.Example {
    public class Program {
        #region MEMBERS

        private static NodeNetwork<string> _network;

        #endregion

        private static void Main(string[] args) {
            if (File.Exists(@"config.json"))
                _network = JsonConvert.DeserializeObject<NodeNetwork<string>>(File.ReadAllText(@"config.json"));
            else
                WriteConfig();

            Console.ReadLine();
        }

        private static void WriteConfig() {
            _network = new NodeNetwork<string>();
            Benchmark.Timer.Reset();

            Console.Write("Reading text from training file: ");
            Benchmark.Timer.Start();
            string rawInput = File.ReadAllText($"{AppContext.BaseDirectory}\\TrainingFile.txt");
            Console.WriteLine($"{Benchmark.Timer.ElapsedMilliseconds}ms");
            Benchmark.Timer.Reset();

            Console.Write("Symbol to new-line replacement pass: ");
            Benchmark.Timer.Start();
            string replacePunctuation = rawInput.OutstandSymbols();
            Console.WriteLine($"{Benchmark.Timer.ElapsedMilliseconds}ms");
            Benchmark.Timer.Reset();

            Console.Write("New-line split pass: ");
            Benchmark.Timer.Start();
            List<string> splitByNewLines = replacePunctuation.Split(' ', '\r', '\n').ToList();
            splitByNewLines.ToList().RemoveAll(string.IsNullOrEmpty);
            Console.WriteLine($"{Benchmark.Timer.ElapsedMilliseconds}ms");
            Benchmark.Timer.Reset();

            Console.WriteLine("Processing input list to create node network...");
            Benchmark.Timer.Start();
            _network.ProcessInput(splitByNewLines.ToArray());
            Console.WriteLine($"Creation of node network completed: {Benchmark.Timer.ElapsedMilliseconds}ms");
            Benchmark.Timer.Reset();

            Console.Write("Writing serialized network to file: ");
            Benchmark.Timer.Start();
            File.WriteAllText("config.json", JsonConvert.SerializeObject(_network));
            Console.WriteLine($"{Benchmark.Timer.ElapsedMilliseconds}ms");
            Benchmark.Timer.Reset();

            Console.WriteLine("All processes complete.");
        }

        //private static readonly Regex _RegexIllegal = new Regex(@"[^A-Za-z0-9\.\?\!\;\:\(\)\[\]\-\'\r\n\’\ ]+", RegexOptions.Compiled);
        //private static readonly Regex _RegexSymbols = new Regex(@"[\.\?\!\:\;\(\)\[\]]+", RegexOptions.Compiled);
    }

    public static class Extensions {
        public static string OutstandSymbols(this string input) {
            Regex alphanumeric = new Regex(@"[A-Za-z0-9 ]", RegexOptions.Compiled);

            StringBuilder output = new StringBuilder(input.Length * 2);

            foreach (char character in input)
                if (alphanumeric.IsMatch(character.ToString()))
                    output.Append(character);
                else
                    output.Append($" {character} ");

            return output.ToString();
        }

        public static IEnumerable<string> CleanseConsecutiveSpaces(this IEnumerable<string> enumerable) {
            foreach (string str in enumerable) {
                if (string.IsNullOrWhiteSpace(str))
                    continue;

                bool charWasSpace = false;

                StringBuilder builder = new StringBuilder();

                foreach (char character in str)
                    if (character.Equals(' ')) {
                        if (charWasSpace)
                            continue;

                        builder.Append(character);
                        charWasSpace = true;
                    } else {
                        builder.Append(character);
                        charWasSpace = false;
                    }

                yield return builder.ToString().Trim();
            }
        }
    }
}
