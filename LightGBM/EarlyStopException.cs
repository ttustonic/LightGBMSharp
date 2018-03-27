using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightGBM
{
    public class EarlyStopException: Exception
    {
        public readonly int BestIteration;

        public readonly IReadOnlyList<EvaluationResult> BestScoreList;

        public EarlyStopException(int bestIteration, IReadOnlyList<EvaluationResult> bestScoreList): base("Early stopping")
        {
            BestIteration = bestIteration;
            BestScoreList = bestScoreList;
        }

    }
}
