namespace cscodec.h243.decoder
{
	public class VLC
	{
		public int bits;
		public short[][] table_base; // VLC_TYPE (*table)[2]; ///< code, bits
		public int table_offset;
		public int table_size, table_allocated;
	}
}