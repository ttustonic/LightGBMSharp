using System;
using System.Linq;
using System.Collections.Generic;

namespace LightGBM
{
    public partial class Dataset: IDisposable
    {
        #region Fields and properties
        DatasetHandle _handle;
        int _maxBin = 0;
        public float[] Label
        {
            get { return (float[])GetField(Constants.LABEL); }
            set { SetField(Constants.LABEL, value); }
        }

        public IReadOnlyList<float> Weight
        {
            get { return (float[])GetField(Constants.WEIGHT); }
            set { SetField(Constants.WEIGHT, value); }
        }

        public IReadOnlyList<double> InitScore
        {
            get { return (double[])GetField(Constants.INIT_SCORE); }
            set { SetField(Constants.INIT_SCORE, value); }
        }

        public IReadOnlyList<string> Features
        {
            get { return GetFeatureNames(); }
            set { SetFeatureNames(value); }
        }

        public int DataNumber
        {
            get { return GetNumData(); }
        }

        internal DatasetHandle Handle
        {
            get { return _handle; }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name = "data">double array of data.</param>
        /// <param name = "maxBin">Max number of discrete bin for features.</param>
        /// <param name = "label">Optional, label of the data</param>
        /// <param name = "parameters">Other parameters.</param>
        public Dataset(double[, ] data, double[] label = null, int maxBin = 255, Dataset reference = null, IReadOnlyDictionary<string, object> parameters = null)
            : this(data, Array.ConvertAll(label, (d) => (float)d) , maxBin, reference, parameters.ToParamsString())
        {
        }

        public Dataset(double[,] data, float[] label = null, int maxBin = 255, Dataset reference = null, IReadOnlyDictionary<string, object> parameters = null)
            : this(data, label, maxBin, reference, parameters.ToParamsString())
        {
        }

        Dataset(double[,] data, float[] label = null, int maxBin = 255, Dataset reference = null, string paramString = "")
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            _maxBin = maxBin;
            _handle = CreateFromMat(data, 1, paramString, reference?._handle);
            if (label != null)
                SetField<float>("label", label);
        }

        public Dataset(string fileName, IReadOnlyDictionary<string, object> parameters=null, Dataset reference=null)
        {
            if (fileName == null)
                throw new ArgumentNullException(nameof(fileName));
            var refHandle = reference?._handle ?? DatasetHandle.Zero;
            _handle = CreateFromFile(fileName, parameters.ToParamsString(), refHandle);
        }

        #region IDisposable Support

        public void Dispose()
        {
            _handle.Dispose();
            _handle = null;
        }

        #endregion
    }
}