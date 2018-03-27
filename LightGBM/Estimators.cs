using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static LightGBM.Constants;
using Scores = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<double>>>;

namespace LightGBM
{
    public abstract class Estimator
    {
        #region Fields and properties
        string _treeLearner; //= TreeLearnerParameters.serial ;
        string _objective;
        internal Booster _booster;
        internal Scores _evalResult;

        public Booster Booster
        {
            get
            {
                if (_booster == null)
                    throw new InvalidOperationException("Booster not found. Run Fit method.");
                return _booster;
            }
        }

        public Scores EvalResult
        {
            get
            {
                if (_evalResult == null)
                    throw new InvalidOperationException("Eval result not found. Run fit method.");
                return _evalResult;
            }
        }

        public string Task { get { return "train"; } }

        public string[] Model;

        public Objective Objective { get; set; }

        public string BoostingType { get; set; }
        
        public int? NumIterations { get; set; } 

        public double? LearningRate { get; set; }

        public int? NumLeaves { get; set; }

        public int? MaxDepth { get; set; } 

        public TreeLearner? TreeLearner { get; set; }

        // https://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c
        public int? NumThreads { get; set; } //= Environment.ProcessorCount;

        public double? HistogramPoolSize { get; set; }

        public int? MinDataInLeaf { get; set; }

        public double? MinSumHessianInLeaf { get; set; }

        public double? LambdaL1 { get; set; }

        public double? LambdaL2 { get; set; }

        public double? MinGainToSplit { get; set; }

        public double? FeatureFraction { get; set; }

        public double? FeatureFractionSeed { get; set; }

        public double? BaggingFraction { get; set; }

        public int? BaggingFreq { get; set; }

        public int? BaggingSeed { get; set; }

        public int? EarlyStoppingRound { get; set; } 

        public int? MaxBin { get; set; }

        public int? DataRandomSeed { get; set; }

        public string InitScore { get; set; } // = String.Empty ;

        public bool? IsSparse { get; set; }

        public bool? SaveBinary { get; set; }

        public int[] CategoricalFeature { get; set; }// = Array.Empty<int>();

        public bool? IsUnbalance { get; set; }

        public int? Verbose { get; set; }

        // https://github.com/Microsoft/LightGBM/blob/master/docs/Parameters.rst#metric-parameters
        Metric[] _metric = {LightGBM.Metric.l2 };
        public Metric[] Metric
        {
            get { return _metric; }
            set { _metric = value; }
        }

        public int? MetricFreq { get; set; }

        public bool? IsTrainingMetric { get; set; }

        public int[] NcdgAt { get; set; } = Array.Empty<int>();

        public int? NumMachines { get; set; } 

        //        public int? LocalListenPort { get; set; } = 12400;

        //        public int? TimeOut { get; set; } = 120;

        //        public string MachineListFile { get; set; } = String.Empty;

        #endregion


        public void Fit(double[,] X, float[] y)
        {
            var dsParameters = this.ToDictionary(DataSetParameters.Values);
            var trainDs = new Dataset(X, y, parameters: dsParameters);

            var bstParameters = this.ToDictionary(Constants.BoosterParameters);
            var booster = new Booster(trainDs, bstParameters);
        }

        #region ToDictionary

        static Dictionary<string, string> _underscoreNames = null;

        public IReadOnlyDictionary<string, object> ToDictionary(IEnumerable<string> names=null)
        {
            var thisType = this.GetType();
            var ret = new Dictionary<string, object>();

            if (_underscoreNames == null)
            {
                _underscoreNames = new Dictionary<string, string>();
            }

            Func<PropertyInfo, string> getUnderscoreName = (pi) =>
            {
                var piname = pi.Name;
                return _underscoreNames.SetDefault(pi.Name, pi.Name.ToUnderscoreString());
            }; 
            
            Predicate<string> isValidName = (name) => (names == null || names.Contains(name));

            var props = thisType.GetProperties();
//            foreach (var pi in props)
            for (int i=0; i<props.Length; i++)
            {
                var pi = props[i];
                if (pi.Name == nameof(Booster) || pi.Name == nameof(EvalResult))
                    continue;
                var usName = getUnderscoreName(pi);
                if (!isValidName(usName))
                    continue;
                var propValue = pi.GetValue(this);
                if (propValue == null)
                    continue;
                var propString = String.Empty;
                var propEnum = propValue as IEnumerable;
                if (propEnum != null && pi.PropertyType != typeof(string))
                {
                    propString = String.Join(",", propEnum
                        .Cast<object>()
                        .Select(o => Convert.ToString(o, CultureInfo.InvariantCulture)));
                }
                else if (pi.PropertyType == typeof(bool))
                    propString = (bool)propValue ? "true" : "false";
                else if (pi.PropertyType == typeof(string))
                    propString = (string)propValue;
                else
                    propString = Convert.ToString(propValue, CultureInfo.InvariantCulture);
                if (!String.IsNullOrWhiteSpace(propString))
                    ret.Add(usName, propString);
            }
            return ret;
        }

/*        IReadOnlyDictionary<string, object> GetDictionary(List<PropertyInfo> props)
        {
            var ret = new Dictionary<string, object>();
            foreach (var pi in props)
            {
                var propValue = pi.GetValue(this);
                if (propValue == null)
                    continue;
                string propString = String.Empty;
                var propEnum = propValue as IList;
                if (propEnum != null && pi.PropertyType != typeof(string))
                {
                    propString = String.Join(",", propEnum
                        .Cast<object>()
                        .Select(o => Convert.ToString(o, CultureInfo.InvariantCulture)));
                }
                else if (pi.PropertyType == typeof(bool))
                {
                    var boolVal = (bool)propValue;
                    propString = boolVal ? "true" : "false";
                }
                else
                    propString = Convert.ToString(propValue, CultureInfo.InvariantCulture);
                if (!String.IsNullOrWhiteSpace(propString))
                    ret.Add(pi.Name.ToUnderscoreString(), propString);
                //if (!String.IsNullOrWhiteSpace(propString))
                //    sb.AppendFormat("{0}={1} ", pi.Name.ToUnderscoreString(), propString);
            }
            return ret;
        }
*/
        #endregion
    }

    public class Regressor: Estimator
    {
        public Regressor()
        {
            Objective = Objective.regression;
        }
    }

    public class Classifier: Estimator
    {
        public double Sigmoid { get; set; } = 1.0;

        public Classifier()
        {
            Metric = new[] { LightGBM.Metric.binary_logloss };
            Objective = Objective.binary ;
        }
    }

    public class MultiClassifier: Estimator
    {
        public MultiClassifier()
        {
            Metric = new[] { LightGBM.Metric.multi_logloss };
            Objective = Objective.multiclass ;
        }
    }

}
