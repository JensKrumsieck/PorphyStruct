﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace PorphyStruct
{
    public class Result
    {
        public Vector<double> Conformation, Coefficients;
        public double[] Error;

		/// <summary>
		/// Construct Result
		/// </summary>
		/// <param name="Conformation"></param>
		/// <param name="Coefficients"></param>
		/// <param name="Error"></param>
        public Result(Vector<double> Conformation, Vector<double> Coefficients, double[] Error)
        {
            this.Conformation = Conformation; this.Coefficients = Coefficients; this.Error = Error;
        }
    }
}
