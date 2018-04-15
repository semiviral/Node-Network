#region usings

#endregion

namespace NodeNetwork {
    public class Node<T> where T : class {
        public Node(int index, T value) {
            Value = value;
            Index = index;
        }

        public int Index { get; }
        public T Value { get; }
    }
}
