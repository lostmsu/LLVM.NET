using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public class FloatType: DerivedType
	{
		internal FloatType(IntPtr typeref): base(typeref){}

		public static FloatType Get(Context context, int bits)
		{
			IntPtr type;
			switch (bits)
			{
			case 32:
				type = llvm.GetFloat(context);
				break;

			case 64:
				type = llvm.GetDouble(context);
				break;

			default:
				throw new NotSupportedException("Floats of size " + bits + " bits");
			}
			return new FloatType(type);
		}
	}
}
