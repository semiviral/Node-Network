using System.Diagnostics;

namespace NodeNetwork {
    public class NodeLink<T> where T : class {
        public NodeLink(Node<T> input, Node<T> output) {
            Input = input;
            Output = output;
        }

        public Node<T> Input { get; }
        public Node<T> Output { get; }

        public double Weight => Successes / Iterations * Successes;
        public int Iterations { get; private set; } = 1;
        public int Successes { get; private set; } = 1;

        public void ProbabilityPass(T output) {
            Successes += output.Equals(Output.Value) ? 1 : 0;

            Iterations++;
        }
    }
}
