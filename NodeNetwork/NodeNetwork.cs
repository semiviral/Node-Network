#region usings

using System;
using System.Collections.Generic;
using System.Linq;

#endregion

namespace NodeNetwork {
    public class NodeNetwork<T> where T : class {
        public List<Node<T>> Nodes { get; } = new List<Node<T>>();
        public List<NodeLink<T>> Links { get; } = new List<NodeLink<T>>();

        public int MaxIndex {
            get { return Nodes.Count == 0 ? 0 : Nodes.Max(neuron => neuron.Index) + 1; }
        }

        public Node<T> GetNeuron(T value) {
            return Nodes.SingleOrDefault(neuron => neuron.Value.Equals(value));
        }

        public Node<T> GetNeuron(int index) {
            return Nodes.SingleOrDefault(neuron => neuron.Index == index);
        }

        public IEnumerable<NodeLink<T>> GetLinks(Node<T> value) {
            return Links.Where(link => link.Input.Equals(value));
        }

        public IEnumerable<NodeLink<T>> GetLinks(T value) {
            return Links.Where(link => link.Input.Value.Equals(value));
        }

        /// <summary>
        ///     Recieves input and abstracts to a neural blueprint
        /// </summary>
        /// <param name="inputs">
        ///     input will be abstracted in order of bottom-to-
        ///     top
        /// </param>
        public void ProcessInput(List<T> inputs) {
            int count = inputs.Count;

            Console.Write($"Processing nodes...\n{count}");
            int cursorFromTop = Console.CursorTop;

            foreach (T input in inputs)
                if (Nodes.All(neuron => !neuron.Value.Equals(input)))
                    Nodes.Add(new Node<T>(MaxIndex, input));

            for (int i = 0; i < count - 1; i++) {
                List<NodeLink<T>> links = GetLinks(inputs[i]).ToList();

                if (!links.Any() || !links.Any(link => link.Output.Value.Equals(inputs[i + 1])))
                    Links.Add(new NodeLink<T>(GetNeuron(inputs[i]), GetNeuron(inputs[i + 1])));

                foreach (NodeLink<T> link in links)
                    link.ProbabilityPass(inputs[i + 1]);

                Console.SetCursorPosition(0, cursorFromTop);
                Console.Write($"{count - i}");
            }


            Console.WriteLine($"Finished processing {inputs.Count} nodes.");
        }

        public T DetermineOutput(T input) {
            List<NodeLink<T>> links = new List<NodeLink<T>>(GetLinks(GetNeuron(input)));

            if (links.Count == 0)
                return input;

            Dictionary<T, double> outcomes = links.ToDictionary(link => link.Output.Value, link => link.Weight);

            return outcomes.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }
    }
}
