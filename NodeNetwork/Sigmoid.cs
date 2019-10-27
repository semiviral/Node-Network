#region

using System;

#endregion

namespace NodeNetwork
{
    // todo: Implement sigmoid function as weight algorithm
    // might not do, I actually haven't gotten around to
    // understanding what a sigmoid function does.
    public static class Sigmoid
    {
        public static double Output(double x) => x < -45.0 ? 0.0 : x > 45.0 ? 1.0 : 1.0 / (1.0 + Math.Exp(-x));

        public static double Derivative(double x) => x * (1 - x);
    }
}
