using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public class Type: ReferenceBase {

		internal Type(IntPtr typeref): base(typeref)
		{
			
		}

		static internal Type DetectType(IntPtr typeref)
		{
			var temp = new Type(typeref);
			switch (temp.Kind) {
			case TypeKind.Integer:
				return new IntegerType(typeref);

			case TypeKind.Function:
				return new FunctionType(typeref);

			case TypeKind.Pointer:
				return new PointerType(typeref);

			case TypeKind.Struct:
				return new StructType(typeref);

			case TypeKind.Array:
				return new ArrayType(typeref);

			case TypeKind.Void:
				return new Type(typeref);

			default:
				return new Type(typeref);
			}
		}

		public Context Context
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public static Type GetVoid(Context context)
		{
			return new Type(llvm.GetVoid(context));
		}

		public TypeKind Kind
		{
			get
			{
				return llvm.GetTypeKind(this);
			}
		}

		public Value UndefinedValue
		{
			get
			{
				if (Kind == TypeKind.Void) return Zero;
				var valueref = llvm.GetUndefinedValue(this);
				return new Value(valueref);
			}
		}

		public Constant Zero
		{
			get
			{
				var valueref = llvm.GetZero(this);
				return new Constant(valueref);
			}
		}

		public Value Size
		{
			get
			{
				return new Value(llvm.SizeOf(this));
			}
		}

		public Value Align
		{
			get
			{
				return new Value(llvm.AlignOf(this));
			}
		}

		public override string ToString() {
			return Kind.ToString();
		}

		public virtual bool StructuralEquals(Type obj) {
			if (this == null && obj == null) return true;
			var other = obj as Type;
			if (other == null) return false;
			return this.Kind == TypeKind.Void && other.Kind == TypeKind.Void;
		}
	}
}
