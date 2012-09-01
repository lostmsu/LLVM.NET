using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class StructType: CompositeType {
		internal StructType(IntPtr typeref) : base(typeref) { }

		static IntPtr structType(Context context, IEnumerable<Type> elementTypes, bool packed = false)
		{
			if (elementTypes == null)
				elementTypes = new Type[] { };

			var typesArray = elementTypes.Select(type => (IntPtr)type).ToArray();

			return llvm.StructType(context, typesArray, typesArray.Length, packed);
		}

		static IntPtr structType(Context context)
		{
			return llvm.StructCreateEmptyType(context);
		}

		static IntPtr structType(Context context, string name)
		{
			return llvm.StructType(context, name);
		}

		public StructType(Context context, IEnumerable<Type> elementTypes, bool packed = false)
			: base(structType(context, elementTypes, packed)) { }

		public StructType(Context context, string name, IEnumerable<Type> elementTypes, bool packed = false)
			: base(structType(context, name)) {
			this.SetBody(elementTypes, packed);
		}

		public StructType(Context context)
			: base(structType(context)) { }

		public StructType(Context context, string name)
			: base(structType(context, name)) { }

		public bool IsPacked
		{
			get
			{
				return llvm.IsPackedStruct(this);
			}
		}

		public bool IsOpaque
		{
			get
			{
				return llvm.IsOpaqueStruct(this);
			}
		}

		public uint FieldCount
		{
			get
			{
				return llvm.StructFieldCount(this);
			}
		}

		public Type[] FieldTypes
		{
			get
			{
				IntPtr[] typerefs = new IntPtr[FieldCount];
				llvm.StructElements(this, typerefs);
				return typerefs.Select(Type.DetectType).ToArray();
			}

			set
			{
				
			}
		}

		public void SetBody(IEnumerable<Type> fieldTypes, bool packed = false)
		{
			if (fieldTypes == null)
				fieldTypes = new Type[] { };

			var typesArray = fieldTypes.Select(type => (IntPtr)type).ToArray();

			llvm.StructSetBody(this, typesArray, typesArray.Length, packed);
		}

		public override string ToString() {
			Type[] elements = FieldTypes;
			if (elements.Length == 0) return "{}";

			var result = new StringBuilder("{ " + elements[0]);
			for (int i = 1; i < elements.Length; i++)
				result.Append(" * " + elements[i]);
			result.Append(" }");

			return result.ToString();
		}

		public override bool StructuralEquals(Type obj) {
			if (this == null && obj == null) return true;
			var other = obj as StructType;
			if (other == null) return false;
			if (other.IsPacked != this.IsPacked) return false;
			return this.FieldTypes.SequenceEqual(other.FieldTypes);
		}
	}
}
