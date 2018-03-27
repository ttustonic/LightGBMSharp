using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using LightGBM;
using MathNet.Numerics;
using Utils;

namespace Examples
{
    static class Examples
    {
        static string _dir = @"..\..\..\res\data";

        public static void SimpleExample()
        {
            var trainFile = Path.Combine(_dir, "regression", "regression.train");
            var testFile = Path.Combine(_dir, "regression", "regression.test");

            //var testMatrix = DelimitedReader.Read<double>(testFile, false, "\t", false, CultureInfo.InvariantCulture);
            //var y_test = testMatrix.Column(0).ToArray();
            //var X_test = testMatrix.RemoveColumn(0).ToArray();

            var trainTpl = GetDataAndLabel(trainFile);
            var testTpl = GetDataAndLabel(testFile);
            var X_test = testTpl.Item1;
            var y_test = testTpl.Item2;

            var est = new Regressor()
            {
                BoostingType = "gbdt",
                Metric = new[] { Metric.l2, Metric.auc },
                NumLeaves = 31,
                LearningRate = 0.05,
                FeatureFraction = 0.9,
                BaggingFraction = 0.8,
                BaggingFreq = 5,
                Verbose = 1,
                EarlyStoppingRound = 5,
                NumIterations = 20
            };
            var estDict = est.ToDictionary();
            var dsTrain = new Dataset(trainFile);
            var dsEval = new Dataset(testFile, reference: dsTrain);

            var res =
                new Dictionary<string, Dictionary<string, List<double>>>();
            var bst = LightGBM.LGBM.Train(est, dsTrain, res, dsEval);

            Console.WriteLine("Save model...");
            bst.SaveModel("model.txt");

            Console.WriteLine("Start predicting...");
            var y_pred = bst.Predict(X_test, LGBMPredictType.PredictNormal);
            var mse = Distance.MSE(y_test, y_pred);

            var y_predS = String.Join(", ", y_pred.Take(10));
            Console.WriteLine($"Pred == {y_predS}");
            Console.WriteLine($"The rmse of prediction is: {Math.Sqrt(mse)}");
        }

        public static void SimpleExampleLoadFromString()
        {
            var trainFile = Path.Combine(_dir, "regression", "regression.train");
            var testFile = Path.Combine(_dir, "regression", "regression.test");
            var trainTpl = GetDataAndLabel(trainFile);
            var testTpl = GetDataAndLabel(testFile);

            var X_test = testTpl.Item1;
            var y_test = testTpl.Item2;

            var est = new Regressor()
            {
                BoostingType = "gbdt",
                Metric = new[] { Metric.l2, Metric.auc },
                NumLeaves = 31,
                LearningRate = 0.05,
                FeatureFraction = 0.9,
                BaggingFraction = 0.8,
                BaggingFreq = 5,
                Verbose = 1,
                EarlyStoppingRound = 5,
                NumIterations = 20
            };
            var estDict = est.ToDictionary();
            var dsTrain = new Dataset(trainFile);
            var dsEval = new Dataset(testFile, reference: dsTrain);

            var res = new Dictionary<string, Dictionary<string, List<double>>>();
            string modelString;
            using (var bst = LightGBM.LGBM.Train(est, dsTrain, res, dsEval))
            {
                Console.WriteLine("Save model...");
                bst.SaveModel("model.txt");
                modelString = bst.SaveModelToString();
            }

            using (var bst2 = Booster.LoadFromModelString(modelString))
            {
                Console.WriteLine("Start predicting...");
                var y_pred = bst2.Predict(X_test, LGBMPredictType.PredictNormal);
                var mse = Distance.MSE(y_test, y_pred);
                var y_predS = String.Join(", ", y_pred.Take(10));
                Console.WriteLine($"Pred == {y_predS}");
                Console.WriteLine($"The rmse of prediction is: {Math.Sqrt(mse)}");
            }
        }

        static Tuple<double[,], double[]> GetDataAndLabel(string file)
        {
            var dt = new DataTable();
            using (var rdr = new CsvDataReader(file, false))
            {
                var types = ArrayUtils.FillArray(typeof(double), rdr.FieldCount);
                rdr.ColumnTypes = types;
                dt.Load(rdr);
            }
            var tpl = dt.SplitDataAndLabel<double, double>(0);
            return tpl;
        }

    }
}
