using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace LLVM.GarbageCollection
{
	using CC = System.Runtime.InteropServices.CallingConvention;

	public class ShadowStack: GarbageCollector
	{
		const string rootName = "llvm_gc_root_chain";

		/// <summary>
		/// Shadow stack GC roots visitor procedure type
		/// </summary>
		/// <param name="root">Double pointer to the root</param>
		/// <param name="meta">A pointer to the root's meta information</param>
		[UnmanagedFunctionPointer(CC.Cdecl)]
		public delegate void RootVisitor(IntPtr root, IntPtr meta);

		readonly RootVisitor rootVisitor;
		readonly GlobalValue rootPointer;
		readonly ExecutionEngine engine;
		readonly Module module;

		public ShadowStack(ExecutionEngine engine, Module module, RootVisitor rootVisitor)
		{
			Contract.Requires<ArgumentNullException>(rootVisitor != null);
			Contract.Requires<ArgumentNullException>(engine != null);
			Contract.Requires<ArgumentNullException>(module != null);

			this.rootVisitor = rootVisitor;
			this.module = module;
			this.engine = engine;
			rootPointer = module.AddGlobal(PointerType.GetVoid(module.Context), rootName);

		}

		public void VisitRoots()
		{
			VisitRoots(rootVisitor);
		}

		#region Unsafe structs
		struct FrameMap
		{
			public int RootCount;
			public int MetaCount;
			public IntPtr Meta;
		}

		struct StackEntry
		{
			public IntPtr Next;
			public IntPtr FrameMap;
			public IntPtr Roots;

			public StackEntry GetNext()
			{
				Contract.Requires<InvalidOperationException>(Next != IntPtr.Zero);

				return (StackEntry)Marshal.PtrToStructure(Next, typeof(StackEntry));
			}

			public FrameMap GetFrameMap()
			{
				Contract.Requires<InvalidOperationException>(FrameMap != IntPtr.Zero);

				return (FrameMap)Marshal.PtrToStructure(FrameMap, typeof(FrameMap));
			}
		}
		#endregion

		private StackEntry GetRoot()
		{
			var root = engine.GetValue<IntPtr>(rootPointer);
			return root.IsNull() ? new StackEntry { }
				: (StackEntry)Marshal.PtrToStructure(root, typeof(StackEntry));
		}

		public void VisitRoots(RootVisitor visitor)
		{
			Contract.Requires<ArgumentNullException>(rootVisitor != null);

			for (StackEntry roots = GetRoot();
				roots.FrameMap != IntPtr.Zero;
				roots = roots.GetNext()) {
				int i = 0;
				var frameMap = roots.GetFrameMap();
				for (int metas = frameMap.MetaCount; i < metas; i++)
					visitor(roots.Roots + i * IntPtr.Size, frameMap.Meta + i * IntPtr.Size);

				for (int rootCount = frameMap.RootCount; i < rootCount; i++)
					visitor(roots.Roots + i * IntPtr.Size, IntPtr.Zero);
			}
		}

		public Module Module
		{
			get
			{
				return module;
			}
		}

		protected override bool InitializeCustomLowering(Module module)
		{
			throw new NotSupportedException();
		}

		protected override bool PerformCustomLowering(Function function)
		{
			throw new NotSupportedException();
		}

		protected override bool FindCustomSafePoints(IntPtr functionInfo, IntPtr machineFunction)
		{
			throw new NotSupportedException();
		}

		public override string Name
		{
			get { return "shadow-stack"; }
		}
	}
}
