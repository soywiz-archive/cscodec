namespace cscodec.h243.util
{

	public class FilterInputStream : InputStream {

		protected volatile InputStream inputStream;
	
		public FilterInputStream(InputStream is) {
			inputStream = is;
		}
	
		public long skip(long length) throws IOException {
			long ret = inputStream.skip(length);
			return ret;
		}

		////
		public int read() throws IOException {
			// TODO Auto-generated method stub
			int ret = inputStream.read();
			return ret;
		}
	
		public int read(sbyte b[]) throws IOException  {
			int cnt =  inputStream.read(b);
			return cnt;
		}
 
		 public int read(sbyte b[], int off, int len) throws IOException  {
    		int cnt = inputStream.read(b, off, len);
			return cnt;
		 }
  
		 public int available() throws IOException  {
    		 return inputStream.available();
		 }
 
		 public void close() throws IOException  {
    		 inputStream.close();
		 }
 
		 public synchronized void mark(int readlimit) {
    		 inputStream.mark(readlimit);
		 }
 
		 public synchronized void reset() throws IOException  {
    		 inputStream.reset();
		 }
 
		 public boolean markSupported() {
    		 return inputStream.markSupported();
		 }

	}
}