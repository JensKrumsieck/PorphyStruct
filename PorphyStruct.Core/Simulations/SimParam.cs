using PorphyStruct.Chemistry;
using System.Collections.Generic;

namespace PorphyStruct.Simulations
{
    public class SimParam
    {
        public string Title { get; set; }
        public double Current { get; set; }
        public double Best { get; set; }
        public double Start { get; set; }
        public bool Optimize { get; set; }


        /// <summary>
        /// Construct Save Param
        /// </summary>
        /// <param name="title">Title as shown in GridView</param>
        /// <param name="start">Startvalue</param>
        /// <param name="best">Bestvalue</param>
        /// <param name="current">Currentvalue</param>
        public SimParam(string title, double start, double best = 0, double current = 0, bool optimize = true)
        {
            Title = title;
            Current = current;
            Best = best;
            Start = start;
            Optimize = optimize;
        }

        /// <summary>
        /// Builds a List of Cycle specific parameters
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<SimParam> ListParameters(Macrocycle.Type type)
        {
            List<SimParam> param = new List<SimParam>
            {
                new SimParam("Doming", 0),
                new SimParam("Ruffling", 0),
                new SimParam("Saddling", 0)
            };
            if (type == Macrocycle.Type.Corrole || type == Macrocycle.Type.Corrphycene || type == Macrocycle.Type.Porphycene)
            {
                param.Add(new SimParam("Waving 2 (X)", 0));
                param.Add(new SimParam("Waving 2 (Y)", 0));
            }
            else if (type == Macrocycle.Type.Porphyrin)
            {
                param.Add(new SimParam("Waving 1 (X)", 0));
                param.Add(new SimParam("Waving 1 (Y)", 0));
                param.Add(new SimParam("Waving 2 (X)", 0));
                param.Add(new SimParam("Waving 2 (Y)", 0));
            }
            else if (type == Macrocycle.Type.Norcorrole)
            {
                param.Add(new SimParam("Waving 2 (Dipy)", 0));
                param.Add(new SimParam("Waving 2 (Bipy)", 0));
            }
            param.Add(new SimParam("Propellering", 0));

            return param;
        }

    }
}
