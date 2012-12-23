namespace cscodec.h264.util
{
	#if false
	public class IOUtils {
	
		public static sbyte[] toByteArray(InputStream input) throws IOException {
			ByteArrayOutputStream output = new ByteArrayOutputStream();
		
			sbyte[] buffer = new sbyte[4096];
			int n = 0;
			while (-1 != (n = input.read(buffer))) {
				output.write(buffer, 0, n);
			} // while

			return output.toByteArray();
		}

		public static void closeQuietly(InputStream  input) {
			if( input == null ) {
				return;
			}
			try {
				input.close();
			} catch( IOException  ioe ) {
			}
		}

		public static void closeQuietly(OutputStream  output) {
			if( output == null ) {
				return;
			}
			try {
				output.close();
			} catch( IOException  ioe ) {
			}
		}
	
	}
#endif
}