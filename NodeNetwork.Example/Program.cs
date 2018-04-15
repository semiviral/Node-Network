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
        private static NodeNetwork<string> _network;

        private static readonly char[] _AcceptableChars = {'-', '.', ';', ':', '!', '?', '(', ')', ' '};
        private static readonly string[] _SplitBy = {".", ";", ":", "!", "?", "(", ")", ",", " "};
        private static readonly string[] _SpecifiedIllegalOutputStrings = {" "};

        private static readonly Stopwatch _Stopwatch = new Stopwatch();

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
            string rawInput = File.ReadAllText($"{AppContext.BaseDirectory}\\TestFile.txt");
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Initial refactoring pass: ");
            _Stopwatch.Start();
            string refactoredInput = rawInput.Refactor(_AcceptableChars);
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Enumeration split pass: ");
            _Stopwatch.Start();
            IEnumerable<string> enumerable = refactoredInput.ToEnumerable(_SplitBy);
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.Write("Final cleansing pass: ");
            _Stopwatch.Start();
            List<string> finalList = enumerable.CleanseEnumerableToList(_SpecifiedIllegalOutputStrings);
            Console.WriteLine($"{_Stopwatch.ElapsedMilliseconds}ms");
            _Stopwatch.Reset();

            Console.WriteLine("Processing input list to create node network...");
            _Stopwatch.Start();
            _network.ProcessInput(finalList);
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
        ///     Cleans a string of any characters that are not explicitly allowed,
        ///     and
        /// </summary>
        /// <param name="input"></param>
        /// <param name="specialSeperationChars">characters to seperate by spaces</param>
        /// <returns></returns>
        public static string Refactor(this string input, char[] specialSeperationChars) {
            StringBuilder refactoredString = new StringBuilder();
            Regex alphanumericCheck = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);

            for (int i = 0; i < input.Length; i++)
                if (!specialSeperationChars.Contains(input[i])) {
                    if (!alphanumericCheck.IsMatch(input[i].ToString()))
                        continue;

                    refactoredString.Append(input[i].ToString().ToLower());
                } else {
                    refactoredString.Append($" {input[i].ToString().ToLower()} ");
                }

            return refactoredString.ToString();
        }

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
                    yield return input.Substring(lastDeliminatorIndex, i - lastDeliminatorIndex);

                    lastDeliminatorIndex = i;
                }
        }

        public static List<string> CleanseEnumerableToList(this IEnumerable<string> enumerable, string[] illegalStrings) {
            List<string> output = enumerable.ToList();
            output.RemoveAll(illegalStrings.Contains);
            return output;
        }
    }
}
