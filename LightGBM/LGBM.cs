using System;
using System.Collections.Generic;
using System.Linq;
using Scores = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<double>>>;

namespace LightGBM
{
    public static class LGBM
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns>a trained <see cref="Booster"/></returns>
        public static Booster Train(Estimator estimator, Dataset trainSet, Scores evalResult, params Dataset[] validationSets)
        {
            //var estParameters = estimator.ToDictionary(Constants.BoosterParameters);
            var estParameters = estimator.ToDictionary();
            var earlyStoppingRound = estimator.EarlyStoppingRound ;

            var paramString = estParameters.ToParamsString();
            var booster = new Booster(trainSet, paramString);

            if (validationSets.Length > 0)
            {
                for (int testNdx = 0; testNdx < validationSets.Length; testNdx++)
                {
                    // u engine.py train metodi imam i parametar valid_names, ovdje generiram validation names.
                    var testName = $"Test_{testNdx}";
                    // ovo je: booster.add_valid(valid_set, name_valid_set)
                    booster.AddValidData(validationSets[testNdx], testName);
                }
            }

            var initIteration = 0;
            var trainDataName = "training";

            List<Callback> cbs = new List<Callback>();
            cbs.Add(DefaultCallback.PrintScore());
            evalResult = new Scores();
            if (evalResult != null)
            {
                cbs.Add(DefaultCallback.StoreScore(evalResult));
            }
            if (earlyStoppingRound.HasValue)
                cbs.Add(DefaultCallback.EarlyStopping(earlyStoppingRound.Value));

            var evaluationResultList = new List<EvaluationResult>();

            // start training
            for (int iter = initIteration; iter < estimator.NumIterations; iter++)
            {
                var isFinished = booster.Update();
#if DEBUG
                ConsoleColor cc = isFinished ? ConsoleColor.Red : ConsoleColor.Green;
                Console.ForegroundColor = cc;
                Console.WriteLine($"Iter {iter} ::: IsFinished = {isFinished}");
                Console.ResetColor();
#endif
                evaluationResultList.Clear();

//if valid_sets is not None:
                if (validationSets.Length > 0)
                {
                    var evData = booster.EvaluateValidationData();
                    evaluationResultList.AddRange(evData);
                }

                var cbenv = new CallbackEnv
                {
                    Booster = booster,
                    Estimator = estimator,
                    Iteration = iter,
                    EvaluationList = evaluationResultList.ToList() 
                };
                try
                {
                    foreach (var cb in cbs)
                        cb(cbenv);
                }
                catch (EarlyStopException esex)
                {
                    booster.BestIteration = esex.BestIteration + 1;
                    evaluationResultList = esex.BestScoreList as List<EvaluationResult> ?? esex.BestScoreList.ToList();
                    break;
                }
            }

            var grp = evaluationResultList
                    .GroupBy(evr => evr.TestName)
                    .ToDictionary(
                        g => g.Key,
                        g => g
                            .ToDictionary(
                                g1 => g1.MetricName,
                                g1 => g1.Score)
                );

            booster.BestScore.Clear();
            booster.BestScore.Merge(grp);
            return booster;
        }
    }
}
