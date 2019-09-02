using MathNet.Numerics.LinearAlgebra;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using PorphyStruct.Chemistry;
using PorphyStruct.Simulations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PorphyStruct
{
	/// <summary>
	/// Interaktionslogik für SimWindow.xaml
	/// </summary>
	public partial class SimWindow : Window
	{
		/// <summary>
		/// Form Parameters
		/// </summary>
		private readonly SynchronizationContext synchronizationContext;
		OxyPlot.Wpf.PlotView parentView;
		private Random rnd = new Random();
		private DateTime previousTime = DateTime.Now;

		/// <summary>
		/// Boolean Parameters
		/// </summary>
		public bool running = false; //simulation is running
		private bool firstOnly = false; //calculate only with start values
		private bool targetData = true; //error target is data
		private bool targetInt = true; //error target is integral
		private bool targetDeriv = true; //error target is derivative

		/// <summary>
		/// Simulation Parameters
		/// </summary>
		public Macrocycle cycle;
		public List<SimParam> param; //list of all simulation parameters
		private double[][] simplex = null; //simplex matrix
		private double[] lastCurrent = null;
		private double[] err = null;
		private double[] intErr = null;
		private double[] derErr = null;
		int[] indices = null;

		//Simulation Mode Setter
		private enum SimulationMode { MonteCarlo, Simplex };
		private SimulationMode type = SimulationMode.MonteCarlo;

		/// <summary>
		/// Constructs the Window
		/// </summary>
		/// <param name="cycle">Macrocycle to simulate</param>
		/// <param name="pv">Plotview to Plot after Simulation</param>
		public SimWindow(Macrocycle cycle, OxyPlot.Wpf.PlotView pv)
		{
			InitializeComponent();
			synchronizationContext = SynchronizationContext.Current;
			this.cycle = cycle;
			this.parentView = pv;
			param = new List<SimParam>
			{
				new SimParam("Doming", 0),
				new SimParam("Ruffling", 0),
				new SimParam("Saddling", 0)
			};
			if (cycle.type == Macrocycle.Type.Corrole || cycle.type == Macrocycle.Type.Corrphycene || cycle.type == Macrocycle.Type.Porphycene)
			{
				param.Add(new SimParam("Waving 2 (X)", 0));
				param.Add(new SimParam("Waving 2 (Y)", 0));
			}
			else if(cycle.type == Macrocycle.Type.Porphyrin)
			{
				param.Add(new SimParam("Waving 1 (X)", 0));
				param.Add(new SimParam("Waving 1 (Y)", 0));
				param.Add(new SimParam("Waving 2 (X)", 0));
				param.Add(new SimParam("Waving 2 (Y)", 0));
			}
			else if(cycle.type == Macrocycle.Type.Norcorrole)
			{
				param.Add(new SimParam("Waving 2 (Dipy)", 0));
				param.Add(new SimParam("Waving 2 (Bipy)", 0));
			}
			param.Add(new SimParam("Propellering", 0));

			simGrid.ItemsSource = this.param;
			PlotExp();
		}

		/// <summary>
		/// Plots the experimental data
		/// basically a copy from MainWindow
		/// <seealso cref="MainWindow.Analyze"/>
		/// </summary>
		private void PlotExp()
		{
            //plot that shit
            Oxy.Override.StandardPlotModel pm = new Oxy.Override.StandardPlotModel();
            LinearAxis x = pm.xAxis;
            Oxy.Override.LinearAxis y = pm.yAxis;

            ScatterSeries series = new ScatterSeries
			{
				MarkerType = Properties.Settings.Default.markerType,
				ItemsSource = cycle.dataPoints
			};

            pm.ScaleX(cycle.dataPoints);

            //add de color axis
            RangeColorAxis xR = cycle.buildColorAxis();
			pm.Axes.Add(xR);
			pm.Series.Add(series);

			simView.Model = pm;
			pm.InvalidatePlot(true);

			foreach (OxyPlot.Annotations.ArrowAnnotation a in cycle.DrawBonds())
			{
				pm.Annotations.Add(a);
			}


			if (Properties.Settings.Default.zero)
			{
				//show zero
				OxyPlot.Annotations.LineAnnotation zero = new OxyPlot.Annotations.LineAnnotation()
				{
					Color = OxyColor.FromAColor(40, OxyColors.Gray),
					StrokeThickness = Properties.Settings.Default.lineThickness,
					Intercept = 0,
					Slope = 0
				};
				pm.Annotations.Add(zero);
			}
			pm.InvalidatePlot(true);
		}

		/// <summary>
		/// Do the Simulation
		/// </summary>
		private void Simulate()
		{
			this.param = (List<SimParam>)simGrid.ItemsSource;
			double[] coeff = new double[param.Count()];
			for (int i = 0; i < param.Count; i++)
			{
				coeff[i] = param[i].start;
			}
			LeastSquares ls = new LeastSquares(param, cycle);
			while (running)
			{
				//simulate vector
				double sum = 0;
				foreach (double i in coeff)
				{
					sum += Math.Abs(i);
				}
				Tuple<Vector<double>, Result> Result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(coeff) / sum);
				for (int i = 0; i < coeff.Length; i++)
				{
					this.param[i].current = coeff[i] / sum;

				}
				if ((DateTime.Now - previousTime).Milliseconds >= 500)
				{
					Macrocycle currentConf = new Macrocycle(cycle.Atoms)
					{
						dataPoints = cycle.dataPoints.OrderBy(s => s.X).ToList()
					};
					for (int i = 0; i < currentConf.dataPoints.Count; i++)
					{
						//write new y data
						currentConf.dataPoints[i] = new AtomDataPoint(currentConf.dataPoints[i].X, Result.Item1[i], currentConf.dataPoints[i].atom);
					}
					synchronizationContext.Post(new SendOrPostCallback(o =>
					{
						try
						{ simView.Model.Series.Remove(simView.Model.Series.FirstOrDefault(s => s.Title == "Current")); }
						catch { }
						simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = "Current", MarkerFill = OxyColors.PaleVioletRed, MarkerType = Properties.Settings.Default.simMarkerType });
						simView.InvalidatePlot();
						simGrid.Items.Refresh();
					}), currentConf.dataPoints);

					previousTime = DateTime.Now;
				}

				//get new best values if every single error is smaller or the overall sum is smaller
				if (Result.Item2 != null && IsNewBest(Result.Item2, ls))
				{
					ls.currentMin = Result.Item2.Error[0];
					ls.currentDeriv = Result.Item2.Error[1];
					ls.currentIntegral = Result.Item2.Error[2];

					//new bestvalues

					for (int i = 0; i < Result.Item2.Coefficients.ToArray().Count(); i++)
					{
						this.param[i].best = Result.Item2.Coefficients.ToArray()[i];

					}
					double[] err = Result.Item2.Error;
					synchronizationContext.Post(new SendOrPostCallback(o =>
					{
						double[] error = (double[])o;
						ErrTB.Text = error[0].ToString("N6", System.Globalization.CultureInfo.InvariantCulture) + ";" + error[1].ToString("N6", System.Globalization.CultureInfo.InvariantCulture) + ";" + error[2].ToString("N6", System.Globalization.CultureInfo.InvariantCulture);
						simGrid.Items.Refresh();
					}), err);

					Macrocycle bestConf = new Macrocycle(cycle.Atoms)
					{
						dataPoints = cycle.dataPoints.OrderBy(s => s.X).ToList()
					};

					for (int i = 0; i < bestConf.dataPoints.Count; i++)
					{
						//write new y data
						bestConf.dataPoints[i] = new AtomDataPoint(bestConf.dataPoints[i].X, Result.Item1[i], bestConf.dataPoints[i].atom);
					}
					synchronizationContext.Post(new SendOrPostCallback(o =>
					{
						try
						{ simView.Model.Series.Remove(simView.Model.Series.FirstOrDefault(s => s.Title == "Best")); }
						catch { }
						simView.Model.Series.Add(new ScatterSeries() { ItemsSource = (List<AtomDataPoint>)o, Title = "Best", MarkerFill = OxyColors.LawnGreen, MarkerType = Properties.Settings.Default.simMarkerType });
						simView.InvalidatePlot();

                        //update meadDisplacement BUT! Denormalize before!
                        double fac = Application.Current.Windows.OfType<MainWindow>().First().normFac;
                        List<AtomDataPoint> tmp = new List<AtomDataPoint>();
                        foreach (AtomDataPoint dp in bestConf.dataPoints)
                        {
                            tmp.Add(new AtomDataPoint(dp.X, dp.Y * fac, dp.atom));
                        }
                        Macrocycle tmpC = new Macrocycle(cycle.Atoms)
                        {
                            dataPoints = tmp
                        };
                        meanDisPar.Content = tmpC.MeanDisplacement().ToString("N6", System.Globalization.CultureInfo.InvariantCulture);

                    }), bestConf.dataPoints);

				}
				//if cb is set return...
				if (firstOnly)
				{
					synchronizationContext.Post(new SendOrPostCallback(o =>
					{
						running = false;
						startBtn.IsEnabled = true;
						stopBtn.IsEnabled = false;
						firstOnlyCB.IsEnabled = true;
						keepBestCB.IsEnabled = true;
						finishSimBtn.IsEnabled = true;
						simGrid.IsEnabled = true;
						fitDataCB.IsEnabled = true;
						fitIntCB.IsEnabled = true;
						fitDerivCB.IsEnabled = true;
					}), null);
				}
				else
				{
					if (type == SimulationMode.MonteCarlo)
						Parallel.Invoke(() => coeff = MonteCarlo());
					if (type == SimulationMode.Simplex)
						Parallel.Invoke(() => coeff = Simplex(coeff));
				}
			}

		}
		/// <summary>
		/// Checks Errors and returns a boolean whether the result is the new best result
		/// </summary>
		/// <param name="result">Result Object (for getting current errors)</param>
		/// <param name="ls">LeastSquares Object (for getting current minima)</param>
		/// <returns></returns>
		public bool IsNewBest(Result result, LeastSquares ls)
		{
			//set error vars
			double error = result.Error[0];
			double derErr = result.Error[1];
			double intErr = result.Error[2];

			//check targets
			List<Tuple<double, double>> errorTargets = new List<Tuple<double, double>>();
			if (targetData)
				errorTargets.Add(new Tuple<double, double>(error, ls.currentMin));
			if (targetDeriv)
				errorTargets.Add(new Tuple<double, double>(derErr, ls.currentDeriv));
			if (targetInt)
				errorTargets.Add(new Tuple<double, double>(intErr, ls.currentIntegral));

			double targetSum = 0;
			double lsSum = 0;
			bool[] isBest = new bool[errorTargets.Count];
			for (int i = 0; i < errorTargets.Count; i++)
			{
				targetSum += errorTargets[i].Item1;
				lsSum += errorTargets[i].Item2;
				if (errorTargets[i].Item1 < errorTargets[i].Item2)
					isBest[i] = true;
				else
					isBest[i] = false;
			}
			if (isBest.All(x => x))
				return true;
			//now check the sum! if sum of targets is smaller -> return true
			//if not, do nothing
			if (targetSum < lsSum)
				return true;

			//return false as default
			return false;

		}

		/// <summary>
		/// Do a MonteCarlo Step (Random Number Generator)
		/// </summary>
		/// <returns>New Parameters</returns>
		private double[] MonteCarlo()
		{
			//get new coeff
			double[] coeff = new double[param.Count];
			for (int i = 0; i < param.Count; i++)
			{
				coeff[i] = rnd.Next(-100, 101);
			}

			return coeff;
		}

		/// <summary>
		/// Do a Simplex Minimization-Step
		/// </summary>
		/// <param name="current">Current Parameters</param>
		/// <returns>New Parameters</returns>
		private double[] Simplex(double[] current)
		{
			LeastSquares ls = new LeastSquares(param, cycle);
			Tuple<Vector<double>, Result> result;
			//set probe points
			double[] centroid;
			double[] contraction;
			double[] reflection;
			double[] expansion;
			int N = param.Count;
			double contractionErr = 0;
			double contractionIntErr = 0;
			double contractionDerErr = 0;
			double reflectionErr = 0;
			double reflectionIntErr = 0;
			double reflectionDerErr = 0;
			double expansionErr = 0;
			double expansionIntErr = 0;
			double expansionDerErr = 0;

			//step 1:
			//build initial simplex with N+1 points
			if (simplex == null)
			{
				if (lastCurrent == null)
				{
					lastCurrent = current;
				}

				indices = new int[N + 1];
				simplex = new double[N + 1][];
				err = new double[N + 1];
				intErr = new double[N + 1];
				derErr = new double[N + 1];
				//add current 
				if (LeastSquares.AbsSum(current) == 0) //add random not zero vector
				{
					current = MonteCarlo();
				}

				if (current == lastCurrent)
				{
					current = MonteCarlo();
				}
				lastCurrent = current;

				result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(current) / LeastSquares.AbsSum(current));
				simplex[0] = current;
				err[0] = result.Item2.Error[0];
				intErr[0] = result.Item2.Error[2];
				derErr[0] = result.Item2.Error[1];
				//evaluate
				for (int il = 1; il <= N; il++)
				{
					double[] rand = MonteCarlo();
					simplex[il] = rand;
				}

			}

			//evaluate
			for (int il = 1; il <= N; il++)
			{
				result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(simplex[il]) / LeastSquares.AbsSum(simplex[il]));
				err[il] = result.Item2.Error[0];
				intErr[il] = result.Item2.Error[2];
				derErr[il] = result.Item2.Error[1];
				indices[il] = il;
			}

			//step 2:
			//sort list by error
			Array.Sort(err, simplex);
			//convergence! stop algorithm
			if (err[N] - err[0] < 1e-7)
			{
				//all values are to equal, start new simplex becaus this is endless simplex
				double[] simplexEnd = simplex[1];
				simplex = null;
				//reseed rnd
				rnd = new Random();
				return simplexEnd;
			}

			//step 3:
			//make centroid (leave out last element!) c = 1/N sum_(i=0 to N-1) xi
			centroid = new double[N];

			for (int il = 0; il < N; il++)
			{
				centroid[il] = 0;
				for (int ie = 0; ie <= N; ie++)
				{
					if (ie != indices[N])
					{
						centroid[il] += simplex[ie][il] / N;
					}
				}
			}

			//step 4:
			//reflection of weakest point at centroid
			reflection = new double[N];
			for (int il = 0; il < N; il++)
			{
				reflection[il] = centroid[il] + (centroid[il] - simplex[indices[N]][il]);
			}
			result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(reflection) / LeastSquares.AbsSum(reflection));
			reflectionErr = result.Item2.Error[0];
			reflectionIntErr = result.Item2.Error[2];
			reflectionDerErr = result.Item2.Error[1];

			//step 5:
			//if reflection is best, expand and remove weakest point for best of reflection and expansion, return
			if (reflectionErr < err[0] && reflectionDerErr < derErr[0] && reflectionIntErr < intErr[0])
			{
				//expand
				expansion = new double[N];
				for (int ie = 0; ie < N; ie++)
				{
					expansion[ie] = centroid[ie] + 0.5 * (reflection[ie] - centroid[ie]);
				}
				result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(expansion) / LeastSquares.AbsSum(expansion));
				expansionErr = result.Item2.Error[0];
				expansionIntErr = result.Item2.Error[2];
				expansionDerErr = result.Item2.Error[1];

				if (expansionErr < reflectionErr && expansionDerErr < reflectionDerErr && expansionIntErr < reflectionIntErr)
				{
					simplex[indices[N]] = expansion;
				}
				else
				{
					simplex[indices[N]] = reflection;
				}
				//reorder and return best
				Array.Sort(err, simplex);
				return simplex[0];
			}

			//step 6:
			//if reflection is better than secondweakest, add reflection for weakest
			if ((reflectionErr >= err[0] && reflectionErr < err[N - 1])
				&& (reflectionDerErr >= derErr[0] && reflectionDerErr < derErr[N - 1])
				&& (reflectionIntErr >= intErr[0] && reflectionIntErr < intErr[N - 1]))
			{
				simplex[indices[N]] = reflection;
				//reorder and return best
				Array.Sort(err, simplex);
				return simplex[0];
			}

			//step 7:
			//contraction
			contraction = new double[N];
			for (int il = 0; il < N; il++)
			{
				contraction[il] = centroid[il] + 0.5 * (simplex[indices[N]][il] - centroid[il]);
			}
			result = ls.Simulate(MathNet.Numerics.LinearAlgebra.Double.DenseVector.OfArray(contraction) / LeastSquares.AbsSum(contraction));
			contractionErr = result.Item2.Error[0];
			contractionDerErr = result.Item2.Error[1];
			contractionIntErr = result.Item2.Error[2];

			if (contractionErr < err[N] && contractionDerErr < derErr[N] && contractionIntErr < intErr[N])
			{
				simplex[indices[N]] = contraction;
				//reorder and return best
				Array.Sort(err, simplex);
				return simplex[0];
			}

			//step 8:
			//shrink
			double[] best = simplex[indices[0]];
			for (int ie = 0; ie <= N; ie++)
			{
				for (int il = 0; il < N; il++)
				{
					simplex[ie][il] = best[il] + 0.5 * (simplex[ie][il] - best[il]);
				}
			}
			return simplex[0];

		}

		#region UI Interaction
		/// <summary>
		/// Handle Start Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void StartBtn_Click(object sender, RoutedEventArgs e)
		{
			this.running = true;
			simGrid.IsEnabled = false;
			startBtn.IsEnabled = false;
			stopBtn.IsEnabled = true;
			firstOnlyCB.IsEnabled = false;
			keepBestCB.IsEnabled = false;
			finishSimBtn.IsEnabled = false;
			fitDataCB.IsEnabled = false;
			fitIntCB.IsEnabled = false;
			fitDerivCB.IsEnabled = false;

			await Task.Run(() => this.Simulate());
		}
		
		/// <summary>
		/// Handle Stop Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void StopBtn_Click(object sender, RoutedEventArgs e)
		{
			this.running = false;

			//set best to start
			if (keepBestCB.IsChecked == true)
			{
				for (int i = 0; i < this.param.Count; i++)
				{
					this.param[i].start = this.param[i].best;
				}
				simGrid.Items.Refresh();
			}
			startBtn.IsEnabled = true;
			stopBtn.IsEnabled = false;
			firstOnlyCB.IsEnabled = true;
			keepBestCB.IsEnabled = true;
			simGrid.IsEnabled = true;
			finishSimBtn.IsEnabled = true;
			fitDataCB.IsEnabled = true;
			fitIntCB.IsEnabled = true;
			fitDerivCB.IsEnabled = true;
			simplex = null;
		}

		/// <summary>
		/// Handle Check&Uncheck of First Only Checkbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FirstOnlyCB_Checked(object sender, RoutedEventArgs e)
		{
			firstOnly = (firstOnlyCB.IsChecked == true ? true : false);
		}

		/// <summary>
		/// Handle Selection Changed of Mode ComboBox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ModeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			type = (SimulationMode)modeCB.SelectedIndex;
		}

		/// <summary>
		/// Handle Finalize Button Click
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FinishSimBtn_Click(object sender, RoutedEventArgs e)
		{
			string[] dontMark = Properties.Settings.Default.dontMark.Split(',');
			parentView.InvalidatePlot();
			//has simulation!
			if (simView.Model.Series.Count >= 2)
			{
				parentView.Model.Annotations.Clear();
				//save exp series
				ScatterSeries exp = (ScatterSeries)parentView.Model.Series[0];

				//clear all series and readd series
				parentView.Model.Series.Clear();
				parentView.Model.Series.Add(exp);

				ScatterSeries sim = (ScatterSeries)simView.Model.Series.FirstOrDefault(s => s.Title == "Best");
				Simulation simObj = new Simulation(cycle.Atoms)
				{
					type = cycle.type,
					dataPoints = (List<AtomDataPoint>)sim.ItemsSource
				};

				//dont mark (sim)
				for (int i = 0; i < simObj.dataPoints.Count; i++)
				{
					if (dontMark.Contains(simObj.dataPoints[i].atom.Type) || dontMark.Contains(simObj.dataPoints[i].atom.Identifier))
						simObj.dataPoints[i].Size = 0;
				}
				//export param
				foreach (SimParam p in param)
				{
					simObj.par.Add(p.title, Math.Round(p.best * 100, 1));					
				}
				//export errors
				simObj.errors = new double[]
				{
					Convert.ToDouble(ErrTB.Text.Split(';')[0], System.Globalization.CultureInfo.InvariantCulture),
					Convert.ToDouble(ErrTB.Text.Split(';')[1], System.Globalization.CultureInfo.InvariantCulture),
					Convert.ToDouble(ErrTB.Text.Split(';')[2], System.Globalization.CultureInfo.InvariantCulture),
				};

				//set true if exp has been inverted
				if (Application.Current.Windows.OfType<MainWindow>().First().invert)
					simObj.isInverted = true;
				Application.Current.Windows.OfType<MainWindow>().First().DelSimButton.IsEnabled = true;
				Application.Current.Windows.OfType<MainWindow>().First().DiffSimButton.IsEnabled = true;


				simObj.Paint(parentView.Model);
				Application.Current.Windows.OfType<MainWindow>().First().simulation = simObj;
				Application.Current.Windows.OfType<MainWindow>().First().Analyze();
			}
		}

		/// <summary>
		/// Handle fit target checkboxes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FitDataCB_Checked(object sender, RoutedEventArgs e)
		{
			targetData = (fitDataCB.IsChecked == true ? true : false);
		}
		private void FitDerivCB_Checked(object sender, RoutedEventArgs e)
		{
			targetDeriv = (fitDerivCB.IsChecked == true ? true : false);
		}
		private void FitIntCB_Checked(object sender, RoutedEventArgs e)
		{
			targetInt = (fitIntCB.IsChecked == true ? true : false);
		}

		/// <summary>
		/// Handle Window Closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			//stop sim thread on closing
			running = false;
		}
		#endregion
	}
}
