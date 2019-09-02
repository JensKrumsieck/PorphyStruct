using System;

namespace PorphyStruct.Simulations
{
    class MonteCarlo : IParameterProvider
    {
        protected Random rnd;

        public MonteCarlo(Func<double[], Result> function, double[] param)
        {
            Parameters = param;
            Function = function;

            rnd = new Random();
        }

        /// <summary>
        /// Gets or Sets the Parameters
        /// </summary>
        /// <see cref="IParameterProvider.Parameters"/>
        public double[] Parameters { get; set; }

        /// <summary>
        /// The Function
        /// </summary>
        /// <see cref="IParameterProvider.Function"/>
        public Func<double[], Result> Function { get; set; }

        /// <summary>
        /// Evaluates current data
        /// </summary>
        /// <see cref="IParameterProvider.Evaluate"/>
        /// <returns></returns>
        public Result Evaluate()
        {
            return Function(Parameters);
        }

        public Result Next()
        {
            //sets next parameters
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = rnd.Next(-100, 101);
            }
            //evaluates
            return Evaluate();
        }
    }
}
