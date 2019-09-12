namespace PorphyStruct.Simulations
{
    public class Result
    {
        public double[] Conformation, Coefficients;
        public double[] Error;

        /// <summary>
        /// Construct Result
        /// </summary>
        /// <param name="Conformation"></param>
        /// <param name="Coefficients"></param>
        /// <param name="Error"></param>
        public Result(double[] Conformation, double[] Coefficients, double[] Error)
        {
            this.Conformation = Conformation;
            this.Coefficients = Coefficients;
            this.Error = Error;
        }
    }
}
