using System;
using System.Collections.Generic;
using System.Linq;

namespace LLVM {
	public class FunctionType: DerivedType {
		internal FunctionType(IntPtr typeref) : base(typeref) { }

		static IntPtr functionType(Type ret, Type[] args = null, bool vararg = false)
		{
			if (args == null)
				args = new Type[] { };

			return llvm.FunctionType(ret, args.Select(arg => (IntPtr)arg).ToArray(), args.Length, vararg);
		}

		public FunctionType(Type ret, Type[] args = null, bool vararg = false)
			: base(functionType(ret, args, vararg))
		{
		}

		public int ArgumentCount
		{
			get
			{
				return llvm.GetArgumentCount(this);
			}
		}

		public Type[] ArgumentTypes
		{
			get
			{
				IntPtr[] result = new IntPtr[ArgumentCount];
				llvm.GetArgumentTypes(this, result);
				return result.Select(DetectType).ToArray();
			}
		}

		public Type ReturnType
		{
			get
			{
				return DetectType(llvm.GetReturnType(this));
			}
		}

		public override string ToString()
		{
			return ArgumentCount > 0
					? String.Join(" * ", ArgumentTypes.Select(t => t.ToString())) + " -> " + ReturnType
					: "void" + " -> " + ReturnType;
		}

		public override bool Equals(object obj) {
			var other = obj as FunctionType;
			if (other == null) return false;
			if (!other.ReturnType.Equals(ReturnType)) return false;
			return ArgumentTypes.SequenceEqual(other.ArgumentTypes);
		}
	}
}
