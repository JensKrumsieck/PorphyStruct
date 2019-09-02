using System;

namespace PorphyStruct.Simulations
{
    interface IParameterProvider
    {
        /// <summary>
        /// gets or sets the parameters
        /// </summary>
        double[] Parameters {get; set;}

        /// <summary>
        /// The Function to evaluate
        /// </summary>
        Func<double[], Result> Function { get; set; }

        /// <summary>
        /// returns current error
        /// </summary>
        /// <returns></returns>
        Result Evaluate();

        /// <summary>
        /// calculates next parameter set
        /// </summary>
        /// <returns></returns>
        Result Next();
    }
}
