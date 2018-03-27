using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;


namespace LightGBM
{
    using static Utils;
    using static DatasetMethods;

    partial class Dataset
    {
        /// <summary>
        /// Create DatasetHandle from double matrix
        /// </summary>
        /// <param name="data">data matrix</param>
        /// <param name="isRowMajor"></param>
        /// <param name="parameters"></param>
        /// <param name="reference"></param>
        /// <returns>a new DatasetHandle</returns>
        static DatasetHandle CreateFromMat(double[,] data, int isRowMajor, string parameters, DatasetHandle reference = null)
        {
            var dataType = Constants.TypeLgbmTypeMap[typeof(double)];
            return CreateFromMat(data, dataType, isRowMajor, parameters, reference);
        }

        /// <summary>
        /// Create DatasetHandle from float matrix
        /// </summary>
        /// <param name="data">data matrix</param>
        /// <param name="isRowMajor"></param>
        /// <param name="parameters"></param>
        /// <param name="reference"></param>
        /// <returns>a new DatasetHandle</returns>
        static DatasetHandle CreateFromMat(float[,] data, int isRowMajor, string parameters, DatasetHandle reference = null)
        {
            var dataType = Constants.TypeLgbmTypeMap[typeof(float)];
            return CreateFromMat(data, dataType, isRowMajor, parameters, reference);
        }

        static DatasetHandle CreateFromMat(Array data, LGBMDataType dataType, int isRowMajor, string parameters, DatasetHandle reference)
        {
            DatasetHandle dsHandle = null;
            var numCols = data.GetColsCount();
            var numRows = data.GetRowsCount();
            DatasetHandle ptr = reference ?? DatasetHandle.Zero;
            IntPtr dataPtr = data.GetPointer();

            SafeCall(LGBM_DatasetCreateFromMat(dataPtr, (int)dataType, numRows, numCols, isRowMajor, parameters, ptr, out dsHandle));
            return dsHandle;
        }

        static DatasetHandle CreateFromFile(string fileName, string parameters, DatasetHandle reference = null)
        {
            DatasetHandle dsHandle = null;

            reference = reference ?? DatasetHandle.Zero;
            SafeCall(LGBM_DatasetCreateFromFile(fileName, parameters, reference, out dsHandle));
            return dsHandle;
        }

        public int GetNumData()
        {
            int numData;
            SafeCall(LGBM_DatasetGetNumData(_handle, out numData));
            return numData;
        }

        #region Get/set fields
        /// <summary>
        /// </summary>
        /// <typeparam name="T">Type of data, should be int, long, float, double</typeparam>
        /// <param name="dsHandle"></param>
        /// <param name="fieldName"></param>
        /// <param name="data"></param>
        public void SetField<T>(string fieldName, IReadOnlyList<T> data) where T : struct
        {
            LGBMDataType dataType = 0; // Constants.TypeLgbmTypeMap[typeof(T)];
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            if (!Constants.FieldDataTypeMap.TryGetValue(fieldName, out dataType))
            {
                throw new ArgumentException($"Invalid field name {fieldName}");
            }

            Array dataArray = data as T[] ?? data.ToArray() ;
            if (dataType != Constants.TypeLgbmTypeMap[typeof(T)])
            {
                if (fieldName == Constants.LABEL || fieldName == Constants.WEIGHT)
                {
                    dataArray = data.Select(t => (float)Convert.ToDouble(t)).ToArray();
                }
                else if (fieldName == Constants.GROUP || fieldName == Constants.GROUP_ID)
                {
                    dataArray = data.Select(t => Convert.ToInt32(t)).ToArray();
                }
            }

            IntPtr dataPtr = IntPtr.Zero;
            int dataLength = 0;
            if (data == null)
            {
                dataPtr = IntPtr.Zero;
                dataLength = 0;
            }
            else
            {
                dataPtr = dataArray.GetPointer();
                dataLength = dataArray.Length;
            }

            SafeCall(LGBM_DatasetSetField(_handle, fieldName, dataPtr, dataLength, dataType));
        }

        public Array GetField(string fieldName)
        {
            int length;
            LGBMDataType type;
            IntPtr dataPtr;

            Utils.SafeCall(LGBM_DatasetGetField(_handle, fieldName, out length, out dataPtr, out type));
            switch (type)
            {
                case LGBMDataType.Float32:
                default:
                    {
                        float[] array = new float[length];
                        Marshal.Copy(dataPtr, array, 0, length);
                        return array;
                    }
                case LGBMDataType.Float64:
                    {
                        double[] array = new double[length];
                        Marshal.Copy(dataPtr, array, 0, length);
                        return array;
                    }
                case LGBMDataType.Int32:
                    {
                        int[] array = new int[length];
                        Marshal.Copy(dataPtr, array, 0, length);
                        return array;
                    }
            }
        }
        #endregion

        #region Get/set Features
        public int GetNumFeatures()
        {
            int numFeatures;
            SafeCall(LGBM_DatasetGetNumFeature(_handle, out numFeatures));
            return numFeatures;
        }

        string[] GetFeatureNames()
        {
            int length = GetNumFeatures();
            if (length == 0)
                return new string[0];

            var fnPtr = Utils.GetIntPtrForStringArray(length);
            SafeCall(LGBM_DatasetGetFeatureNames(_handle, fnPtr, out length));
            var strings = Utils.IntPtrToStringArray<byte>(fnPtr, length);
            Marshal.FreeCoTaskMem(fnPtr);
            return strings;
        }

        void SetFeatureNames(IReadOnlyList<string> featureNames)
        {
            int length = GetNumFeatures();
            var featureNamesArr = featureNames as string[] ?? featureNames.ToArray();
            length = featureNamesArr.Length;

            IntPtr fnPtr = Utils.StringArrayToIntPtr<byte>(featureNamesArr);
            SafeCall(LGBM_DatasetSetFeatureNames(_handle, fnPtr, length));
            Marshal.FreeCoTaskMem(fnPtr);
        }
        #endregion

        public void SaveBinary(string fileName)
        {
            SafeCall(LGBM_DatasetSaveBinary(_handle, fileName));
        }
    }
}
