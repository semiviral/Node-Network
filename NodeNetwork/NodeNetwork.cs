#region

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace NodeNetwork
{
    public class NodeNetwork<T> where T : class
    {
        #region MEMBERS

        public readonly Dictionary<T, Dictionary<T, NodeLink<T>>> Links;

        #endregion

        public NodeNetwork() => Links = new Dictionary<T, Dictionary<T, NodeLink<T>>>();

        /// <summary>
        ///     Recieves input and abstracts to a neural blueprint
        /// </summary>
        /// <param name="inputs">
        ///     input will be abstracted in order of bottom-to-
        ///     top
        /// </param>
        public void ProcessInput(IEnumerable<T> inputs)
        {
            List<T> inputsAlloc = inputs.ToList();

            Console.Write($"Processing {inputsAlloc.Count} nodes... ");

            for (int i = 0; i < (inputsAlloc.Count - 1); i++)
            {
                T input = inputsAlloc[i];
                T output = inputsAlloc[i + 1];

                if (!Links.ContainsKey(input))
                {
                    Links.Add(input, new Dictionary<T, NodeLink<T>>());
                }

                Dictionary<T, NodeLink<T>> linksByOutput = Links[input];

                // check if output is already added, add if not
                if (!linksByOutput.ContainsKey(output))
                {
                    Links[input].Add(output, new NodeLink<T>(input, output));
                }

                foreach ((T _, NodeLink<T> link) in linksByOutput)
                {
                    if (link.Output != output)
                    {
                        link.ProbabilityPass(output);
                    }
                }
            }

            Console.WriteLine("completed.");
        }

        public T DetermineOutput(T input)
        {
            List<NodeLink<T>> links = Links[input].Select(kvp => kvp.Value).ToList();

            if (links.Count == 0)
            {
                return input;
            }

            Dictionary<T, double> outcomes = links.ToDictionary(link => link.Output, link => link.Weight);

            return outcomes.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }
}
