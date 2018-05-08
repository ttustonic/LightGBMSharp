using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace LightGBMSharp
{
    public static class ArrayUtils
    {
        public static int GetRowsCount(this Array array)
        {
            return array.GetLength(0);
        }

        public static int GetColsCount(this Array array)
        {
            return array.GetLength(1);
        }

        public static TOut[,] ConvertMultiArray<TIn, TOut>(this TIn[,] array, Converter<TIn, TOut> converter)
        {
            var rows = array.GetRowsCount();
            var cols = array.GetColsCount();
            var res = new TOut[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    res[i, j] = converter(array[i, j]);
            }
            return res;
        }

        public static T[,] FillArray<T>(T value, int rows, int cols)
        {
            T[,] ret = new T[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    ret[i, j] = value;
            }
            return ret;
        }

        public static T[] FillArray<T>(T value, int rows)
        {
            T[] ret = new T[rows];
            for (int i = 0; i < rows; i++)
                ret[i] = value;
            return ret;
        }

        public static T[,] TrimArray<T>(int rowToRemove, int columnToRemove, T[,] originalArray)
        {
            //            T[,] result = new T[originalArray.GetLength(0) - 1, originalArray.GetLength(1) - 1];
            T[,] result = new T[originalArray.GetLength(0), originalArray.GetLength(1) - 1];

            for (int i = 0, j = 0; i < originalArray.GetLength(0); i++)
            {
                if (i == rowToRemove)
                    continue;

                for (int k = 0, u = 0; k < originalArray.GetLength(1); k++)
                {
                    if (k == columnToRemove)
                        continue;

                    result[j, u] = originalArray[i, k];
                    u++;
                }
                j++;
            }

            return result;
        }
    }

    public static class DictionaryUtils
    {
        public static string ToParamsString(this IReadOnlyDictionary<string, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
                return String.Empty;
            List<string> pairs = new List<string>();
            foreach (var kvp in parameters)
            {
                if (kvp.Value == null)
                    continue;
                var ieVal = kvp.Value as IEnumerable;
                string appVal;

                appVal = ieVal == null || kvp.Value is string ?
                    Convert.ToString(kvp.Value, CultureInfo.InvariantCulture) :
                    String.Join(",", ieVal.Cast<object>().Select(o => Convert.ToString(o, CultureInfo.InvariantCulture)));
                pairs.Add($"{kvp.Key}={appVal}");
            }
            return String.Join(" ", pairs).Trim();
        }

        /// <summary>
        /// If <paramref name="key"/> is in the dictionary, return its value, else
        /// insert key with a value of <paramref name="defaultValue"/> and return <paramref name="defaultValue"/>. 
        /// <para>Default value for <paramref name="defaultValue"/> is default(V) </para>
        /// <para>Like Python setdefault</para>
        /// </summary>
        /// <returns>A dictionary value or <paramref name="defaultValue"/>.</returns>
        public static V SetDefault<K, V>(this IDictionary<K, V> dict, K key, V defaultValue = default(V))
        {
            V value;
            if (!dict.TryGetValue(key, out value))
            {
                dict.Add(key, value = defaultValue);
            }
            return value;
        }

        public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (second == null || first == null)
                return;
            foreach (var item in second)
                if (!first.ContainsKey(item.Key))
                    first.Add(item.Key, item.Value);
        }
    }

    public static class Utils
    {
        public static string ToUnderscoreString(this string s)
        {
            return Regex.Replace(s, @"(^[a-z0-9]+|[A-Z0-9]+(?![a-z])|[A-Z0-9][a-z0-9]+)", "_$1", RegexOptions.Compiled).Trim('_').ToLower();
        }

        internal static IntPtr GetPointer(this object o)
        {
            IntPtr dataPtr;
            GCHandle dataHandle = GCHandle.Alloc(o, GCHandleType.Pinned);
            try
            {
                dataPtr = dataHandle.AddrOfPinnedObject();
            }
            finally
            {
                if (dataHandle.IsAllocated)
                    dataHandle.Free();
            }
            return dataPtr;
        }

        #region string array-IntPtr conversions
        /// <summary>
        /// <paramref name="ptrRoot"/> is not freed, needs Marshal.FreeCoTaskMem
        /// </summary>
        /// <typeparam name="GenChar">type of string. Byte calls PtrToStringAnsi, char calls PtrToStringUni.</typeparam>
        /// <param name="ptrRoot"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        //        https://www.codeproject.com/articles/17450/marshal-an-array-of-zero-terminated-strings-or-str
        public static string[] IntPtrToStringArray<GenChar>(IntPtr ptrRoot, int size) where GenChar : struct
        {
            //get the output array of pointers
            IntPtr[] outPointers = new IntPtr[size];
            Marshal.Copy(ptrRoot, outPointers, 0, size);

            string[] outputStrArray = new string[size];
            for (int i = 0; i < size; i++)
            {
                if (typeof(GenChar) == typeof(byte))
                    outputStrArray[i] = Marshal.PtrToStringAnsi(outPointers[i]);
                else if (typeof(GenChar) == typeof(char))
                    outputStrArray[i] = Marshal.PtrToStringUni(outPointers[i]);
                else
                    throw new Exception($"GenChar has to be byte or char");
                //dispose of unneeded memory
                Marshal.FreeCoTaskMem(outPointers[i]);
            }
            //dispose of the pointers array
            //            Marshal.FreeCoTaskMem(ptrRoot);
            return outputStrArray;
        }

//        https://www.codeproject.com/articles/17450/marshal-an-array-of-zero-terminated-strings-or-str
        internal static IntPtr StringArrayToIntPtr<GenChar>(string[] strings) where GenChar : struct
        {
            int size = strings.Length;
            //build array of pointers to string
            IntPtr[] InPointers = new IntPtr[size];
            int dim = IntPtr.Size * size;
            IntPtr rRoot = Marshal.AllocCoTaskMem(dim);
            for (int i = 0; i < size; i++)
            {
                if (typeof(GenChar) == typeof(byte))
                    InPointers[i] = Marshal.StringToCoTaskMemAnsi(strings[i]);
                else if (typeof(GenChar) == typeof(char))
                    InPointers[i] = Marshal.StringToCoTaskMemUni(strings[i]);
                else
                    throw new Exception($"GenChar has to be byte or char");

            }
            //copy the array of pointers
            Marshal.Copy(InPointers, 0, rRoot, size);

            //for (int i = 0; i < size; i++)
            //{
            //    Marshal.FreeCoTaskMem(InPointers[i]);
            //}

            return rRoot;
        }

/*
        internal static string[] SafeCharppToStringArray(SafeCharPp ptrRoot, int size)
        {
            IntPtr[] outPointers = new IntPtr[size];
            Marshal.Copy(ptrRoot.DangerousGetHandle(), outPointers, 0, size);

            string[] outputStrArray = new string[size];
            for (int i = 0; i < size; i++)
            {
                outputStrArray[i] = Marshal.PtrToStringAnsi(outPointers[i]);
                Marshal.FreeCoTaskMem(outPointers[i]);
            }
            return outputStrArray;
        }

        internal static SafeCharPp StringArrayToSafeCharpp(string[] strings)
        {
            int size = strings.Length;
            //build array of pointers to string
            IntPtr[] InPointers = new IntPtr[size];
            int dim = IntPtr.Size * size;
            IntPtr rRoot = Marshal.AllocCoTaskMem(dim);
            for (int i = 0; i < size; i++)
            {
                InPointers[i] = Marshal.StringToCoTaskMemAnsi(strings[i]);
            }
            //copy the array of pointers
            Marshal.Copy(InPointers, 0, rRoot, size);
            return new SafeCharPp(rRoot);
        }
*/
        /// <summary>
        /// Get IntPtr for methods that expect char** parameter
        /// </summary>
        /// <param name="numElements"></param>
        /// <returns>Allocated IntPtr</returns>
        internal static IntPtr GetIntPtrForStringArray(int numElements)
        {
            var strings = new string[numElements];
            for (int i = 0; i < numElements; i++)
                strings[i] = new string('1', 256) ;
            var ptr = StringArrayToIntPtr<byte>(strings);
            return ptr;
        }

        /// <summary>
        /// Get <see cref="SafeCharPp" /> for methods that expect char** parameter.
        /// </summary>
        /// <param name="numElements">Number of strings</param>
        /// <returns>Allocated <see cref="SafeCharPp"/></returns>
        internal static SafeCharPp GetSafePtrForStringArray(int numElements)
        {
            var strings = new string[numElements];
            for (int i = 0; i < numElements; i++)
                strings[i] = "";
            SafeCharPp ptr = new SafeCharPp(Utils.StringArrayToIntPtr<byte>(strings));
            return ptr;
        }
        #endregion

        internal static void SafeCall(int lgbmFuncRet)
        {
            if (lgbmFuncRet != 0)
            {
                var err = Marshal.PtrToStringAnsi(NativeMethods.LGBM_GetLastError());
                throw new Exception(err);
            }
        }
    }
}
