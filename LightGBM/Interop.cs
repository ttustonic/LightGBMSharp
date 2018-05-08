using System;
using System.Runtime.InteropServices;

namespace LightGBMSharp
{
    static class NativeMethods
    {
        /// Return Type: char*
        [DllImport("lib_lightgbm.dll", EntryPoint = "LGBM_GetLastError")]
        public static extern IntPtr LGBM_GetLastError();
    }

    /// <summary>
    /// safehandle for char**
    /// </summary>
    internal class SafeCharPp: SafeHandle
    {
        SafeCharPp(IntPtr invalidHandleValue, bool ownsHandle) : base(invalidHandleValue, ownsHandle)
        {
        }

        public SafeCharPp(IntPtr handle) : base(IntPtr.Zero, true)
        {
            this.SetHandle(handle);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        /// <summary>When overridden in a derived class, executes the code required to free the handle.</summary>
        /// <returns>true if the handle is released successfully; otherwise, in the event of a catastrophic failure, false. In this case, it generates a releaseHandleFailed MDA Managed Debugging Assistant.</returns>
        protected override bool ReleaseHandle()
        {
            Marshal.FreeCoTaskMem(handle);
            return true;
        }
    }

    public class BoosterHandle: SafeHandle
    {
        static readonly BoosterHandle _zero = new BoosterHandle();

        public static BoosterHandle Zero { get { return _zero; } }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            return BoosterMethods.LGBM_BoosterFree(this.handle) == 0;
        }

        BoosterHandle() : base(IntPtr.Zero, true)
        {
        }
    }

    // http://blog.benoitblanchon.fr/safehandle/
    // https://blogs.msdn.microsoft.com/shawnfa/2004/08/12/safehandle/
    public class DatasetHandle: SafeHandle
    {
        static readonly DatasetHandle _zero = new DatasetHandle();

        public static DatasetHandle Zero
        {
            get
            {
                return _zero;
            }
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            return DatasetMethods.LGBM_DatasetFree(this.handle) == 0;
        }

        DatasetHandle() : base(IntPtr.Zero, true)
        {
        }
    }



}

