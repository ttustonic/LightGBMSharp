using System;
using System.Linq;
using System.Collections.Generic;
using Scores = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<double>>>;

namespace LightGBMSharp
{
    public class CallbackEnv
    {
        public Booster Booster;
        public Estimator Estimator;
        public int Iteration;
        public string IterationName;
        public int BeginIteration;
        public int EndIteration;
        public List<EvaluationResult> EvaluationList;
    }

    public delegate void Callback(CallbackEnv env);

    static class DefaultCallback
    {
        public static Callback PrintScore(int period=1)
        {
            Callback cb = (env) =>
            {
                if (period > 0 && env.EvaluationList != null && (env.Iteration + 1) % period == 0)
                {
                    var res = String.Join(" ::: ",
                        env.EvaluationList.Select(evr => $"{evr.TestName} - {evr.MetricName} - {evr.Score} - {evr.BiggerIsBetter}"));
                    Console.WriteLine($"[{env.Iteration+1}] - {res}");
                }
            };
            return cb;
        }

        public static Callback StoreScore(Scores scoreDict)
        {
            scoreDict.Clear();

            Callback cb = (env) =>
            {
                if (scoreDict != null)
                {
                    var grp = env.EvaluationList.ToScoreDictionary();
                    //var grp = env.EvaluationList
                    //    .GroupBy(evr => evr.TestName)
                    //    .ToDictionary(
                    //        g => g.Key,
                    //        g => g.GroupBy(evr => evr.MetricName)
                    //             .ToDictionary(
                    //                g1 => g1.Key, 
                    //                g1 => g1.Select(evr => evr.Score).ToList() ) );
                    scoreDict.Merge(grp);
                    //foreach (var tpl in env.EvaluationList)
                    //{
                    //    var nameDict = evalResult.SetDefault(tpl.TestName, new Dictionary<string, List<double>>());
                    //    var evalName = tpl.MetricName;
                    //    var result = tpl.Score;
                    //    var resList = nameDict.SetDefault(evalName, new List<double>());
                    //    resList.Add(result);
                    //}
                }
            };
            return cb;
        }

        public static Callback EarlyStopping(int stoppingRounds, bool verbose=true)
        {
            //best score, best iter, best score list
            ValueTuple<double, 
                int,
                List<EvaluationResult>,
                Func<double, double, bool>
                >[] bestResult = null;

            Action<CallbackEnv> Init = (env) =>
            {
                if (env.EvaluationList == null)
                    throw new ArgumentException("For early stopping, at least one dataset and eval metric is required for evaluation");
                if (verbose)
                    Console.WriteLine($"Training until validation scores don't improve for {stoppingRounds} rounds.");
                if (bestResult == null)
                {
                    var numMetrics = env.Booster.EvaluationNames.Length;
                    bestResult = new ValueTuple<
                        double, 
                        int, 
                        List<EvaluationResult>,
                        Func<double, double, bool>
                        >[numMetrics];

                    for (int i=0; i<env.EvaluationList.Count; i++)
                    {
                        var evr = env.EvaluationList[i];
                        var bestIter = 0;
                        var bestScoreList = (List<EvaluationResult>)null;
                        double bestScore;
                        Func<double, double, bool> comp ;
                        if (evr.BiggerIsBetter)
                        {
                            bestScore = Double.NegativeInfinity;
                            comp = (a, b) => a > b;
                        }
                        else
                        {
                            bestScore = Double.PositiveInfinity ;
                            comp = (a, b) => a < b;
                        }
                        bestResult[i] = ValueTuple.Create(bestScore, bestIter, bestScoreList, comp);
                    }
                }
            };

            Callback cb = (env) =>
            {
                if (bestResult == null)
                    Init(env);

                for (int i=0; i<env.EvaluationList.Count; i++)
                {
                    var score = env.EvaluationList[i].Score;
                    if (bestResult[i].Item4(score, bestResult[i].Item1))
                    {
                        bestResult[i].Item1 = score;
                        bestResult[i].Item2 = env.Iteration;
                        bestResult[i].Item3 = env.EvaluationList;
                    }
                    else if (env.Iteration - bestResult[i].Item2 >= stoppingRounds)
                    {
                        var bestIter = bestResult[i].Item2;
                        var bestScoreList = bestResult[i].Item3;
                        var res = String.Join(" ::: ",
                            bestScoreList
                                .Select(evr => $"{evr.TestName} - {evr.MetricName} - {evr.Score} - {evr.BiggerIsBetter}"));
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"[{bestIter + 1}] - {res}");
                        Console.ResetColor();

                        throw new EarlyStopException(bestIter, bestScoreList);
                    }
                }
            };

            return cb;
        }

        static Scores ToScoreDictionary(this IReadOnlyList<EvaluationResult> evaluationList)
        {
            var grp = evaluationList
                .GroupBy(evr => evr.TestName)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(evr => evr.MetricName)
                         .ToDictionary(
                            g1 => g1.Key,
                            g1 => g1.Select(evr => evr.Score).ToList()));
            return grp;
        }

    }
}
