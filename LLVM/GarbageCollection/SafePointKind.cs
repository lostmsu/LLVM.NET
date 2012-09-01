using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM.GarbageCollection
{
	[Flags]
	public enum SafePointKind: uint
	{
		None = 0,
		/// <summary>
		/// Backwards branch
		/// </summary>
		Loop = 1,
		/// <summary>
		/// Return instruction
		/// </summary>
		Return = 2,
		/// <summary>
		/// Call instruction
		/// </summary>
		PreCall = 4,
		/// <summary>
		/// Return address of a call
		/// </summary>
		PostCall = 8,
	}
}
