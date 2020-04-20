using PorphyStruct.Chemistry;
using PorphyStruct.OxyPlotOverride;
using PorphyStruct.Simulations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PorphyStruct.ViewModel
{
    public class SimViewModel : AbstractViewModel
    {
        public SimViewModel(Macrocycle Cycle)
        {
            Model = new StandardPlotModel();
            this.Cycle = Cycle;
            Cycle.Paint(Model);
            Model.Scale(Model.xAxis);
            Parameters = SimParam.ListParameters(Cycle.type);
            Mode = SimulationMode.Simplex;

            //default only fitting data
            TargetData = true;
            KeepBest = true;
        }

        /// <summary>
        /// PlotModel
        /// </summary>
        public StandardPlotModel Model { get => Get<StandardPlotModel>(); set => Set(value); }

        /// <summary>
        /// Macrocycle
        /// </summary>
        public Macrocycle Cycle { get => Get<Macrocycle>(); set => Set(value); }

        /// <summary>
        /// Simulation Parameters
        /// </summary>
        public List<SimParam> Parameters { get => Get<List<SimParam>>(); set => Set(value); }

        /// <summary>
        /// Calculate only with start values
        /// </summary>
        public bool FirstOnly { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Simulationtargets
        /// </summary>
        public bool TargetData { get => Get<bool>(); set => Set(value); }
        public bool TargetIntegral { get => Get<bool>(); set => Set(value); }
        public bool TargetDerivative { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Indicates if a sim is running
        /// </summary>
        public bool IsRunning { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Keep Best as Start Values?
        /// </summary>
        public bool KeepBest { get => Get<bool>(); set => Set(value); }

        /// <summary>
        /// Best Result,yet
        /// </summary>
        public Result Best { get => Get<Result>(); set => Set(value); }

        /// <summary>
        /// Current Result
        /// </summary>
        public Result Current { get => Get<Result>(); set => Set(value); }

        /// <summary>
        /// Conversion from Conformation to DataPoints
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public List<AtomDataPoint> ConformationToData(Result result) => Cycle.DataPoints.OrderBy(s => s.X)
            .Select(s => new AtomDataPoint(s.X, result.Conformation[Cycle.DataPoints.IndexOf(s)], s.atom)).ToList();


        /// <summary>
        /// Simulation Parameters
        /// </summary>
        private Simplex simplex = null; //simplex matrix

        //error array
        private double[] currentErr = new double[] { double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity };

        //Simulation Mode Setter
        public enum SimulationMode { MonteCarlo, Simplex };
        public SimulationMode Mode { get => Get<SimulationMode>(); set => Set(value); }

        /// <summary>
        /// Do the Simulation
        /// </summary>
        private void Simulate()
        {
            //update param
            double[] coeff = Parameters.Select(p => p.Start).ToArray();
            var indices = new List<int>();
            simplex = new Simplex(Result.Calculate, coeff, Cycle);
            //set start values
            for (int i = 0; i < Parameters.Count; i++) if (!Parameters[i].Optimize) indices.Add(i);

            while (IsRunning)
            {
                //get result
                Result result;
                if (FirstOnly) result = Result.Calculate(Cycle, coeff);
                else
                {
                    if (Mode == SimulationMode.MonteCarlo)
                    {
                        var mc = new MonteCarlo(Result.Calculate, coeff, Cycle) { Indices = indices };
                        result = mc.Next();
                    }
                    else //simplex
                    {
                        simplex.Parameters = coeff;
                        simplex.Indices = indices;
                        result = simplex.Next();
                    }
                }

                for (int i = 0; i < result.Coefficients.Length; i++) Parameters[i].Current = result.Coefficients[i];

                Current = result;

                //get new best values if every single error is smaller or the overall sum is smaller
                if (IsNewBest(result))
                {
                    Best = result;
                    currentErr = result.Error;
                    //new bestvalues
                    for (int i = 0; i < result.Coefficients.ToArray().Count(); i++) Parameters[i].Best = result.Coefficients.ToArray()[i];
                }
                //if cb is set return...
                if (FirstOnly) StopSimulation();
            }
        }

        /// <summary>
        /// Checks Errors and returns a boolean whether the result is the new best result
        /// </summary>
        /// <param name="result">Result Object (for getting current errors)</param>
        /// <param name="ls">LeastSquares Object (for getting current minima)</param>
        /// <returns></returns>
        private bool IsNewBest(Result result)
        {
            //set error vars
            double error = result.Error[0];
            double derErr = result.Error[1];
            double intErr = result.Error[2];

            //check targets
            var errorTargets = new List<(double, double)>();
            if (TargetData) errorTargets.Add((error, currentErr[0]));
            if (TargetDerivative) errorTargets.Add((derErr, currentErr[1]));
            if (TargetIntegral) errorTargets.Add((intErr, currentErr[2]));

            double targetSum = 0, lsSum = 0;
            bool[] isBest = new bool[errorTargets.Count];
            for (int i = 0; i < errorTargets.Count; i++)
            {
                targetSum += errorTargets[i].Item1;
                lsSum += errorTargets[i].Item2;
                if (errorTargets[i].Item1 < errorTargets[i].Item2) isBest[i] = true;
                else isBest[i] = false;
            }

            if (isBest.All(x => x)) return true;
            //now check the sum! if sum of targets is smaller -> return true
            //if not, do nothing
            if (targetSum < lsSum) return true;
            //return false as default
            return false;
        }

        /// <summary>
        /// Starts Simulation
        /// </summary>
        public async void StartSimulation()
        {
            IsRunning = true;
            await Task.Run(() => Simulate());
        }

        /// <summary>
        /// Handle Simulation has ended
        /// </summary>
        public void StopSimulation()
        {
            IsRunning = false;
            simplex = null;
        }
    }
}
