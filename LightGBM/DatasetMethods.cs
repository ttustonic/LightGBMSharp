using System;
using System.Runtime.InteropServices;

namespace LightGBM
{
    internal static class DatasetMethods
    {
        /// Return Type: int
        ///filename: char*
        ///parameters: char*
        ///reference: DatasetHandle->void*
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromFile")]
        public static extern int LGBM_DatasetCreateFromFile([In][MarshalAs(UnmanagedType.LPStr)] string filename, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, DatasetHandle reference, out DatasetHandle @out);

        //[DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromFile")]
        //public static extern int LGBM_DatasetCreateFromFile([In][MarshalAs(UnmanagedType.LPStr)] string filename, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, DatasetHandle reference, ref IntPtr @out);

        /// Return Type: int
        ///sample_data: double**
        ///sample_indices: int**
        ///numCols: int
        ///num_per_col: int*
        ///num_sample_row: int
        ///num_total_row: int
        ///parameters: char*
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromSampledColumn")]
        public static extern int LGBM_DatasetCreateFromSampledColumn(ref IntPtr sample_data, ref IntPtr sample_indices, int numCols, ref int num_per_col, int num_sample_row, int num_total_row, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, ref IntPtr @out);
        /// Return Type: int
        ///reference: DatasetHandle->void*
        ///num_total_row: int
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateByReference")]
        public static extern int LGBM_DatasetCreateByReference(IntPtr reference, int num_total_row, ref IntPtr @out);
        /// Return Type: int
        ///dataset: DatasetHandle->void*
        ///data: void*
        ///data_type: int
        ///numRows: int
        ///numCols: int
        ///start_row: int
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetPushRows")]
        public static extern int LGBM_DatasetPushRows(IntPtr dataset, IntPtr data, int data_type, int numRows, int numCols, int start_row);
        /// Return Type: int
        ///dataset: DatasetHandle->void*
        ///indptr: void*
        ///indptr_type: int
        ///indices: int*
        ///data: void*
        ///data_type: int
        ///nindptr: int
        ///nelem: int
        ///num_col: int
        ///start_row: int
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetPushRowsByCSR")]
        public static extern int LGBM_DatasetPushRowsByCSR(IntPtr dataset, IntPtr indptr, int indptr_type, ref int indices, IntPtr data, int data_type, int nindptr, int nelem, int num_col, int start_row);
        /// Return Type: int
        ///indptr: void*
        ///indptr_type: int
        ///indices: int*
        ///data: void*
        ///data_type: int
        ///nindptr: int
        ///nelem: int
        ///num_col: int
        ///parameters: char*
        ///reference: DatasetHandle->void*
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromCSR")]
        public static extern int LGBM_DatasetCreateFromCSR(IntPtr indptr, int indptr_type, ref int indices, IntPtr data, int data_type, int nindptr, int nelem, int num_col, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, IntPtr reference, ref IntPtr @out);
        /// Return Type: int
        ///col_ptr: void*
        ///col_ptr_type: int
        ///indices: int*
        ///data: void*
        ///data_type: int
        ///numCols_ptr: int
        ///nelem: int
        ///num_row: int
        ///parameters: char*
        ///reference: DatasetHandle->void*
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromCSC")]
        public static extern int LGBM_DatasetCreateFromCSC(IntPtr col_ptr, int col_ptr_type, ref int indices, IntPtr data, int data_type, int numCols_ptr, int nelem, int num_row, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, IntPtr reference, ref IntPtr @out);
        /// <summary>
        /// Create dataset from matrix
        /// </summary>
        /// <param name = "data">void*</param>
        /// <param name = "data_type">int</param>
        /// <param name = "numRows">int</param>
        /// <param name = "numCols">int</param>
        /// <param name = "is_row_major"></param>
        /// <param name = "parameters">string</param>
        /// <param name = "reference"></param>
        /// <param name = "out">created Dataset pointer</param>
        /// <returns></returns>
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetCreateFromMat")]
        public static extern int LGBM_DatasetCreateFromMat(IntPtr data, int data_type, int numRows, int numCols, int is_row_major, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, DatasetHandle reference, out DatasetHandle @out);
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///used_row_indices: int*
        ///num_used_row_indices: int
        ///parameters: char*
        ///out: DatasetHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetSubset")]
        public static extern int LGBM_DatasetGetSubset(DatasetHandle handle, ref int used_row_indices, int num_used_row_indices, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, ref IntPtr @out);
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///feature_names: char**
        ///num_feature_names: int
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetSetFeatureNames")]
        public static extern int LGBM_DatasetSetFeatureNames(DatasetHandle handle, IntPtr feature_names, int num_feature_names);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetSetFeatureNames")]
        public static extern int LGBM_DatasetSetFeatureNames(DatasetHandle handle, SafeCharPp feature_names, int num_feature_names);
        /* Ne dela
                [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetSetFeatureNames")]
                public static extern int LGBM_DatasetSetFeatureNames(DatasetHandle handle,
                    [In, MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr) ]
                    string[] feature_names, 
                    int num_feature_names);
        */
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///feature_names: char**
        ///num_feature_names: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetFeatureNames")]
        public static extern int LGBM_DatasetGetFeatureNames(DatasetHandle handle, IntPtr feature_names, out int num_feature_names);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetFeatureNames")]
        public static extern int LGBM_DatasetGetFeatureNames(DatasetHandle handle, SafeCharPp feature_names, out int num_feature_names);
        /* Ovo ne radi
                    [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetFeatureNames")]
                    public static extern int LGBM_DatasetGetFeatureNames(DatasetHandle handle,
                        [MarshalAs(UnmanagedType.LPArray, ArraySubType=UnmanagedType.LPStr, SizeParamIndex =2) ]
                    string[] feature_names,
                        ref int num_feature_names);
        */

        /// Return Type: int
        ///handle: DatasetHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetFree")]
        public static extern int LGBM_DatasetFree(IntPtr handle);
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///filename: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetSaveBinary")]
        public static extern int LGBM_DatasetSaveBinary(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string filename);
        /// <summary>
        /// Set field value
        /// </summary>
        /// <param name = "handle">Dataset handle</param>
        /// <param name = "field_name">Field name to set value. Can be 'label', 'weight', 'group', 'group_id'.</param>
        /// <param name = "field_data">Field data. Pointer to vector</param>
        /// <param name = "num_element">Number of element in field_data</param>
        /// <param name = "type">C_API_DTYPE_FLOAT32 or C_API_DTYPE_INT32</param>
        /// <returns></returns>
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetSetField")]
        public static extern int LGBM_DatasetSetField(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string field_name, IntPtr field_data, int num_element, LGBMDataType type);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetField")]
        public static extern int LGBM_DatasetGetField(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string field_name, out int out_len, out IntPtr out_ptr, out LGBMDataType out_type);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetField")]
        public static extern int LGBM_DatasetGetField(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string field_name, out int out_len, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out float[] out_ptr, out int out_type);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetField")]
        public static extern int LGBM_DatasetGetField(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string field_name, out int out_len, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out int[] out_ptr, out int out_type);
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetField")]
        public static extern int LGBM_DatasetGetField(DatasetHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string field_name, out int out_len, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] out long[] out_ptr, out int out_type);
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///out: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetNumData")]
        public static extern int LGBM_DatasetGetNumData(DatasetHandle handle, out int @out);
        /// Return Type: int
        ///handle: DatasetHandle->void*
        ///out: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_DatasetGetNumFeature")]
        public static extern int LGBM_DatasetGetNumFeature(DatasetHandle handle, out int @out);
    }
}