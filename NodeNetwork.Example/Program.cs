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

        private static readonly Regex _RegexReplaceIllegal = new Regex(@"[^A-Za-z\.\?\!\;\:\(\)\[\]\-\ ]+");
        private static readonly string[] _SplitBy = {".", "?", "!", ":", ";", "(", ")", "[", "]", "\n", "\r"};

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

            Console.Write("Initial refactoring pass: ");
            _Stopwatch.Start();
            string refactoredInput = _RegexReplaceIllegal.Replace(rawInput, " ");
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Enumeration split pass: ");
            _Stopwatch.Start();
            IEnumerable<string> enumerable = rawInput.ToEnumerable(_SplitBy);
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Final cleansing pass: ");
            _Stopwatch.Start();
            IEnumerable<string> cleansedEnumerable = enumerable.CleanseConsecutiveSpaces();
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.WriteLine("Processing input list to create node network...");
            _Stopwatch.Start();

            Stack<string> processingStack = new Stack<string>(cleansedEnumerable);

            while (processingStack.Count > 0)
                _network.ProcessInput(processingStack.Pop().Split(" "));

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
        /// <summary>
        ///     Seperates a string into a list of strings by deliminators
        /// </summary>
        /// <param name="input"></param>
        /// <param name="deliminators">characters to split from</param>
        /// <returns></returns>
        public static IEnumerable<string> ToEnumerable(this string input, string[] deliminators) {
            int lastDeliminatorIndex = 0;

            for (int i = 0; i < input.Length; i++)
                if (deliminators.Contains(input[i].ToString())) {
                    // this will retrieve the word from the input, and append a newline character
                    string returnableString = $"{input.Substring(lastDeliminatorIndex, i - lastDeliminatorIndex)}";

                    if (string.IsNullOrEmpty(returnableString))
                        continue;

                    yield return returnableString;

                    lastDeliminatorIndex = i;
                }
        }

        public static IEnumerable<string> CleanseConsecutiveSpaces(this IEnumerable<string> enumerable) {
            foreach (string str in enumerable) {
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
