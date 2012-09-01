using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public struct LayoutInfo
	{
		int size;
		int align;

		public LayoutInfo(int size, int align)
		{
			this.size = size;
			this.align = align;
		}

		public LayoutInfo(int size) : this(size, size) { }

		/// <summary>
		/// Size in bits
		/// </summary>
		public int Size
		{
			get { return size; }
			set { size = value; }
		}
		/// <summary>
		/// Align in bits
		/// </summary>
		public int Align
		{
			get { return align; }
			set { align = value; }
		}
	}
}
