using System;
using System.Runtime.InteropServices;
using System.Text;

namespace LightGBMSharp
{
    internal static class BoosterMethods
    {
        /// Return Type: int
        ///train_data: DatasetHandle->void*
        ///parameters: char*
        ///out: BoosterHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterCreate")]
        public static extern int LGBM_BoosterCreate(DatasetHandle train_data, [In][MarshalAs(UnmanagedType.LPStr)] string parameters, out BoosterHandle @out);
        /// Return Type: int
        ///filename: char*
        ///out_num_iterations: int*
        ///out: BoosterHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterCreateFromModelfile")]
        public static extern int LGBM_BoosterCreateFromModelfile([In][MarshalAs(UnmanagedType.LPStr)] string filename, out int out_num_iterations, out BoosterHandle @out);
        /// Return Type: int
        ///model_str: char*
        ///out_num_iterations: int*
        ///out: BoosterHandle*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterLoadModelFromString")]
        public static extern int LGBM_BoosterLoadModelFromString([In][MarshalAs(UnmanagedType.LPStr)] string model_str, out int out_num_iterations, out BoosterHandle @out);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterFree")]
        public static extern int LGBM_BoosterFree(IntPtr handle);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///other_handle: BoosterHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterMerge")]
        public static extern int LGBM_BoosterMerge(BoosterHandle handle, BoosterHandle other_handle);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///valid_data: DatasetHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterAddValidData")]
        public static extern int LGBM_BoosterAddValidData(BoosterHandle handle, DatasetHandle valid_data);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///train_data: DatasetHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterResetTrainingData")]
        public static extern int LGBM_BoosterResetTrainingData(BoosterHandle handle, DatasetHandle train_data);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///parameters: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterResetParameter")]
        public static extern int LGBM_BoosterResetParameter(BoosterHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string parameters);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_len: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetNumClasses")]
        public static extern int LGBM_BoosterGetNumClasses(BoosterHandle handle, out int out_len);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///is_finished: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterUpdateOneIter")]
        public static extern int LGBM_BoosterUpdateOneIter(BoosterHandle handle, out int is_finished);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///grad: float*
        ///hess: float*
        ///is_finished: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterUpdateOneIterCustom")]
        public static extern int LGBM_BoosterUpdateOneIterCustom(BoosterHandle handle, ref float grad, ref float hess, ref int is_finished);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterRollbackOneIter")]
        public static extern int LGBM_BoosterRollbackOneIter(BoosterHandle handle);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_iteration: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetCurrentIteration")]
        public static extern int LGBM_BoosterGetCurrentIteration(BoosterHandle handle, out int out_iteration);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_len: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetEvalCounts")]
        public static extern int LGBM_BoosterGetEvalCounts(BoosterHandle handle, out int out_len);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_len: int*
        ///out_strs: char**
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetEvalNames")]
        public static extern int LGBM_BoosterGetEvalNames(BoosterHandle handle, out int out_len, IntPtr out_strs);

        //[DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetEvalNames")]
        //public static extern int LGBM_BoosterGetEvalNames(BoosterHandle handle, out int out_len, SafeCharPp out_strs);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_len: int*
        ///out_strs: char**
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetFeatureNames")]
        public static extern int LGBM_BoosterGetFeatureNames(BoosterHandle handle, out int out_len, IntPtr out_strs);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///out_len: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetNumFeature")]
        public static extern int LGBM_BoosterGetNumFeature(BoosterHandle handle, out int out_len);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///data_idx: int
        ///out_len: int*
        ///out_results: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetEval")]
        public static extern int LGBM_BoosterGetEval(BoosterHandle handle, int data_idx, out int out_len, double[] out_results);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///data_idx: int
        ///out_len: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetNumPredict")]
        public static extern int LGBM_BoosterGetNumPredict(BoosterHandle handle, int data_idx, out int out_len);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///data_idx: int
        ///out_len: int*
        ///out_result: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetPredict")]
        public static extern int LGBM_BoosterGetPredict(BoosterHandle handle, int data_idx, out int out_len, out double[] out_result);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///data_filename: char*
        ///data_has_header: int
        ///predict_type: int
        ///num_iteration: int
        ///parameter: char*
        ///result_filename: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterPredictForFile")]
        public static extern int LGBM_BoosterPredictForFile(BoosterHandle handle, [In][MarshalAs(UnmanagedType.LPStr)] string data_filename, 
            [MarshalAs(UnmanagedType.I4)]
            bool data_has_header, LGBMPredictType predict_type, 
            int num_iteration, 
            [In][MarshalAs(UnmanagedType.LPStr)] string parameter, [In][MarshalAs(UnmanagedType.LPStr)] string result_filename);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///num_row: int
        ///predict_type: int
        ///num_iteration: int
        ///out_len: int*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterCalcNumPredict")]
        public static extern int LGBM_BoosterCalcNumPredict(BoosterHandle handle, int num_row, LGBMPredictType predict_type, int num_iteration, out int out_len);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///indptr: void*
        ///indptr_type: int
        ///indices: int*
        ///data: void*
        ///data_type: int
        ///nindptr: int
        ///nelem: int
        ///num_col: int
        ///predict_type: int
        ///num_iteration: int
        ///parameter: char*
        ///out_len: int*
        ///out_result: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterPredictForCSR")]
        public static extern int LGBM_BoosterPredictForCSR(BoosterHandle handle, IntPtr indptr, int indptr_type, ref int indices, IntPtr data, int data_type, int nindptr, int nelem, int num_col, int predict_type, int num_iteration, [In][MarshalAs(UnmanagedType.LPStr)] string parameter, ref int out_len, ref double out_result);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///col_ptr: void*
        ///col_ptr_type: int
        ///indices: int*
        ///data: void*
        ///data_type: int
        ///numCols_ptr: int
        ///nelem: int
        ///num_row: int
        ///predict_type: int
        ///num_iteration: int
        ///parameter: char*
        ///out_len: int*
        ///out_result: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterPredictForCSC")]
        public static extern int LGBM_BoosterPredictForCSC(BoosterHandle handle, IntPtr col_ptr, int col_ptr_type, ref int indices, IntPtr data, int data_type, int numCols_ptr, int nelem, int num_row, int predict_type, int num_iteration, [In][MarshalAs(UnmanagedType.LPStr)] string parameter, ref int out_len, ref double out_result);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///data: void*
        ///data_type: int
        ///numRows: int
        ///numCols: int
        ///is_row_major: int
        ///predict_type: int
        ///num_iteration: int
        ///parameter: char*
        ///out_len: int*
        ///out_result: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterPredictForMat")]
        public static extern int LGBM_BoosterPredictForMat(BoosterHandle handle, IntPtr data, LGBMDataType data_type, int numRows, int numCols, int is_row_major, LGBMPredictType predict_type, int num_iteration, [In][MarshalAs(UnmanagedType.LPStr)] string parameter, out int out_len, double[] out_result);

        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterPredictForMat")]
        public static extern int LGBM_BoosterPredictForMat(BoosterHandle handle, IntPtr data, LGBMDataType data_type, int numRows, int numCols, int is_row_major, LGBMPredictType predict_type, int num_iteration, [In][MarshalAs(UnmanagedType.LPStr)] string parameter, out int out_len, IntPtr out_result);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///num_iteration: int
        ///filename: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterSaveModel")]
        public static extern int LGBM_BoosterSaveModel(BoosterHandle handle, int num_iteration, [In][MarshalAs(UnmanagedType.LPStr)] string filename);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///num_iteration: int
        ///buffer_len: int
        ///out_len: int*
        ///out_str: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterSaveModelToString")]
        public static extern int LGBM_BoosterSaveModelToString(BoosterHandle handle, int num_iteration, int buffer_len, out int out_len,
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder out_str);

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///num_iteration: int
        ///buffer_len: int
        ///out_len: int*
        ///out_str: char*
/*https://docs.microsoft.com/en-us/dotnet/framework/interop/default-marshaling-for-strings
In some circumstances, a fixed-length character buffer must be passed 
into unmanaged code to be manipulated. Simply passing a string does 
not work in this case because the callee cannot modify the contents of 
the passed buffer. Even if the string is passed by reference, 
there is no way to initialize the buffer to a given size.

The solution is to pass a StringBuilder buffer as the argument instead of 
a string. A StringBuilder can be dereferenced and modified by the callee, 
provided it does not exceed the capacity of the StringBuilder. 
It can also be initialized to a fixed length. 
For example, if you initialize a StringBuilder buffer to a capacity of N, 
the marshaler provides a buffer of size (N+1) characters. 
The +1 accounts for the fact that the unmanaged string has a null terminator while StringBuilder does not. 
*/
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterDumpModel", CharSet =CharSet.Unicode)]
        public static extern int LGBM_BoosterDumpModel(BoosterHandle handle, int num_iteration, int buffer_len, out int out_len, 
            [MarshalAs(UnmanagedType.LPStr)]
            StringBuilder out_str) ;

        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///tree_idx: int
        ///leaf_idx: int
        ///out_val: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterGetLeafValue")]
        public static extern int LGBM_BoosterGetLeafValue(BoosterHandle handle, int tree_idx, int leaf_idx, out double out_val);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///tree_idx: int
        ///leaf_idx: int
        ///val: double
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterSetLeafValue")]
        public static extern int LGBM_BoosterSetLeafValue(BoosterHandle handle, int tree_idx, int leaf_idx, double val);
        /// Return Type: int
        ///handle: BoosterHandle->void*
        ///num_iteration: int
        ///importance_type: int
        ///out_results: double*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_BoosterFeatureImportance")]
        public static extern int LGBM_BoosterFeatureImportance(BoosterHandle handle, int num_iteration, int importance_type, ref double out_results);
    }
}
 