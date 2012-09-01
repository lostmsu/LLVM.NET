using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;

namespace LLVM {
	public class InstructionBuilder: ReferenceBase {
		#region Constructors
		internal InstructionBuilder(IntPtr builderref) : base(builderref) { }

		public InstructionBuilder(Context context) : base(llvm.CreateBuilder(context)) { }
		public InstructionBuilder(Context context, Block block) : base(llvm.CreateBuilder(context))
		{
			PointToEnd(block);
		}
		#endregion Constructors

		#region Utility

		static void CheckBinaryIntegerOpArguments(Value left, Value right)
		{
			if (left == null) throw new ArgumentNullException("left");
			if (right == null) throw new ArgumentNullException("right");

			CheckIntegerType(left.Type, "left");
			CheckIntegerType(right.Type, "right");
		}

		static void CheckIntegerType(Type type, string value)
		{
			if (!(type is IntegerType)) throw new ArgumentException("Value must be of integer type", value);
		}

		static void CheckPointerType(Value value, string valueName)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (!(value.Type is PointerType)) throw new ArgumentException("Value must be of pointer type", valueName);
		}

		#endregion Utility

		#region Comparison
		public Value Compare(IntegerComparison comparison, Value left, Value right, string name = "")
		{
			if (left == null) throw new ArgumentNullException("left");
			if (right == null) throw new ArgumentNullException("right");
			if (left.Type.Kind != TypeKind.Pointer && right.Type.Kind != TypeKind.Pointer)
				CheckBinaryIntegerOpArguments(left, right);

			if (!left.Type.Equals(right.Type))
				throw new ArgumentException("Type mismatch: "
					+ left.Type + " vs " + right.Type);

			var valueref = llvm.EmitCompare(this, comparison, left, right, name);
			return new Value(valueref);
		}

		public Value IsNull(Value value, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");

			var valueref = llvm.EmitIsNull(this, value, name);
			return new Value(valueref);
		}
		#endregion Comparison

		#region Control Flow
		public Branch If(Value condition, Block ontrue, Block onfalse)
		{
			if (condition == null) throw new ArgumentNullException("condition");
			if (ontrue == null) throw new ArgumentNullException("ontrue");
			if (onfalse == null) throw new ArgumentNullException("onfalse");

			if (condition.Type.Kind != TypeKind.Integer ||
				(condition.Type as IntegerType).Width != 1)
				throw new ArgumentException("Invalid condition type. Must be i1");

			var valueref = llvm.EmitIf(this, condition, ontrue, onfalse);
			return new Branch(valueref);
		}

		public Switch Switch(Value value, Block elseCase, Tuple<Value, Block>[] branches)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (elseCase == null) throw new ArgumentNullException("elseCase");
			if (branches == null) throw new ArgumentNullException("branches");

			var valueref = llvm.EmitSwitch(this, value, elseCase, branches.Length);
			var @switch = new Switch(valueref);
			foreach (var branch in branches)
				@switch.Add(branch.Item1, branch.Item2);
			return @switch;
		}

		public Branch GoTo(Block dest)
		{
			if (dest == null) throw new ArgumentNullException("dest");

			var valueref = llvm.EmitGoTo(this, dest);
			return new Branch(valueref);
		}

		#region Calls

		public Call Call(Function target, Value[] args, string name = "")
		{
			if (args == null) args = new Value[]{};
			var argtypes = target.Type.ArgumentTypes;
			CheckArgumentTypes(args, argtypes);
			var valueref = llvm.EmitCall(this, target, args.Select(val => (IntPtr)val).ToArray(), args.Length, name);
			return new Call(valueref);
		}

		private static void CheckArgumentTypes(Value[] args, Type[] argtypes) {
			if (argtypes.Length != args.Length) {
				var msg = string.Format("Incorrect number of arguments. Expecting: {0}, given: {1}", argtypes.Length, args.Length);
				throw new InvalidProgramException(msg);
			}
			for (int i = 0; i < argtypes.Length; i++)
				if (!argtypes[i].Equals(args[i].Type)) {
					var msg = string.Format("Incorrect argument {0} type. Expecting: {1}, given: {2}", i, argtypes[i], args[i].Type);
					throw new InvalidProgramException(msg);
				}
		}

		public Call Call(Function target, params Value[] args)
		{
			var arguments = args.Select(val => (IntPtr)val).ToArray();
			CheckArgumentTypes(args, target.Type.ArgumentTypes);
			var valueref = llvm.EmitCall(this, target, arguments, args.Length, "");
			return new Call(valueref) {
				CallingConvention = target.CallingConvention,
			};
		}

		public Return Return(Value value)
		{
			IntPtr valueref = llvm.EmitReturn(this, value);
			return new Return(valueref);
		}

		public Return Return()
		{
			IntPtr valueref = llvm.EmitReturn(this);
			return new Return(valueref);
		}
		#endregion Calls
		#endregion Control Flow

		#region Arithmetics
		public Value Add(Value left, Value right, string name = "")
		{
			CheckBinaryIntegerOpArguments(left, right);

			var valueref = llvm.EmitAdd(this, left, right, name);
			return new Value(valueref);
		}

		public Value AddFloat(Value left, Value right, string name = "")
		{
			var valueref = llvm.EmitAddFloat(this, left, right, name);
			return new Value(valueref);
		}

		public Value Subtract(Value left, Value right, string name = "")
		{
			var valueref = llvm.EmitSubtract(this, left, right, name);
			return new Value(valueref);
		}

		public Value Multiply(Value left, Value right, string name = "")
		{
			var valueref = llvm.EmitMultiply(this, left, right, name);
			return new Value(valueref);
		}

		public Value Divide(bool signed, Value left, Value right, string name = "")
		{
			IntPtr valueref = signed
				? llvm.EmitDivideSigned(this, left, right, name)
				: llvm.EmitDivideUnsigned(this, left, right, name);
			return new Value(valueref);
		}

		public Value Reminder(bool signed, Value left, Value right, string name = "")
		{
			IntPtr valueref = signed
				? llvm.EmitReminderSigned(this, left, right, name)
				: llvm.EmitReminderUnsigned(this, left, right, name);
			return new Value(valueref);
		}

		public Value Negate(Value value, string name = "")
		{
			var valueref = llvm.EmitNegate(this, value, name);
			return new Value(valueref);
		}
		#endregion Arithmetics

		#region Logics
		public Value And(Value left, Value right, string name = "")
		{
			CheckBinaryIntegerOpArguments(left, right);

			var valueref = llvm.EmitAnd(this, left, right, name);
			return new Value(valueref);
		}

		public Value Or(Value left, Value right, string name = "")
		{
			CheckBinaryIntegerOpArguments(left, right);

			var valueref = llvm.EmitOr(this, left, right, name);
			return new Value(valueref);
		}

		public Value Xor(Value left, Value right, string name = "")
		{
			CheckBinaryIntegerOpArguments(left, right);

			var valueref = llvm.EmitXor(this, left, right, name);
			return new Value(valueref);
		}

		public Value Not(Value value, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");
			CheckIntegerType(value.Type, "value");

			var valueref = llvm.EmitNot(this, value, name);
			return new Value(valueref);
		}
		#endregion

		#region Shifts
		public Value ShiftLeft(Value value, Value shiftBy, string name = "")
		{
			CheckShiftArguments(value, shiftBy);

			var valueref = llvm.EmitShiftLeft(this, value, shiftBy, name);
			return new Value(valueref);
		}

		private static void CheckShiftArguments(Value value, Value shiftBy)
		{
			if (value == null) throw new ArgumentNullException("value");
			CheckIntegerType(value.Type, "value");
			if (shiftBy == null) throw new ArgumentNullException("shiftBy");
			CheckIntegerType(shiftBy.Type, "shiftBy");
		}

		public Value ShiftRight(bool signed, Value value, Value shiftBy, string name = "")
		{
			CheckShiftArguments(value, shiftBy);

			var valueref = signed
				? llvm.EmitShiftRightSigned(this, value, shiftBy, name)
				: llvm.EmitShiftRightUnsigned(this, value, shiftBy, name);
			return new Value(valueref);
		}
		#endregion

		#region Memory
		public StackAlloc StackAlloc(Type type, string name = "")
		{
			var valueref = llvm.EmitStackAlloc(this, type, name);
			return new StackAlloc(valueref);
		}

		public StackAlloc StackAlloc(Type type, Value arraySize, string name = "")
		{
			var valueref = llvm.EmitStackAlloc(this, type, arraySize, name);
			return new StackAlloc(valueref);
		}

		public Store Store(Value value, Value pointer)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (pointer == null) throw new ArgumentNullException("pointer");

			var ptype = pointer.Type as PointerType;
			if (ptype == null) throw new ArgumentException("Value must be of pointer type", "pointer");

			if (!value.Type.Equals(ptype.ElementType))
				throw new InvalidProgramException(string.Format(
					"Can't store {0} to {1} location", value.Type, ptype.ElementType));

			var valueref = llvm.EmitStore(this, value, pointer);
			return new Store(valueref);
		}

		public Load Load(Value pointer, string name = "")
		{
			CheckPointerType(pointer, "pointer");

			var valueref = llvm.EmitLoad(this, pointer, name);
			return new Load(valueref);
		}

		public Value StructureElement(Value pointer, int index, string name = "")
		{
			CheckPointerType(pointer, "pointer");

			var valueref = llvm.EmitStructElementPointer(this, pointer, index, name);
			return new Value(valueref);
		}

		public Value Element(Value pointer, Value[] indexes, string name = "")
		{
			CheckPointerType(pointer, "pointer");
			if (indexes == null) throw new ArgumentNullException("indexes");

			var idxs = indexes.Select(i => (IntPtr)i).ToArray();
			var valueref = llvm.EmitGetElementPointer(this, pointer, idxs, indexes.Length, name);
			return new Value(valueref);
		}
		#endregion Memory

		#region Conversions
		public Value Trunc(Value value, Type destType, string name = "")
		{
			var valueref = llvm.EmitTrunc(this, value, destType, name);
			return new Value(valueref);
		}

		public Value ZeroExtend(Value value, Type destType, string name = "")
		{
			var valueref = llvm.EmitZeroExtend(this, value, destType, name);
			return new Value(valueref);
		}

		public Value SignExtend(Value value, Type destType, string name = "")
		{
			var valueref = llvm.EmitSignExtend(this, value, destType, name);
			return new Value(valueref);
		}

		public Value PointerCast(Value value, PointerType destType, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");
			if (value.Type.Kind != TypeKind.Pointer)
				throw new ArgumentException("value must be of pointer type", "value");
			if (destType == null) throw new ArgumentNullException("destType");

			var valueref = llvm.EmitPointerCast(this, value, destType, name);
			return new Value(valueref);
		}

		public Value IntToPointer(Value value, PointerType destType, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");
			if (destType == null) throw new ArgumentNullException("destType");

			if (value.Type.Kind != TypeKind.Integer)
				throw new ArgumentException("Value must be of integer type", "value");

			var valueref = llvm.EmitIntToPtr(this, value, destType, name);
			return new Value(valueref);
		}

		public Value PointerToInt(Value value, IntegerType destType, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");
			if (destType == null) throw new ArgumentNullException("destType");

			if (value.Type.Kind != TypeKind.Pointer)
				throw new ArgumentException("Value must be of pointer type", "value");

			var valueref = llvm.EmitPtrToInt(this, value, destType, name);
			return new Value(valueref);
		}

		public Value BitCast(Value value, Type destType, string name = "")
		{
			if (value == null) throw new ArgumentNullException("value");
			if (destType == null) throw new ArgumentNullException("destType");

			var valueref = llvm.EmitBitCast(this, value, destType, name);
			return new Value(valueref);
		}
		#endregion

		#region Structures
		public Value Insert(Value into, Value what, int index, string name = "")
		{
			if (into == null) throw new ArgumentNullException("into");
			if (what == null) throw new ArgumentNullException("what");

			var valueref = llvm.EmitInsert(this, into, what, index, name);
			return new Value(valueref);
		}

		public Value Extract(Value from, int index, string name = "")
		{
			if (from == null) throw new ArgumentNullException("from");

			var valueref = llvm.EmitExtract(this, from, index, name);
			return new Value(valueref);
		}
		#endregion

		#region Intrinsics
		public Call GCRoot(StackAlloc value, Module module)
		{
			if(value == null) throw new ArgumentNullException("value");
			if(!(value.Type is PointerType)) throw new ArgumentException();

			var i8p = PointerType.Get(IntegerType.Get(Context, 8));
			var i8pp = PointerType.Get(i8p);
			var arg = this.PointerCast(value, i8pp);

			var gcroot = module.GetIntrinsic("llvm.gcroot",
				new FunctionType(Type.GetVoid(Context), new[] { i8pp, i8p }));
			return this.Call(gcroot, arg, i8p.Zero);
		}
		#endregion

		public Context Context
		{
			get
			{
				var contextRef = llvm.GetBuilderContext(this);
				return new Context(contextRef);
			}
		}

		public void PointToEnd(Block block)
		{
			llvm.PointToEnd(this, block);
		}
	}
}
