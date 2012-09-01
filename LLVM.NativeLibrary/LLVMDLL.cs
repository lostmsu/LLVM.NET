using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Lost.Native;

namespace LLVM.NativeLibrary
{
    public static class LLVMDLL
    {
        public static void Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            DynamicLibrary.Load(assembly, "LLVM-3.1", "LLVM.NativeLibrary");
        }
    }
}
