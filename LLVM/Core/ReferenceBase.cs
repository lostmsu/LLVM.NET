using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM {
	public abstract class ReferenceBase: IEquatable<ReferenceBase> {
		IntPtr reference;

		protected internal ReferenceBase(IntPtr reference)
		{
			if (reference == IntPtr.Zero)
				throw new ArgumentNullException("reference");

			this.reference = reference;
		}

		//protected internal ReferenceBase(Func<IntPtr> source)
		//{
		//    if (source == null)
		//        throw new ArgumentNullException("source");

		//    this.reference = source();
		//    if (this.reference == null)
		//        throw new ArgumentNullException("reference");
		//}

		public static implicit operator IntPtr(ReferenceBase reference)
		{
			return reference.reference;
		}

		#region IEquatable<ReferenceBase> Members

		public bool Equals(ReferenceBase other)
		{
			if (other == null) return false;
			return reference == other.reference;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ReferenceBase);
		}

		public override int GetHashCode()
		{
			return reference.GetHashCode() ^ 0x12415BA;
		}
		#endregion

		public override string ToString()
		{
			return string.Format("{0}: {1}", this.GetType().Name, reference);
		}
	}
}
