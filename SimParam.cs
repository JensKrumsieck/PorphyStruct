using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PorphyStruct
{
	public class SimParam
	{
		public string title { get; set; }
		public double current{ get; set; }
		public double best { get; set; }
		public double start { get; set; }


		/// <summary>
		/// Construct Save Param
		/// </summary>
		/// <param name="title">Title as shown in GridView</param>
		/// <param name="start">Startvalue</param>
		/// <param name="best">Bestvalue</param>
		/// <param name="current">Currentvalue</param>
		public SimParam(string title, double start, double best = 0, double current = 0)
		{
			this.title = title;
			this.current = current;
			this.best = best;
			this.start = start;
		}

		public SimParam(string title, object p1, double v, object p2)
		{
			this.title = title;
		}
	}
}
