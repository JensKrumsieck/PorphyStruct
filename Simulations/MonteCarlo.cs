using PorphyStruct.Chemistry;
using PorphyStruct.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Simulations
{
    class MonteCarlo : IParameterProvider
    {
        protected Random rnd;
        protected Macrocycle cycle;

        //indices which remain zero (checkbox unset)
        public List<int> Indices { get; set; }

        public MonteCarlo(Func<Macrocycle, double[], Result> function, double[] param, Macrocycle cycle)
        {
            Parameters = param;
            Function = function;
            this.cycle = cycle;
            rnd = new Random();
            Indices = new List<int>();
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
        public Result Evaluate() => Function(cycle, Parameters);

        /// <summary>
        /// provides next values
        /// </summary>
        /// <returns></returns>
        public Result Next()
        {
            //sets next parameters
            for (int i = 0; i < Parameters.Length; i++)
            {
                if (Indices.Contains(i))
                    Parameters[i] = 0;
                else
                    Parameters[i] = rnd.Next(-100, 101);
            }
            Parameters = Parameters.Normalize().ToArray();
            //evaluates
            return Evaluate();
        }
    }
}
