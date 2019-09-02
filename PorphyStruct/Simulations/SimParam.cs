using System;

namespace PorphyStruct.Simulations
{
    public class SimParam
    {
        public string title { get; set; }
        public double current { get; set; }
        public double best { get; set; }
        public double start { get; set; }
        public bool optimize { get; set; }


        /// <summary>
        /// Construct Save Param
        /// </summary>
        /// <param name="title">Title as shown in GridView</param>
        /// <param name="start">Startvalue</param>
        /// <param name="best">Bestvalue</param>
        /// <param name="current">Currentvalue</param>
        public SimParam(string title, double start, double best = 0, double current = 0, bool optimize = true)
        {
            this.title = title;
            this.current = current;
            this.best = best;
            this.start = start;
            this.optimize = optimize;
        }

        /// <summary>
        /// calculates the absolute sum
        /// </summary>
        /// <param name="d"></param>
        public static double AbsSum(double[] d)
        {
            double sum = 0;
            foreach (double i in d)
            {
                sum += Math.Abs(i);
            }
            return sum;
        }

    }
}
