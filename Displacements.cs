using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace PorphyStruct
{
	/// <summary>
	/// Library of Displacements
	/// </summary>
	public static class Displacements
	{
		#region Corrole
		public static Vector<double> DomingCorrole = DenseVector.OfArray(new double[]
		{
			0.38203,
			-0.65065,
			1,
			-0.71393,
			0.22354,
			-0.07988,
			0.12971,
			-0.60693,
			0.66343,
			-0.55814,
			0.18709,
			0.04992,
			0.18706,
			-0.55814,
			0.66343,
			-0.60693,
			0.12971,
			-0.07988,
			0.22354,
			-0.71393,
			1,
			-0.65065,
			0.38203
		});

		public static Vector<double> RufflingCorrole = DenseVector.OfArray(new double[]
		{
			0.73926,
			0.45275,
			0.35428,
			-0.28356,
			-0.37014,
			-1,
			-0.69109,
			-0.29923,
			-0.29567,
			0.49943,
			0.44473,
			0.89631,
			0.44473,
			0.49943,
			-0.29567,
			-0.29923,
			-0.69109,
			-1,
			-0.37014,
			-0.28356,
			0.35428,
			0.45275,
			0.73926
		});

		public static Vector<double> SaddlingCorrole = DenseVector.OfArray(new double[]
		{
			-0.19859,
			-0.83491,
			-0.04726,
			-1,
			-0.45954,
			-0.19999,
			0.28171,
			0.82307,
			0.11432,
			0.8928,
			0.39031,
			0,
			-0.39031,
			-0.8928,
			-0.11432,
			-0.82307,
			-0.28171,
			0.19999,
			0.45954,
			1,
			0.04726,
			0.83491,
			0.19859
		});

		public static Vector<double> Waving2aCorrole = DenseVector.OfArray(new double[]
		{
			0.46953,
			-0.53146,
			0.93903,
			-0.63926,
			0.2237,
			0.07323,
			0.083,
			1,
			-0.91384,
			0.45216,
			-0.72288,
			-0.86997,
			-0.72288,
			0.45216,
			-0.91384,
			1,
			0.083,
			0.07323,
			0.2237,
			-0.63926,
			0.93903,
			-0.53146,
			0.46953
		});

		public static Vector<double> Waving2bCorrole = DenseVector.OfArray(new double[]
		{
			-0.13081,
			0.83957,
			-1,
			0.51263,
			-0.55919,
			-0.44234,
			-0.45251,
			0.50534,
			-0.79094,
			0.74025,
			-0.06177,
			0,
			0.06177,
			-0.74025,
			0.79094,
			-0.50534,
			0.45251,
			0.44234,
			0.55919,
			-0.51263,
			1,
			-0.83957,
			0.13081
		});

		public static Vector<double> PropelleringCorrole = DenseVector.OfArray(new double[]
		{
			0.70391,
			1,
			0.583,
			-0.61169,
			-0.55268,
			-0.61423,
			0.67827,
			0.84078,
			0.09428,
			-0.69171,
			-0.74095,
			0,
			0.74095,
			0.69171,
			-0.09428,
			-0.84078,
			-0.67827,
			0.61423,
			0.55268,
			0.61169,
			-0.583,
			-1,
			-0,70391
		});
		#endregion
		#region Porphyrin
		public static Vector<double> DomingPorphyrin = DenseVector.OfArray(new double[]{
			0.11699,
			0.27601,
			-0.83387,
			1,
			-0.83387,
			0.27601,
			0.11699,
			0.27601,
			-0.83387,
			1,
			-0.83387,
			0.27601,
			0.11699,
			0.27601,
			-0.83387,
			1,
			-0.83387,
			0.27601,
			0.11699,
			0.27601,
			-0.83387,
			1,
			-0.83387,
			0.27601,
			0.11699,
		});

		public static Vector<double> PropelleringPorphyrin = DenseVector.OfArray(new double[] {
			0,
			0.89159,
			1,
			0,
			-0.96015,
			-0.90979,
			0,
			0.89159,
			1,
			0,
			-0.96015,
			-0.90979,
			0,
			0.89159,
			1,
			0,
			-0.96015,
			-0.90979,
			0,
			0.89159,
			1,
			0,
			-0.96015,
			-0.90979,
			0,
		});

		public static Vector<double> RufflingPorphyrin = DenseVector.OfArray(new double[] {
			1, //meso
			0.57656, //alpha
			0.39884, //Beta
			0, //N
			-0.39884, //btea
			-0.57656, //alpha
			-1, //meso
			-0.57656, //alpha
			-0.39884, //beta
			0, //N
			0.39884, //Beta
			0.57656, //alpha
			1,//meso
			0.57656,
			0.39884,
			0,
			-0.39884,
			-0.57656,
			-1,
			-0.57656,
			-0.39884,
			0,
			0.39884,
			0.57656,
			1,
		});

		public static Vector<double> SaddlingPorphyrin = DenseVector.OfArray(new double[]
		{
			0, //meso
			0.48248, //alpha
			1,//beta
			0.28256,//N
			1,//beta
			0.48248, //alpha
			0,//meso
			-0.48248,//alpha
			-1,//beta
			-0.28256, //N
			-1,//beta
			-0.48248,//alpha
			0,//meso
			0.48248,  //alpha
			1,//beta
			0.28256, //N
			1,//beta
			0.48248, //alpha
			0,//meso
			-0.48248, //alpha
			-1, //beta
			-0.28256, //N
			-1,//beta
			-0.48248, //alpha
			0, //meso
		});

		/// <summary>
		/// Waving 1 is Waving over pyrrole
		/// </summary>
		public static Vector<double> Waving1aPorphyrin = DenseVector.OfArray(new double[] {
			0.80859,
			0.19819,
			-1,//-0.97526,
			0.62314,
			-1,
			0.16605,
			0.80859,
			0.68258,
			0.54714,
			0,
			-0.54714,
			-0.68258,
			-0.80859,
			-0.16605,
			1,//0.97526,
			-0.62314,
			1,
			-0.19818,
			-0.80859,
			-0.68258,
			-0.54714,
			0,
			0.54714,
			0.68258,
			0.80859,
		});

		public static Vector<double> Waving1bPorphyrin = DenseVector.OfArray(new double[]
		{
			0.80859,
			0.68258,
			0.54714,
			0,
			-0.54714,
			-0.68258,
			-0.80859,
			-0.16605,
			1,//0.97526,
			-0.62314,
			1,
			-0.19818,
			-0.80859,
			-0.68258,
			-0.54714,
			0,
			0.54714,
			0.68258,
			0.80859,
			0.19819,
			-1,//-0.97526,
			0.62314,
			-1,
			0.16605,
			0.80859,
		});

		/// <summary>
		/// is waving over meso
		/// </summary>
		public static Vector<double> Waving2aPorphyrin = DenseVector.OfArray(new double[]
		{
			0.53259, //meso
			0, //alpha
			0.54917, //beta
			-1, //N
			-0.14371, //beta
			-0.73868, //alpha
			0, //meso
			0.73868, //alpha
			0.14371, //beta
			1, //N
			-0.54917, //beta
			0, //alpha
			-0.53259, //meso
			0, //alpha
			-0.54917, //beta
			1, //N
			0.14371, //beta
			0.73868, //alpha
			0, // meso
			-0.73868, //alpha
			-0.14371, //beta
			-1, //N
			0.54917, //beta
			0, //alpha
 			0.53259, //meso
		});

		public static Vector<double> Waving2bPorphyrin = DenseVector.OfArray(new double[] {
			0, //meso
			0.73868, //alpha
			0.14371, //beta
			1, //N
			-0.54917, //beta
			0, //alpha
			-0.53259, //meso
			0, //alpha
			-0.54917, //beta
			1, //N
			0.14371, //beta
			0.73868, //alpha
			0, // meso
			-0.73868, //alpha
			-0.14371, //beta
			-1, //N
			0.54917, //beta
			0, //alpha
 			0.53259, //meso
			0, //alpha
			0.54917, //beta
			-1, //N
			-0.14371, //beta
			-0.73868, //alpha
			0, //meso
		});
		#endregion
		#region Norcorrole
		public static Vector<double> DomingNorcorrole = DenseVector.OfArray(new double[]
		{
			0.3081, //alpha
			-0.75333, //beta
			1, //N
			-0.77283, //beta
			0.2425, //alpha
			0, //meso
			0.2425, //alpha
			-0.77283,
			1,
			-0.75333,
			0.3081,
			0.3081,
			-0.75333,
			1,
			-0.77283,
			0.2425,
			0,
			0.2425,
			-0.77283,
			1,
			-0.75333,
			0.3081,
		});

		public static Vector<double> PropelleringNorcorrole = DenseVector.OfArray(new double[]{
			0.85898,
			1,//beta
			0.58807,
			-0.81876,
			-0.58939,
			0, //meso
			0.58939,
			0.81876,
			-0.58807,
			-1,//beta
			-0.85898,
			0.85898,
			1,//beta
			0.58807,
			-0.85898,
			-0.58939,
			0, //meso
			0.58939,
			0.81876,
			-0.58807,
			-1,//beta
			-0.85898,
		});

		public static Vector<double> RufflingNorcorrole = DenseVector.OfArray(new double[] {
			0.67216, //alpha
			0.53794, //beta
			0.11188, //N
			-0.28996, //beta
			-0.5307, //alpha
			-1, //meso
			-0.5307,
			-0.28996,
			0.11188,
			0.53794,
			0.67216,
			0.67216,
			0.53794,
			0.11188,
			-0.28996,
			-0.5307,
			-1,
			-0.5307,
			-0.28996,
			0.11188,
			0.53794,
			0.67216,
		});

		public static Vector<double> SaddlingNorcorrole = DenseVector.OfArray(new double[] {
			-0.21379,//alpha
			-0.87992,//beta
			-0.01881,//N
			-1, //beta
			-0.3932, //alpha
			0, //meso
			0.3932, //alpha
			1, //beta
			-0.01881, //N
			0.87992, //beta
			0.21379,//alpha
			-0.21379, //alpha
			-0.87992,//beta
			0.01881, //N
			-1,//beta
			-0.3932, //alpha
			0, //meso
			0.3932, //alpha
			1, //beta
			0.01881, //N
			0.87992, //beta
			0.21379, //alpha
		});

		public static Vector<double> Waving2aNorcorrole = DenseVector.OfArray(new double[] {
			0.55122, //alpha
			-0.43869, //beta
			1, //N
			-0.58813, //beta
			0.23523, //alpha
			0, //meso
			-0.23523, //alpha
			0.58813,
			-1,
			0.43869,
			-0.55122,
			-0.55122,
			0.43869,
			-1,
			0.58813,
			-0.23523,
			0,
			0.23523,
			-0.58813,
			1,
			-0.43869,
			0.55112,
		});

		public static Vector<double> Waving2bNorcorrole = DenseVector.OfArray(new double[] {
			-0.15542, //alpha
			0.74422, //beta
			-1, //N
			0.51472, //beta
			-0.48887, //alpha
			-0.3027, //meso
			-0.48887, //alpha
			0.51472, //beta
			-1, //N
			0.74422, //Beta
			-0.15542, //alpha
			0.15542, //alpha
			-0.74422, //beta
			1, //N
			-0.51472, //beta
			0.48887, //alpha
			0.3027, //meso
			0.48887, //alpha
			-0.51472, //beta
			1, //N
			-0.74422, //beta
			0.15542, //alpha
		});
		#endregion
		#region Corrphycene
		public static Vector<double> DomingCorrphycene = DenseVector.OfArray(new double[] 
		{
			0.70504,
			-0.24982,
			1,
			-0.63803,
			0.0701,
			-0.52053,
			-0.36194,
			-1,
			0.40178,
			-0.51377,
			0.31346,
			0.7981,
			0.7981,
			0.31346,
			-0.51377,
			0.40178,
			-1,
			-0.36194,
			-0.52054,
			0.0701,
			-0.63803,
			1,
			-0.24982,
			0.70504,
		});

		public static Vector<double> PropelleringCorrphycene = DenseVector.OfArray(new double[]
		{
			0.7104,
			1,
			0.56875,
			-0.46266,
			-0.56892,
			-0.86748,
			0.50552,
			0.64878,
			0.45018,
			-0.29435,
			-0.36464,
			-0.54747,
			0.54747,
			0.36464,
			0.29435,
			-0.45018,
			-0.64876,
			-0.50552,
			0.86748,
			0.56892,
			0.46266,
			-0.56753,
			-1,
			-0.7104,
		});

		public static Vector<double> RufflingCorrphycene = DenseVector.OfArray(new double[]
		{
			0.31609,
			0.71863,
			-0.27832,
			0.26823,
			-0.332,
			-0.72518,
			-0.6569,
			-0.48739,
			-0.27448,
			0.16427,
			0.28686,
			1,
			1,
			0.28686,
			0.16427,
			-0.27448,
			-0.48739,
			-0.6569,
			-0.72518,
			-0.332,
			0.26823,
			-0.27832,
			0.71863,
			0.31609,
		});

		public static Vector<double> SaddlingCorrphycene = DenseVector.OfArray(new double[]
		{
			0.2647,
			0.62704,
			0.39226,
			0.89941,
			0.67447,
			0.39516,
			-0.12484,
			-0.6982,
			-0.12231,
			-1,
			-0.55587,
			-0.31599,
			0.31599,
			0.55587,
			1,
			0.12231,
			0.6982,
			0.12484,
			-0.39516,
			-0.67447,
			-0.89941,
			-0.39226,
			-0.62704,
			-0.2647,
		});

		public static Vector<double> Waving2aCorrphycene = DenseVector.OfArray(new double[]
		{
			0.15512,
			0.14282,
			0.24778,
			0.02016,
			0.05035,
			-0.35208,
			0,
			-0.81231,
			1,
			0,
			0.88845,
			0.56181,
			-0.56181,
			-0.88845,
			0,
			-1,
			0.81231,
			0,
			0.35308,
			-0.05035,
			-0.02016,
			-0.24778,
			-0.14281,
			-0.15512,
		});

		public static Vector<double> Waving2bCorrphycene = DenseVector.OfArray(new double[] 
		{
			1,
			-0.1116,
			1,
			-0.91318,
			-0.20798,
			-0.62426,
			-0.28004,
			0.76291,
			-0.74927,
			0.89551,
			-0.18843,
			-0.58266,
			-0.58266,
			-0.18843,
			0.89551,
			-0.74927,
			0.76291,
			-0.28004,
			-0.62427,
			-0.20798,
			-0.91318,
			1,
			-0.1116,
			1,
		});

		#endregion
		#region Porphycene
		public static Vector<double> DomingPorphycene = DenseVector.OfArray(new double[]
		{
			0.07747,
			-1,
			0.93138,
			-0.77746,
			0.35054,
			0.4178,
			0.4178,
			0.35054,
			-0.77746,
			0.93138,
			-1,
			0.07777,
			0.07777,
			-1,
			0.93138,
			-0.77746,
			0.35054,
			0.4178,
			0.4178,
			0.35054,
			-0.77746,
			0.93138,
			-1,
			0.07747,
		});

		public static Vector<double> PropelleringPorphycene = DenseVector.OfArray(new double[] 
		{
			0.69992,
			1,
			0.56078,
			-0.41032,
			-0.54621,
			-0.75708,
			0.75708,
			0.54559,
			0.41032,
			-0.56078,
			-1,
			-0.69953,
			0.69953,
			1,
			0.56078,
			-0.410321,
			-0.546559,
			-0.75708,
			0.75708,
			0.54621,
			0.41032,
			-0.56078,
			-1,
			-0.69991,
		});

		public static Vector<double> RufflingPorphycene = DenseVector.OfArray(new double[] 
		{
			0.65768,
			0.4904,
			0.27937,
			-0.14495,
			-0.28252,
			-1,
			-1,
			-0.28252,
			-0.14495,
			0.27937,
			0.4904,
			0.65768,
			0.65768,
			0.4904,
			0.27937,
			-0.14495,
			-0.28253,
			-1,
			-1,
			-0.27937,
			-0.14495,
			0.27937,
			0.4904,
			0.65768,
		});

		public static Vector<double> SaddlingPorphycene = DenseVector.OfArray(new double[]
		{
			-0.21012,
			-0.74728,
			-0.16332,
			-1,
			-0.55824,
			-0.30699,
			0.30711,
			0.55824,
			1,
			0.16332,
			0.74709,
			0.21012,
			-0.21012,
			-0.74709,
			-0.16332,
			-1,
			-0.55824,
			-0.30699,
			0.30711,
			0.55824,
			1,
			0.16332,
			0.74709,
			0.21012,
		});

		public static Vector<double> Waving2aPorphycene = DenseVector.OfArray(new double[]
		{
			-0.04995,
			-1,
			0.61864,
			-0.822,
			0.24752,
			0.72571,
			0.72571,
			0.24751,
			-0.822,
			0.61864,
			-1,
			-0.04995,
			0.04995,
			1,
			-0.61864,
			0.822,
			-0.24751,
			-0.72571,
			-0.72571,
			-0.24751,
			0.822,
			-0.61864,
			1,
			0.04995,
		});

		public static Vector<double> Waving2bPorphycene = DenseVector.OfArray(new double[]
		{
			0.74468,
			-0.30999,
			1,
			-0.7536,
			0.04237,
			-0.14404,
			0.14404,
			-0.04229,
			0.7536,
			-1,
			0.30999,
			-0.7536,
			-0.7536,
			0.30999,
			1,
			0.7536,
			-0.04237,
			0.14404,
			-0.14404,
			0.04237,
			-0.7536,
			1,
			-0.30999,
			0.74468,
		});
		#endregion
	}
}
