using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLVM.NativeLibrary.Test
{
    [TestClass]
    public class LLVMDLLTest
    {
        [TestMethod]
        public void TestDllLoad()
        {
            LLVMDLL.Load();
        }
    }
}
