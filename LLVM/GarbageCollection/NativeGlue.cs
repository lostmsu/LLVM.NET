using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

using CC = System.Runtime.InteropServices.CallingConvention;

namespace LLVM.GarbageCollection.NativeGlue
{
	[StructLayout(LayoutKind.Sequential)]
	struct ExternalGCInfo
	{
		public SafePointKind NeededSafePoints;
		public bool CustomReadBarriers;
		public bool CustomWriteBarriers;
		public bool CustomRoots;
		public bool CustomSafePoints;

		public bool InitRoots;
		public bool UsesMetadata;

		public IntPtr InitializeCustomLowering;
		public IntPtr PerformCustomLowering;
		public IntPtr FindCustomSafePoints;
	}

	[UnmanagedFunctionPointer(CC.Cdecl)]
	delegate bool InitializeCustomLowering(IntPtr module);
	[UnmanagedFunctionPointer(CC.Cdecl)]
	delegate bool PerformCustomLowering(IntPtr function);
	[UnmanagedFunctionPointer(CC.Cdecl)]
	delegate bool FindCustomSafePoints(IntPtr functionInfo, IntPtr machineFunction);
	[UnmanagedFunctionPointer(CC.Cdecl)]
	delegate IntPtr GCStrategyConstructor();
}
