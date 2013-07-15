using System;
using LLVM.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LLVM.Tests.Interop
{
	[TestClass]
	public class ManagedTransitionTests
	{
		static readonly ClrInterop.Managed interop = new ClrInterop.Managed();

		#region Primitive
		[TestMethod]
		public void PrimitiveIn()
		{
			WrapTest<PrimitiveInDelegate, PrimitiveInDelegateConverted>();
		}

		[TestMethod]
		public void PrimitiveRef()
		{
			WrapTest<PrimitiveRefDelegate, PrimitiveRefDelegate>();
		}
		[TestMethod]
		public void PrimitiveOut()
		{
			WrapTest<PrimitiveOutDelegate, PrimitiveOutDelegate>();
		}
		[TestMethod]
		public void PrimitiveRet()
		{
			WrapTest<PrimitiveRetDelegate, PrimitiveRetDelegateConverted>();
		}
		#endregion

		#region Struct
		[TestMethod]
		public void StructIn()
		{
			WrapTest<StructInDelegate, StructInDelegateConverted>();
		}

		[TestMethod]
		public void StructRef()
		{
			WrapTest<StructRefDelegate, StructRefDelegate>();
		}
		[TestMethod]
		public void StructOut()
		{
			WrapTest<StructOutDelegate, StructOutDelegate>();
		}
		[TestMethod]
		public void StructRet()
		{
			WrapTest<StructRetDelegate, StructRetDelegateConverted>();
		}
		#endregion

		#region Class
		[TestMethod]
		public void ClassIn()
		{
			WrapTest<ClassInDelegate, ClassInDelegate>();
		}

		[TestMethod]
		public void ClassRef()
		{
			WrapTest<ClassRefDelegate, ClassRefDelegate>();
		}
		[TestMethod]
		public void ClassOut()
		{
			WrapTest<ClassOutDelegate, ClassOutDelegate>();
		}
		[TestMethod]
		public void ClassRet()
		{
			WrapTest<ClassRetDelegate, ClassRetDelegate>();
		}
		#endregion

		static void WrapTest<TOriginal, TExpected>()
		{
			var actual = interop.WrapDelegateType(typeof(TOriginal));
			AssertDelegateTypesEqual(typeof(TExpected), actual);
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

		delegate void StructInDelegateConverted(ref SampleStruct parameter);
		delegate void StructRetDelegateConverted(out SampleStruct parameter);
		#endregion

		struct SampleStruct{
			private int Int;
			public object Obj;
		}
	}
}
