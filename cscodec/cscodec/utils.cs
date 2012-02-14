using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cscodec
{
	public struct IntFloat
	{
		private byte[] Value;

		public float f
		{
			get
			{
				return BitConverter.ToSingle(Value, 0);
			}
			set
			{
				Value = BitConverter.GetBytes(value);
			}
		}

		public uint v
		{
			get
			{
				return BitConverter.ToUInt32(Value, 0);
			}
			set
			{
				Value = BitConverter.GetBytes(value);
			}
		}
	}
}
