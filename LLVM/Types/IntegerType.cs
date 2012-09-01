using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class IntegerType: DerivedType {
		internal IntegerType(IntPtr typeref) : base(typeref) { }

		public static IntegerType GetInt32(Context context)
		{
			var type = llvm.GetInt32(context);
			return new IntegerType(type);
		}

		public static IntegerType Get(Context context, int bits)
		{
			var type = llvm.GetInt(context, bits);
			return new IntegerType(type);
		}

		public IntegerConstant Constant(ulong value, bool sign)
		{
			IntPtr constant = llvm.Constant(this, value, sign);
			return new IntegerConstant(constant);
		}

		/// <summary>
		/// Bit count
		/// </summary>
		public int Width
		{
			get
			{
				return llvm.GetWidth(this);
			}
		}

		public override string ToString()
		{
			return "i" + Width;
		}

		public override bool StructuralEquals(Type obj) {
			if (this == null && obj == null) return true;
			var other = obj as IntegerType;
			if (other == null) return false;
			return this.Width == other.Width;
		}
	}
}
