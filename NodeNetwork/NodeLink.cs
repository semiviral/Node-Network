#region

using System.Text.Json.Serialization;

#endregion

namespace NodeNetwork
{
    public class NodeLink<T> where T : class
    {
        public NodeLink(T input, T output)
        {
            Input = input;
            Output = output;
        }

        public T Input { get; }
        public T Output { get; }

        [JsonIgnore]
        public double Weight => (Successes / Iterations) * Successes;

        public int Iterations { get; private set; } = 1;
        public int Successes { get; private set; } = 1;

        public void ProbabilityPass(T output)
        {
            Successes += output.Equals(Output) ? 1 : 0;

            Iterations++;
        }
    }
}
