#region usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

#endregion

namespace NodeNetwork.Example {
    internal class Program {
        #region MEMBERS

        private static NodeNetwork<string> _network;

        //private static readonly Regex _RegexIllegal = new Regex(@"[^A-Za-z0-9\.\?\!\;\:\(\)\[\]\-\'\r\n\’\ ]+", RegexOptions.Compiled);
        //private static readonly Regex _RegexSymbols = new Regex(@"[\.\?\!\:\;\(\)\[\]]+", RegexOptions.Compiled);

        private static readonly Stopwatch _Stopwatch = new Stopwatch();

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
            _Stopwatch.Reset();

            Console.Write("Reading text from training file: ");
            _Stopwatch.Start();
            string rawInput = File.ReadAllText($"{AppContext.BaseDirectory}\\TrainingFile.txt");
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Symbol to new-line replacement pass: ");
            _Stopwatch.Start();
            string replacePunctuation = rawInput.OutstandSymbols();
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("New-line split pass: ");
            _Stopwatch.Start();
            List<string> splitByNewLines = replacePunctuation.Split(' ', '\r', '\n').ToList();
            splitByNewLines.ToList().RemoveAll(string.IsNullOrEmpty);
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.WriteLine("Processing input list to create node network...");
            _Stopwatch.Start();
            _network.ProcessInput(splitByNewLines.ToArray());
            Console.WriteLine($"Creation of node network completed: {_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Writing serialized network to file: ");
            _Stopwatch.Start();
            File.WriteAllText("config.json", JsonConvert.SerializeObject(_network));
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.WriteLine("All processes complete.");
        }
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
