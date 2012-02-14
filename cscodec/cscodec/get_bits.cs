using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cscodec
{
	/// <summary>
	/// get_bits.h
	/// </summary>
	public partial class VLC
	{
		public Object table;

		int bits;
		//VLC_TYPE (*table)[2]; ///< code, bits
		int table_size, table_allocated;

		internal void init_vlc(int HGAINVLCBITS, int p, byte[] ff_wma_hgain_huffbits, int p_2, int p_3, ushort[] ff_wma_hgain_huffcodes, int p_4, int p_5, int p_6)
		{
			throw new NotImplementedException();
		}

		internal void free_vlc()
		{
			throw new NotImplementedException();
		}

		internal unsafe void init_vlc(int VLCBITS, int n, byte* table_bits, int p, int p_2, uint* table_codes, int p_3, int p_4, int p_5)
		{
			throw new NotImplementedException();
		}

		internal unsafe void init_coef_vlc(ushort** p, float** p_2, ushort** p_3, CoefVLCTable coefVLCTable)
		{
			throw new NotImplementedException();
		}

		internal void init_vlc(int VLCBITS, int n, byte[] table_bits, int p, int p_2, uint[] table_codes, int p_3, int p_4, int p_5)
		{
			throw new NotImplementedException();
		}

		internal void init_coef_vlc(ushort p, float p_2, ushort p_3, global::cscodec.wma.wma.CoefVLCTable globalcscodecwmawmaCoefVLCTable)
		{
			throw new NotImplementedException();
		}
	}
}
