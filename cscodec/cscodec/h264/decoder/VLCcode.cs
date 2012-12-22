namespace cscodec.h243.decoder
{
	public class VLCcode
	{
		/*
		uint8_t bits;
		uint16_t symbol;
		 * codeword, with the first bit-to-be-read in the msb
		 * (even if intended for a little-endian bitstream reader) 
		uint32_t code;
		*/
    
		public int bits;
		public int symbol;
		public long code;
    
		public int compareTo(object arg0) {
			// TODO Auto-generated method stub
			if(arg0 == null || !(arg0 is VLCcode)) return 0;
			return (int)(this.code - ((VLCcode)arg0).code);
		}
	}
}
