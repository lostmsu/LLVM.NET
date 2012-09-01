using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public class ArrayType: CompositeType
	{
		internal ArrayType(IntPtr typeref) : base(typeref) { }

		public ArrayType(Type elementType, int elementCount = 0):
			this(arrayType(elementType, elementCount))
		{
		}

		static IntPtr arrayType(Type elemType, int elementCount)
		{
			return llvm.ArrayType(elemType, elementCount);
		}

		public Type ElementType
		{
			get
			{
				var elementType = llvm.GetElementType(this);
				return Type.DetectType(elementType);
			}
		}

		public override string ToString()
		{
			var elementType = ElementType;
			if (elementType is DerivedType)
				return '[' + elementType.GetType().Name + ']';
			return '[' + ElementType.ToString() + ']';
		}
	}
}
