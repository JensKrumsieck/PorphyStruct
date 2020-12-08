﻿using PorphyStruct.Chemistry;
using PorphyStruct.Chemistry.Data;
using PorphyStruct.OxyPlotOverride;
using PorphyStruct.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace PorphyStruct.ViewModel
{
    public class CompareViewModel : AbstractViewModel
    {

        /// <summary>
        /// The current Macrocycle
        /// </summary>
        public Macrocycle Cycle;

        /// <summary>
        /// Currently loaded & previewed Data
        /// </summary>
        public CompareData Current { get; set; } = null!;

        /// <summary>
        /// The Previews PlotModel
        /// </summary>
        public StandardPlotModel Model { get; set; } = null!;

        public CompareViewModel(Macrocycle cycle) : base() => Cycle = cycle;

        /// <summary>
        /// Delete Command
        /// </summary>
        private ICommand deleteCommand = null!;
        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null) deleteCommand = new RelayCommand(param => Delete((CompareData)param), param => true);
                return deleteCommand;
            }
        }

        /// <summary>
        /// Delete Handler
        /// </summary>
        /// <param name="param"></param>
        private void Delete(CompareData param) => Cycle.DataProviders.Remove(param);

        /// <summary>
        /// Loads Data into PlotModel
        /// </summary>
        /// <param name="path"></param>
        public void LoadData(string path)
        {
            Model = new StandardPlotModel();
            Current = new CompareData(OpenFile(path), Path.GetFileNameWithoutExtension(path));
            Cycle.Paint(Model, Current);
        }

        /// <summary>
        /// Opens .DAT File and extracts data
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static IEnumerable<AtomDataPoint> OpenFile(string path)
        {
            string file = File.ReadAllText(path);
            string[] lines = file.Split(new[] { "\n", "\r\n", "\r" }, StringSplitOptions.None);

            double[] dataX = new double[lines.Length - 1]; // first line is bs
            double[] dataY = new double[lines.Length - 1]; // first line is bs
            string[] dataA = new string[lines.Length - 1]; // first line is bs
            var index = 0;

            foreach (string line in lines.Where(s => !string.IsNullOrEmpty(s) && !s.StartsWith("A;")))
            {
                dataA[index] = line.Split(';')[0]; //atom identifier
                dataX[index] = Convert.ToDouble(line.Split(';')[1]);
                dataY[index] = Convert.ToDouble(line.Split(';')[2]);
                index++;
            }

            dataX = dataX.Where(s => s != 0).ToArray();
            //protect against y = 0
            dataY = dataY.Where(s => s != 0).ToArray().Length == dataX.Length
                ? dataY.Where(s => s != 0).ToArray()
                : dataY.Where(s => s != dataY.Last()).ToArray();

            Array.Sort(dataX.ToArray(), dataY);
            Array.Sort(dataX, dataA);
            var atoms = new List<Atom>();
            for (var i = 0; i < dataX.Length; i++)
            {
                //add datapoint with dummy atom only having identifier
                var a = new Atom(dataA[i], 0, 0, 0) { IsMacrocycle = true };
                atoms.Add(a);
                yield return new AtomDataPoint(dataX[i], dataY[i], a);
            }
        }
    }
}
