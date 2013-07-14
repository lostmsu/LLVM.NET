using System;
using LLVM.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLVM.Tests.Interop
{
	[TestClass]
	public class ClrInteropTests
	{
		static readonly ClrInterop.Managed interop = new ClrInterop.Managed();

		[TestMethod]
		public void PrimitiveIn()
		{
			var actual = interop.WrapDelegateType(typeof(PrimitiveInDelegate));
			AssertDelegateTypesEqual(typeof(PrimitiveInDelegateConverted), actual);
		}

		static void AssertDelegateTypesEqual(System.Type expected, System.Type actual)
		{
			var invokeE = expected.GetMethod("Invoke");
			var invokeA = actual.GetMethod("Invoke");
			Assert.AreEqual(invokeA.ToString(), invokeE.ToString());
		}

		#region Delegate types
		delegate void PrimitiveInDelegate(int parameter);
		delegate void PrimitiveRefDelegate(ref int parameter);
		delegate void PrimitiveOutDelegate(out int parameter);
		delegate int PrimitiveRetDelegate();

		delegate void PrimitiveInDelegateConverted(ref int parameter);
		delegate void PrimitiveRetDelegateConverted(out int result);

		delegate void ClassInDelegate(string parameter);
		delegate void ClassRefDelegate(ref string parameter);
		delegate void ClassOutDelegate(out string parameter);
		delegate string ClassRetDelegate();

		delegate void StructInDelegate(SampleStruct parameter);
		delegate void StructRefDelegate(ref SampleStruct parameter);
		delegate void StructOutDelegate(out SampleStruct parameter);
		delegate SampleStruct StructRetDelegate();
		#endregion

		struct SampleStruct{
			private int Int;
			public object Obj;
		}
	}
}
