using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LightGBM
{
    using static BoosterMethods;
    using static Utils;

    public partial class Booster: IDisposable
    {
        #region Fields and properties
        internal BoosterHandle _handle;
        Dataset _trainSet = null;
        string[] _evalNames = null;
        string[] _featureNames = null;
        int _numClasses;
        int _numDatasets = 1;
        bool _needReloadEvalInfo = true;
        DefaultDict<string, Dictionary<string, double>> _bestScore = new DefaultDict<string, Dictionary<string, double>>();

        List<string> _nameValidSets = new List<string>();
        string _parameters;
        List<KeyValuePair<Dataset, string>> _validationSets = new List<KeyValuePair<Dataset, string>>();

        public int CurrentIteration
        {
            get { return GetCurrentIteration(); }
        }

        public int BestIteration { get; set; }

        public Dictionary<string, Dictionary<string, double>> BestScore
        {
            get { return _bestScore; }
        } 

        public string[] EvaluationNames
        {
            get
            {
                _evalNames = _evalNames ?? GetEvalNames();
                return _evalNames;
            }
        }

        public IReadOnlyList<string> FeatureNames
        {
            get
            {
                _featureNames = _featureNames ?? GetFeatureNames();
                return _featureNames;
            }
        }

        #endregion

        #region Constructors
        public Booster(Dataset trainSet, IReadOnlyDictionary<string, object> parameters)
            : this(trainSet, DictionaryUtils.ToParamsString(parameters))
        { }

        /// <summary>
        /// Training task
        /// </summary>
        public Booster(Dataset trainSet, string parameters = "")
        {
            _handle = Create(trainSet.Handle, parameters);
            _evalNames = GetEvalNames();
            _trainSet = trainSet;
            _numDatasets = 1;
            _parameters = parameters;
            _numClasses = GetNumClasses();
            _needReloadEvalInfo = true;
        }

        /// <summary>
        /// Prediction task
        /// </summary>
        /// <param name="fileName">Model file name</param>
        public Booster(string fileName)
        {
            int numIterations = 0;
            _handle = CreateFromModelfile(fileName, out numIterations);
            _numClasses = GetNumClasses();
            _needReloadEvalInfo = true;
        }

        Booster(BoosterHandle handle)
        {
            _handle = handle;
            _numClasses = GetNumClasses();
            _needReloadEvalInfo = true;
        }

        public static Booster LoadFromModelString(string modelString)
        {
            BoosterHandle bh = LoadModelFromString(modelString);
            var booster = new Booster(bh);
            return booster;
        }

        #endregion

        public double[] Predict(double[,] data, LGBMPredictType predictType = LGBMPredictType.PredictNormal, int numIterations = -1)
        {
            return PredictForMat(data, predictType, numIterations);
        }

        public double[] Predict(string fileName, bool dataHasHeader = false, LGBMPredictType predictType = LGBMPredictType.PredictNormal, int numIterations = -1)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException($"File {fileName} does not exist");
            var tempFile = Path.GetTempFileName();
            Utils.SafeCall(LGBM_BoosterPredictForFile(_handle, fileName, dataHasHeader, predictType, numIterations, _parameters, tempFile));

            var lines = File.ReadAllLines(tempFile);
            throw new Exception("TODO : What's in lines ?");

            return null;
        }


        /// <summary>
        /// Add validation data
        /// </summary>
        /// <param name="validData">Validation data</param>
        /// <param name="name">Name of validation data</param>
        public void AddValidData(Dataset validData, string name)
        {
            SafeCall(LGBM_BoosterAddValidData(_handle, validData.Handle));
            _validationSets.Add(new KeyValuePair<Dataset, string>(validData, name));
            _numDatasets++;
        }

        /// <summary>
        /// Update for one iteration.
        /// </summary>
        /// <param name="trainSet">Training data. If null, use current training data.</param>
        /// <returns>Is finished</returns>
        public bool Update(Dataset trainSet = null)
        {
            if (!(trainSet == null || trainSet == _trainSet))
            {
                ResetTrainingData(trainSet);
            }
            return UpdateOneIter();
        }

        public void ResetParameters(IReadOnlyDictionary<string, object> parameters)
        {
            if (parameters.ContainsKey("metric"))
            {
                _needReloadEvalInfo = true;
            }
            var paramStr = parameters.ToParamsString();
            if (String.IsNullOrWhiteSpace(paramStr))
                return;
            SafeCall(LGBM_BoosterResetParameter(_handle, paramStr));
        }

        /// <summary>
        /// __get_eval_info
        /// </summary>
        void GetEvaluationInfo()
        {
            if (!_needReloadEvalInfo)
                return;
            _needReloadEvalInfo = false;
            _evalNames = GetEvalNames();
            _featureNames = GetFeatureNames();
        }

        public IReadOnlyList<EvaluationResult> EvaluateValidationData()
        {
            var ret = _validationSets
                .SelectMany((vs, i) => InnerEval(vs.Value, i+1))
                .ToList();
            return ret;
        }

        /// <summary>
        /// Evaluate training or evaluation data.
        /// </summary>
        IReadOnlyList<EvaluationResult> InnerEval(string dataName, int dataIdx)
        {
            GetEvaluationInfo();
            int numInnerEval = EvaluationNames.Length;
            var ret = new List<EvaluationResult>();
            if (numInnerEval > 0)
            {
                var biggerIsBetter =
                    EvaluationNames.Select(m => Constants.MaximizeMetrics.Contains(m))
                    .ToList();

                var result = GetEval(dataIdx);
                for (int i = 0; i < numInnerEval; i++)
                {
                    var tpl = new EvaluationResult(dataName, EvaluationNames[i], result[i],
                        biggerIsBetter[i]);
                    ret.Add(tpl);
                }
            }
            return ret;
        }

        #region IDisposable
        public void Dispose()
        {
            _handle.Dispose();
        }
        #endregion

    }
}
