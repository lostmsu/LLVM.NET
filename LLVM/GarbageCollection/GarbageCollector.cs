using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace LLVM.GarbageCollection
{
	public abstract class GarbageCollector
	{
		protected GarbageCollector()
		{
			findCustomSafePoints = 
				Marshal.GetFunctionPointerForDelegate(
					new NativeGlue.FindCustomSafePoints(FindCustomSafePoints));
			initializeCustomLowering =
				Marshal.GetFunctionPointerForDelegate(
					new NativeGlue.InitializeCustomLowering(InitializeCustomLowering));
			performCustomLowering =
				Marshal.GetFunctionPointerForDelegate(
					new NativeGlue.PerformCustomLowering(PerformCustomLowering));
		}

		protected SafePointKind NeededSafePoints;
		protected bool CustomReadBarriers;   //< Default is to insert loads.
		protected bool CustomWriteBarriers;  //< Default is to insert stores.
		protected bool CustomRoots;          //< Default is to pass through to backend.
		protected bool CustomSafePoints;     //< Default is to use NeededSafePoints
		//  to find safe points.
		protected bool InitRoots = true;            //< If set, roots are nulled during lowering.
		protected bool UsesMetadata;         //< If set, backend must emit metadata tables.

		protected abstract bool InitializeCustomLowering(Module module);
		private bool InitializeCustomLowering(IntPtr module)
		{
			return InitializeCustomLowering(new Module(module));
		}
		protected abstract bool PerformCustomLowering(Function function);
		private bool PerformCustomLowering(IntPtr function)
		{
			return PerformCustomLowering(new Function(function));
		}
		protected abstract bool FindCustomSafePoints(IntPtr functionInfo, IntPtr machineFunction);

		public abstract string Name { get; }

		// TODO: DISPOSE!
		readonly IntPtr findCustomSafePoints;
		readonly IntPtr initializeCustomLowering;
		readonly IntPtr performCustomLowering;

		private NativeGlue.ExternalGCInfo GetExternalGCInfo()
		{
			return new NativeGlue.ExternalGCInfo {
				CustomReadBarriers = this.CustomReadBarriers,
				CustomRoots = this.CustomRoots,
				CustomSafePoints = this.CustomSafePoints,
				CustomWriteBarriers = this.CustomWriteBarriers,
				FindCustomSafePoints = findCustomSafePoints,
				InitializeCustomLowering = initializeCustomLowering,
				InitRoots = this.InitRoots,
				NeededSafePoints = this.NeededSafePoints,
				PerformCustomLowering = performCustomLowering,
				UsesMetadata = this.UsesMetadata,
			};
		}

		static readonly List<GarbageCollector> collectors = new List<GarbageCollector>();
		static readonly Dictionary<string, NativeGlue.GCStrategyConstructor>
			constructors = new Dictionary<string, NativeGlue.GCStrategyConstructor>();

		public static void Register<GC>()
			where GC: GarbageCollector, new()
		{
			var ctor = new NativeGlue.GCStrategyConstructor(
					() => {
						lock(collectors){
							var gc = new GC();
							var gcPtr = llvm.CreateGC(gc.GetExternalGCInfo());
							collectors.Add(gc);
							return gcPtr;
						}
					}
				);

			var name = typeof(GC).FullName;
			var namePtr = Marshal.StringToHGlobalAnsi(name);
			lock(constructors){
				constructors.Add(name, ctor);
				try {
					llvm.RegisterGC(namePtr, IntPtr.Zero, ctor);
				} catch (Exception) {
					constructors.Remove(name);
					throw;
				}
			}
		}
	}
}
