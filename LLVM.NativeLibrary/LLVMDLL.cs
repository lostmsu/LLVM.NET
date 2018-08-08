namespace LLVM.NativeLibrary {
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    using Lost.Native;

    public static class LLVMDLL
    {
        const int ERROR_MOD_NOT_FOUND = 0x0000007E;
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            try {
                DynamicLibrary.Load(assembly, "LLVM-3.3", "LLVM.NativeLibrary");
            } catch (Win32Exception e) when (e.NativeErrorCode == ERROR_MOD_NOT_FOUND && !IsVisualCRuntimeAvailable()) {
                throw new NotSupportedException("The latest Visual C++ 2015 redistributable must be installed", innerException: e);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static bool IsVisualCRuntimeAvailable() =>
            LoadLibrary("MSVCP140") == IntPtr.Zero
            || LoadLibrary("VCRUNTIME140.dll") == IntPtr.Zero;

        [DllImport("kernel32")]
        static extern IntPtr LoadLibrary(string path);
    }
}
