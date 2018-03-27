namespace LightGBM
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using static BoosterMethods;
    using static Utils;

    public partial class Booster
    {
        static BoosterHandle Create(DatasetHandle dataset, string parameters)
        {
            BoosterHandle bsHandle;
            SafeCall(LGBM_BoosterCreate(dataset, parameters, out bsHandle));
            return bsHandle;
        }

        static BoosterHandle CreateFromModelfile(string fileName, out int numIterations)
        {
            BoosterHandle bsHandle;
            SafeCall(LGBM_BoosterCreateFromModelfile(fileName, out numIterations, out bsHandle));
            return bsHandle;
        }

        static BoosterHandle LoadModelFromString(string modelString)
        {
            BoosterHandle bsHandle;
            int numIterations = 0;
            SafeCall(LGBM_BoosterLoadModelFromString(modelString, out numIterations, out bsHandle));
            return bsHandle;
        }

        public void Merge(BoosterHandle otherHandle)
        {
            SafeCall(LGBM_BoosterMerge(_handle, otherHandle));
        }

        void ResetTrainingData(Dataset trainData)
        {
            SafeCall(LGBM_BoosterResetTrainingData(_handle, trainData.Handle));
            this._trainSet = trainData;
        }

        /// <summary>
        /// Reset parameters for <see cref="Booster"/>
        /// </summary>
        public void ResetParameter(IReadOnlyDictionary<string, object> parameters)
        {
            var paramString = DictionaryUtils.ToParamsString(parameters);
            ResetParameter(paramString);
        }

        /// <summary>
        /// Reset parameters for <see cref="Booster"/>
        /// </summary>
        public void ResetParameter(string parameters)
        {
            SafeCall(LGBM_BoosterResetParameter(_handle, parameters));
        }

        public int GetNumClasses()
        {
            int outLen;
            SafeCall(LGBM_BoosterGetNumClasses(_handle, out outLen));
            return outLen;
        }

        public bool UpdateOneIter()
        {
            int isFinished;
            SafeCall(LGBM_BoosterUpdateOneIter(_handle, out isFinished));
            return isFinished == 1;
        }

        public void RollbackOneIter()
        {
            SafeCall(LGBM_BoosterRollbackOneIter(_handle));
        }

        int GetCurrentIteration()
        {
            int iteration;
            SafeCall(LGBM_BoosterGetCurrentIteration(_handle, out iteration));
            return iteration;
        }

        #region Evaluations
        int GetEvalCount()
        {
            int len;
            SafeCall(LGBM_BoosterGetEvalCounts(_handle, out len));
            return len;
        }

// vidi            https://stackoverflow.com/questions/28982669/does-gchandle-alloc-allocate-memory#28988163
        internal string[] GetEvalNames()
        {
            int n = GetEvalCount();
            if (n == 0)
                return new string[0];

            /*
                        using (var ptr = GetSafePtrForStringArray(n))
                        {
                            SafeCall(LGBM_BoosterGetEvalNames(_handle, out n, ptr));
                            var strings = SafeCharppToStringArray(ptr, n);
                            return strings;
                        }
            */
            var ptr = Utils.GetIntPtrForStringArray(n);
            LGBM_BoosterGetEvalNames(_handle, out n, ptr);
            var strings = Utils.IntPtrToStringArray<byte>(ptr, n);
            Marshal.FreeCoTaskMem(ptr);
            return strings;
        }

        internal double[] GetEval(int dataIdx)
        {
            var numMetrics = GetEvalCount();
            if (numMetrics == 0)
                return new double[0];

            double[] results = new double[numMetrics];
            int length;
            SafeCall(LGBM_BoosterGetEval(_handle, dataIdx, out length, results));
            return results;
        }

        #endregion

        #region Features
        internal int GetNumFeature()
        {
            int numfeatures;
            SafeCall(LGBM_BoosterGetNumFeature(_handle, out numfeatures));
            return numfeatures;
        }

        internal string[] GetFeatureNames()
        {
            int outLen;
            int numFeature = GetNumFeature();
            if (numFeature == 0)
                return new string[0];
            var ptr = Utils.GetIntPtrForStringArray(numFeature);
            LGBM_BoosterGetFeatureNames(_handle, out outLen, ptr);
            var strings = Utils.IntPtrToStringArray<byte>(ptr, outLen);
            Marshal.FreeCoTaskMem(ptr);
            return strings;
        }
        #endregion

        #region Prediction
        int GetNumPredict(int dataNdx)
        {
            int outLen;
            SafeCall(LGBM_BoosterGetNumPredict(_handle, dataNdx, out outLen));
            return outLen;
        }

        double[] GetPredict(int dataNdx)
        {
            int outLen;
            int numClass = GetNumClasses();
            int numData = GetNumPredict(dataNdx);
            double[] results = new double[numClass * numData];
            SafeCall(LGBM_BoosterGetPredict(_handle, dataNdx, out outLen, out results));
            return results;
        }

        /// <summary>
        /// Get size of prediction result.
        /// </summary>
        int CalcNumPredict(int numRow, LGBMPredictType predictType, int numIteration)
        {
// Python ima long za outLen
            int outLen;
            SafeCall(LGBM_BoosterCalcNumPredict(_handle, numRow, predictType, numIteration, out outLen));
            return outLen;
        }

        double[] PredictForMat<T>(T[,] data, LGBMPredictType predictType, int numIteration) where T: struct
        {
            var ttype = typeof(T);
            if (!(ttype == typeof(double) || ttype == typeof(float)))
                throw new ArgumentException("data must be float or double");
            int numClasses = GetNumClasses();
            var dataType =  Constants.TypeLgbmTypeMap[ttype];
            var numRows = data.GetRowsCount();
            var numCols = data.GetColsCount();
            int outNumPreds;
            var numPredict = CalcNumPredict(numRows, predictType, numIteration);
            var result = new double[numPredict];
            Array dataArr = (Array)data;
            IntPtr dataPtr = dataArr.GetPointer();
            SafeCall(LGBM_BoosterPredictForMat(_handle, dataPtr, dataType, numRows, numCols, 1, predictType, numIteration, "", out outNumPreds, result));

            if (outNumPreds != numPredict)
                throw new Exception("Wrong number of predict results");
            return result;
        }
        #endregion

        #region Save to file, string, JSON
        /// <summary>
        /// Save booster model to file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="numIteration">Number of iterations to save, or -1 to save the best iteration.</param>
        public void SaveModel(string fileName, int numIteration = -1)
        {
            if (numIteration < 0)
                numIteration = BestIteration;
            SafeCall(LGBM_BoosterSaveModel(_handle, numIteration, fileName));
        }

        public string SaveModelToString(int numIteration=-1)
        {
            if (numIteration < 0)
                numIteration = BestIteration;
            int bufferLen = 1000 ;
            int outLen;
ALLOC:
            StringBuilder sb  = new StringBuilder(bufferLen);
            SafeCall(LGBM_BoosterSaveModelToString(_handle, numIteration, bufferLen, out outLen, sb));
// if buffer length is not long enough, reallocate a buffer
            if (outLen > bufferLen)
            {
                bufferLen = outLen + 1;
                goto ALLOC;
            }
            return sb.ToString() ;
        }

        /// <summary>
        /// Dump booster model to JSON.
        /// </summary>
        /// <param name="numIteration"></param>
        /// <returns></returns>
        public string DumpModel(int numIteration = -1)
        {
            if (numIteration < 0)
                numIteration = BestIteration;
           
            int outLen;
            int bufferLen = 1 << 20 ;
ALLOC:
            var sb = new StringBuilder(bufferLen);
            SafeCall(LGBM_BoosterDumpModel(_handle, numIteration, bufferLen, out outLen, sb));
// if buffer length is not long enough, reallocate a buffer
            if (outLen > bufferLen)
            {
                bufferLen = outLen + 1;
                goto ALLOC;
            }
            return sb.ToString() ;
        }
        #endregion
    }
}
