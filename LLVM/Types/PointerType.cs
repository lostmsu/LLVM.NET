using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace LLVM {
	public class PointerType: SequentialType {
		internal PointerType(IntPtr typeref) : base(typeref) { }

		public static PointerType Get(Type valueType, int addressSpace = 0)
		{
			var typeref = llvm.GetPointerType(valueType, addressSpace);
			return new PointerType(typeref);
		}

		public Type ElementType
		{
			get
			{
				var elementType = llvm.GetElementType(this);
				return Type.DetectType(elementType);
			}
		}

		public override string ToString() {
			var elementType = ElementType;
			if (elementType is DerivedType
				&& !(elementType is PointerType))
				return elementType.GetType().Name + "*";
			return ElementType + "*";
		}

		public override bool StructuralEquals(Type obj) {
			if (obj == null && this == null) return true;
			var other = obj as PointerType;
			if (other == null) return false;
			return this.ElementType.Equals(other.ElementType);
		}
	}
}
