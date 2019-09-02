using PorphyStruct.Chemistry;
using System;

namespace PorphyStruct.Simulations
{
    class MonteCarlo : IParameterProvider
    {
        protected Random rnd;
        protected Macrocycle cycle;

        public MonteCarlo(Func<Macrocycle, double[], Result> function, double[] param, Macrocycle cycle)
        {
            Parameters = param;
            Function = function;
            this.cycle = cycle;
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
        public Func<Macrocycle, double[], Result> Function { get; set; }

        /// <summary>
        /// Evaluates current data
        /// </summary>
        /// <see cref="IParameterProvider.Evaluate"/>
        /// <returns></returns>
        public Result Evaluate()
        {
            return Function(cycle, Parameters);
        }

        public Result Next()
        {
            //sets next parameters
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] = rnd.Next(-100, 101);
            }

            //norm
            double sum = SimParam.AbsSum(Parameters);
            for (int i = 0; i < Parameters.Length; i++)
            {
                Parameters[i] /= sum;
            }
            //evaluates
            return Evaluate();
        }
    }
}
