namespace PorphyStruct.Chemistry
{
    /// <summary>
    /// wrapper for molecule with crystal parameters
    /// </summary>
    public class Crystal : Molecule
    {
        public double a;
        public double b;
        public double c;
        public double alpha;
        public double beta;
        public double gamma;

        /// <summary>
        /// Constructs a Crytal-Obj. with title
        /// </summary>
        /// <param name="title">Title of file</param>
        /// <param name="a">Crystalparameter a</param>
        /// <param name="b">Crystalparameter b</param>
        /// <param name="c">Crystalparameter c</param>
        /// <param name="alpha">Crystalparameter alpha</param>
        /// <param name="beta">Crystalparameter beta</param>
        /// <param name="gamma">Crystalparameter gamma</param>
        public Crystal(string title, double a, double b, double c, double alpha, double beta, double gamma)
            : base(title)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.alpha = alpha;
            this.beta = beta;
            this.gamma = gamma;
        }
        /// <summary>
        /// Constructs a Crytal-Obj. without title
        /// </summary>
        /// <param name="a">Crystalparameter a</param>
        /// <param name="b">Crystalparameter b</param>
        /// <param name="c">Crystalparameter c</param>
        /// <param name="alpha">Crystalparameter alpha</param>
        /// <param name="beta">Crystalparameter beta</param>
        /// <param name="gamma">Crystalparameter gamma</param>
        public Crystal(double a, double b, double c, double alpha, double beta, double gamma)
            : base()
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.alpha = alpha;
            this.beta = beta;
            this.gamma = gamma;
        }

        /// <summary>
        /// Constructs a Crystal Obj.
        /// </summary>
        public Crystal()
            : base()
        {
            //does nothing for now
        }

    }
}
