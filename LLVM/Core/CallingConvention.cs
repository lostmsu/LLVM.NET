using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InteropCallingConvention = System.Runtime.InteropServices.CallingConvention;

namespace LLVM
{
	public enum CallingConvention
	{
		C = 0,
		Cdecl = 0,
		Fast = 8,
		Cold = 9,
		GHC = 10,
	
		StdCallX86 = 64,
		FirstTargetSpecific = 64,
		FastCallX86 = 65,
		ApcsARM = 66,
		EABI = 67,
		EABI_VFP = 68,
		MSP430_INTR = 69,
	}

	public static class CallingConventionHelpers
	{
		public static CallingConvention ToLLVM(this InteropCallingConvention convention)
		{
			switch (convention) {
			case InteropCallingConvention.Cdecl:
				return CallingConvention.Cdecl;
			case InteropCallingConvention.FastCall:
				return CallingConvention.FastCallX86;
			case InteropCallingConvention.StdCall:
				return CallingConvention.StdCallX86;
			case InteropCallingConvention.ThisCall:
				throw new NotSupportedException();
			default:
				throw new NotImplementedException(convention.ToString());
			}
		}
	}
}
