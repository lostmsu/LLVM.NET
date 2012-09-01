using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LLVM
{
	public class Target
	{
		/// <summary>
		/// 
		/// </summary>
		/// <returns>false if there was no errors, else true</returns>
		public static void InitializeNative()
		{
			if (llvm.InitializeNative())
				throw new NotSupportedException("There's no native platform here");
		}
	}
}
