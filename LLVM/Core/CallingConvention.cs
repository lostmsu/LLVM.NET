using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
}
