using PorphyStruct.Core.Analysis;

namespace PorphyStruct.Core.Extension
{
    public static class TypeUtil
    {
        public static MacrocycleType GetAnalysisType(this MacrocycleAnalysis analysis) =>
            analysis switch
            {
                NorcorroleAnalysis => MacrocycleType.Norcorrole,
                CorroleAnalysis => MacrocycleType.Corrole,
                PorphyrinAnalysis => MacrocycleType.Porphyrin,
                PorphyceneAnalysis => MacrocycleType.Porphycene,
                CorrphyceneAnalysis => MacrocycleType.Corrphycene,
                _ => throw new InvalidOperationException()
            };
    }
}
