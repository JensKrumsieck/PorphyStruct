using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using PorphyStruct.Chemistry;

namespace PorphyStruct.Simulations
{
	/// <summary>
	/// Class used to simulate a Conformation with calculating errors
	/// </summary>
	public class LeastSquares
	{
		public List<SimParam> param;
		public List<AtomDataPoint> list;
		public Macrocycle cycle;
		public double currentMin = double.PositiveInfinity;
		public double currentDeriv = double.PositiveInfinity;
		public double currentIntegral = double.PositiveInfinity;

		/// <summary>
		/// Constructs the LeastSquares class.
		/// </summary>
		/// <param name="param">Simulation Parameters</param>
		/// <param name="cycle">The Macrocycle Object</param>
		/// <param name="currentMin">Current Minimum (data)</param>
		/// <param name="currentDeriv">Current Minimum (Deriv.)</param>
		/// <param name="currentIntegral">Current Minimum (Int.)</param>
		public LeastSquares(List<SimParam> param, Macrocycle cycle, double currentMin = double.PositiveInfinity, double currentDeriv = double.PositiveInfinity, double currentIntegral = double.PositiveInfinity)
		{
			this.cycle = cycle;
			this.list = cycle.dataPoints;
			this.param = param;
			this.currentMin = currentMin;
			this.currentDeriv = currentDeriv;
			this.currentIntegral = currentIntegral;

			//reorder by x axis to fit the sim
			this.list = list.OrderBy(s => s.X).ToList();
		}

		/// <summary>
		/// Gets a Conformation with given Parameters and does some error calculation
		/// </summary>
		/// <param name="Coeff">The Coefficients of the Simulation Parameters</param>
		/// <returns>Tuple<Vector<double>, Result></returns>
		public Tuple<Vector<double>, Result> Simulate(Vector<double> Coeff)
		{
			var M = Matrix<double>.Build;

			Matrix<double> Disp = M.Dense(1, Coeff.Count);

			//set displacementmatrix
			if (cycle.type == Macrocycle.Type.Corrole)
				Disp = M.DenseOfColumnVectors(Displacements.DomingCorrole, Displacements.RufflingCorrole, Displacements.SaddlingCorrole, Displacements.Waving2aCorrole, Displacements.Waving2bCorrole, Displacements.PropelleringCorrole);
			else if (cycle.type == Macrocycle.Type.Porphyrin)
				Disp = M.DenseOfColumnVectors(Displacements.DomingPorphyrin, Displacements.RufflingPorphyrin, Displacements.SaddlingPorphyrin, Displacements.Waving1aPorphyrin, Displacements.Waving1bPorphyrin, Displacements.Waving2aPorphyrin, Displacements.Waving2bPorphyrin, Displacements.PropelleringPorphyrin);
			else if (cycle.type == Macrocycle.Type.Norcorrole)
				Disp = M.DenseOfColumnVectors(Displacements.DomingNorcorrole, Displacements.RufflingNorcorrole, Displacements.SaddlingNorcorrole, Displacements.Waving2aNorcorrole, Displacements.Waving2bNorcorrole, Displacements.PropelleringNorcorrole);
			else if (cycle.type == Macrocycle.Type.Corrphycene)
				Disp = M.DenseOfColumnVectors(Displacements.DomingCorrphycene, Displacements.RufflingCorrphycene, Displacements.SaddlingCorrphycene, Displacements.Waving2aCorrphycene, Displacements.Waving2bCorrphycene, Displacements.PropelleringCorrphycene);
			else if (cycle.type == Macrocycle.Type.Porphycene)
				Disp = M.DenseOfColumnVectors(Displacements.DomingPorphycene, Displacements.RufflingPorphycene, Displacements.SaddlingPorphycene, Displacements.Waving2aPorphycene, Displacements.Waving2bPorphycene, Displacements.PropelleringPorphycene);

			var V = Vector<double>.Build;

			List<Vector<double>> Conformations = new List<Vector<double>>();

			Result Res = null;

			//get test conformation
			Conformations.Add(Conformation(Disp, Coeff));

			//normalize conformations
			for (int i = 0; i <	 Conformations.Count; i++)
			{
				Conformations[i] = NormalizeVector(Conformations[i]);
			}

			double error = double.PositiveInfinity;
			double derErr = double.PositiveInfinity;
			double intErr = double.PositiveInfinity;
			Vector<double> Conf = V.Dense(list.Count, 0);

			foreach (Vector<double> v in Conformations)
			{
				//check error
				if (getError(list, v) != 0)
				{
					double currErr = getError(list, v);
					double currDerErr = getError(Derive(list), Derive(GetAtomDataPoints(v)));
					double currIntErr = getError(Integrate(list), Integrate(GetAtomDataPoints(v)));
					if ((currErr < error &&  currDerErr < derErr &&  currIntErr < intErr) || currErr + currDerErr + currIntErr < error + derErr + intErr)
					{
						//is best possibility
						Conf = v;
						error = currErr;
						derErr = currDerErr;
						intErr = currIntErr;
					}

					Res = new Result(
							Conf, Coeff,
							new double[] {
							error,
							derErr,
							intErr
							});
				}
				
			}
			//return best conformation
			return new Tuple<Vector<double>, Result>(Conf, Res);

		}

		/// <summary>
		/// Normalizes a Vector
		/// </summary>
		/// <param name="v">The Vector</param>
		/// <returns>Normalized Vector</returns>
		public Vector<double> NormalizeVector(Vector<double> v)
		{
			//normalize conformation
			//find min & max
			double min = 0;
			double max = 0;
			double fac = 0;
			foreach (double d in v)
			{
				if (d < min)
					min = d;
				if (d > max)
					max = d;
			}
			if (Math.Abs(max) > Math.Abs(min))
				fac = Math.Abs(max);
			else
				fac = Math.Abs(min);
			return v / fac;
		}

		/// <summary>
		/// Converts Vector to AtomDataPoints
		/// </summary>
		/// <param name="input">Conformation Vector</param>
		/// <returns>List of AtomDataPoints</returns>
		public List<AtomDataPoint> GetAtomDataPoints(Vector<double> input){
			List<AtomDataPoint> test = new List<AtomDataPoint>();
			for (int i = 0; i < list.Count; i++)
			{
				//write new y data
				test.Add(new AtomDataPoint(list[i].X, input[i], list[i].atom));
			}
			return test;
		}

		/// <summary>
		/// gets the squared difference of two doubles
		/// </summary>
		/// <param name="x1"></param>
		/// <param name="x2"></param>
		/// <returns></returns>
        private double getError(double x1, double x2)
        {
            return Math.Pow(x1 - x2, 2);
        }

		/// <summary>
		/// get error from data points
		/// </summary>
		/// <param name="list">Current Exp. DataPoints</param>
		/// <param name="test">Tested Conformation as Vector</param>
		/// <returns>The Mean Squared Error</returns>
		private double getError(List<AtomDataPoint> list, Vector<double> test)
        {
            double error = 0;
			if (list.Count() == test.Count())
			{
				for (int i = 0; i < list.Count; i++)
				{
					error += getError(list[i].Y, test[i]);
				}
			}
            return Math.Sqrt(error)/test.Count;
        }

		/// <summary>
		/// get error for double arrayxs
		/// </summary>
		/// <param name="d1"></param>
		/// <param name="d2"></param>
		/// <returns></returns>
		private double getError(double[] d1, double[] d2)
		{
			double error = 0;
			if (d1.Count() ==  d2.Count())
			{
				for (int i = 0; i < list.Count; i++)
				{
					error += getError(d1[i], d2[i]);
				}
			}
			return Math.Sqrt(error)/d1.Count();
		}

		/// <summary>
		/// Calculate Conformation by Multiply Displacement-Matrix with Coefficient-Vector
		/// </summary>
		/// <param name="M">Matrix with all Macrocyclic Displacements</param>
		/// <param name="V">Vector with Simulation-Parameters</param>
		/// <returns>The Conformation of the simulated Macrocycle as Vector</returns>
        private Vector<double> Conformation(Matrix<double> M, Vector<double> V)
        {
            return M * V;
        }

		/// <summary>
		/// Calculate the Derivative of given DataPoints
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private double[] Derive(List<AtomDataPoint> data)
		{
			double[] derivative = new double[data.Count];
			for(int i = 0; i < data.Count; i++)
			{
				if (i != 0)
				{
					derivative[i] = (data[i].Y - data[i - 1].Y) / (data[i].X - data[i - 1].X);
				}
				else
					derivative[i] = 0;
			}
			return derivative;
		}

		/// <summary>
		/// Calculate the integral of given DataPoints
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		private double[] Integrate(List<AtomDataPoint> data)
		{
			double[] integral = new double[data.Count];
			for (int i = 0; i < data.Count; i++)
			{
				if (i != 0)
				{
					integral[i] = integral[i - 1] + data[i].Y;
				}
				else
					integral[i] = data[i].Y;
			}
			return integral;
		}

		/// <summary>
		/// The absolute vector sum
		/// </summary>
		/// <param name="vec">The Vector</param>
		/// <returns>The Sum</returns>
		public static double AbsSum(double[] vec)
		{
			double sum = 0;
			foreach (double i in vec)
			{
				sum += Math.Abs(i);
			}
			return sum;
		}

	}
}
