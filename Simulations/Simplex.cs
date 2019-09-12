using PorphyStruct.Chemistry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PorphyStruct.Simulations
{
    class Simplex : IParameterProvider
    {
        protected Random rnd;
        protected Macrocycle cycle;

        /// <summary>
        /// inidcates which values remain constant
        /// </summary>
        public List<int> Indices { get; set; }

        protected double[][] simplex = null;
        protected double[] lastCurrent;
        protected int[] indices = null;
        protected int count = 0;
        protected double[] error = null;

        public Simplex(Func<Macrocycle, double[], Result> function, double[] param, Macrocycle cycle)
        {
            Parameters = param;
            Function = function;
            this.cycle = cycle;
            rnd = new Random();
            error = new double[Parameters.Length + 1];
            for (int i = 0; i < Parameters.Length; i++)
            {
                error[i] = double.PositiveInfinity;
            }
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
        public Result Evaluate()
        {
            if (SimParam.AbsSum(Parameters) != 1)
            {
                for (int i = 0; i < Parameters.Length; i++)
                {
                    Parameters[i] /= SimParam.AbsSum(Parameters);
                }
            }
            return Function(cycle, Parameters);
        }

        /// <summary>
        /// evaluate with other parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Result Evaluate(double[] parameters)
        {
            if (SimParam.AbsSum(parameters) != 1)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] /= SimParam.AbsSum(parameters);
                }
            }
            return Function(cycle, parameters);
        }

        /// <summary>
        /// generates next coefficients
        /// </summary>
        /// <returns></returns>
        public Result Next()
        {            
            //set probe points
            double[] centroid;
            double[] contraction;
            double[] reflection;
            double[] expansion;
            int N = Parameters.Length;
            Result contractionRes;
            Result reflectionRes;
            Result expansionRes;

            //Step 1: Build initial simplex
            if (simplex == null) Build(Parameters.Length);
            //evaluate
            for (int il = 0; il <= Parameters.Length; il++)
            {
                Result result = Evaluate(simplex[il]);
                error[il] = result.Error[0];
                indices[il] = il;
            }

            //step 2:
            //sort list by error
            Array.Sort(error, simplex);
            //convergence! stop algorithm
            if (error[Parameters.Length] - error[0] < 1e-8|| count == 25)
            {
                count = 0;
                //all values are to equal, start new simplex becaus this is endless simplex
                Parameters = simplex[1];
                simplex = null;
                return Evaluate();
            }

            if (lastCurrent != null && Math.Abs(lastCurrent.Sum() - error.Sum()) < 5e-8)
                count++;
            else count = 0;
            lastCurrent = (double[])error.Clone();

            //step 3:
            //make centroid (leave out last element!) c = 1/N sum_(i=0 to N-1) xi
            centroid = Centroid(Parameters.Length);

            //step 4:
            // make reflection
            reflection = Reflection(centroid);
            reflectionRes = Evaluate(reflection);

            //step 5:
            //if reflection is best, expand and remove weakest point for best of reflection and expansion, return
            if (reflectionRes.Error[0] < error[0])
            {
                expansion = Expansion(centroid, reflection);
                expansionRes = Evaluate(expansion);

                //expansion is better than relfection
                if (expansionRes.Error[0] < reflectionRes.Error[0] && expansionRes.Error[1] < reflectionRes.Error[1] && expansionRes.Error[2] < reflectionRes.Error[2])
                {
                    simplex[indices[N]] = expansion;
                }
                else
                {
                    //reflection is better
                    simplex[indices[N]] = reflection;
                }

                //reorder and return best
                Array.Sort(error, simplex);
                Parameters = simplex[0];
                return Evaluate();
            }

            //step 6:
            //if reflection is better than secondweakest, add reflection for weakest
            if (reflectionRes.Error[0] >= error[0] && reflectionRes.Error[0] < error[Parameters.Length - 1])
            {
                simplex[indices[N]] = reflection;
                //reorder and return best
                Array.Sort(error, simplex);
                Parameters = simplex[0];
                return Evaluate();
            }

            //step 7:
            //contraction
            contraction = Contraction(centroid);
            contractionRes = Evaluate(contraction);


            if (contractionRes.Error[0] < error[Parameters.Length])
            {
                simplex[indices[N]] = contraction;
                //reorder and return best
                Array.Sort(error, simplex);
                Parameters = simplex[0];
                return Evaluate();
            }
            //step 8:
            //shrink
            Shrink();
            Parameters = simplex[0];
            return Evaluate();
        }

        /// <summary>
        /// builds initial simplex
        /// </summary>
        /// <param name="N"></param>
        private void Build(int N)
        {
            //build montecarlo instance
            MonteCarlo mc = new MonteCarlo(Conformation.Calculate, Parameters, cycle)
            {
                Indices = Indices
            };
            //step 1:
            //build initial simplex with N+1 points

            indices = new int[N + 1];
            simplex = new double[N + 1][];
            //add current 
            if (SimParam.AbsSum(Parameters) == 0) //add random not zero vector
            {
                Result result = mc.Next();
                Parameters = (double[])result.Coefficients.Clone();
            }
            simplex[0] = Parameters;
            for (int il = 1; il <= N; il++)
            {
                Result result = mc.Next();
                simplex[il] = (double[])result.Coefficients.Clone();
            }

        }

        /// <summary>
        /// builds the centroid point
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        private double[] Centroid(int N)
        {
            double[] centroid = new double[N];
            for (int il = 0; il < N; il++)
            {
                centroid[il] = 0;
                for (int ie = 0; ie <= N; ie++)
                {
                    if (ie != indices[N] && !Indices.Contains(il))
                    {
                        centroid[il] += simplex[ie][il] / N;
                    }
                    else if (Indices.Contains(il))
                        centroid[il] = 0;
                }
            }
            return centroid;
        }

        /// <summary>
        /// builds the reflection point
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        private double[] Reflection(double[] centroid)
        {
            double[] reflection = new double[centroid.Length];
            for (int il = 0; il < centroid.Length; il++)
            {
                if (!Indices.Contains(il)) reflection[il] = centroid[il] + (centroid[il] - simplex[indices[centroid.Length]][il]);
                else reflection[il] = 0;
            }
            return reflection;
        }

        /// <summary>
        /// Builds expansion
        /// </summary>
        /// <param name="centroid"></param>
        /// <param name="reflection"></param>
        /// <returns></returns>
        private double[] Expansion(double[] centroid, double[] reflection)
        {
            double[] expansion = new double[centroid.Length];
            for (int ie = 0; ie < centroid.Length; ie++)
            {
                if (!Indices.Contains(ie))
                    expansion[ie] = centroid[ie] + 0.5 * (reflection[ie] - centroid[ie]);
                else expansion[ie] = 0;
            }
            return expansion;
        }

        /// <summary>
        /// Builds contraction point
        /// </summary>
        /// <param name="centroid"></param>
        /// <returns></returns>
        private double[] Contraction(double[] centroid)
        {
            double[] contraction = new double[centroid.Length];
            for (int il = 0; il < centroid.Length; il++)
            {
                if (!Indices.Contains(il))
                    contraction[il] = centroid[il] + 0.5 * (simplex[indices[centroid.Length]][il] - centroid[il]);
                else contraction[il] = 0;
            }
            return contraction;
        }

        private void Shrink()
        {
            double[] best = simplex[indices[0]];
            for (int ie = 0; ie <= Parameters.Length; ie++)
            {
                for (int il = 0; il < Parameters.Length; il++)
                {
                    if (!Indices.Contains(il)) simplex[ie][il] = best[il] + 0.5 * (simplex[ie][il] - best[il]);
                    else simplex[ie][il] = 0;
                }
            }
        }

    }
}
