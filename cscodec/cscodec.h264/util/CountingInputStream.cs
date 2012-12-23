namespace cscodec.h264.util
{
	#if false
	public class CountingInputStream : InputStream {
		protected volatile InputStream inputStream;
		private int count;
		private long byteCount;
	
		public CountingInputStream(InputStream @is) {
			inputStream = @is;
		}
	
		public long getByteCount() {
			return byteCount;
		}
	
		public int getCount() {
			return count;
		}

		public long resetByteCount() {
			byteCount = 0;
			return byteCount;
		}

		public int resetCount() {
			count = 0;
			return count;
		}
	
		public long skip(long length) {
			long ret = inputStream.skip(length);
			byteCount += length;
			count += length;
			return ret;
		}

		////
		public int read() {
			// TODO Auto-generated method stub
			int ret = inputStream.read();
			count++;
			byteCount++;
			return ret;
		}
	
		public int read(sbyte b[]) {
			int cnt =  inputStream.read(b, 0, b.Length);
			count += cnt;
			byteCount += cnt;
			return cnt;
		}
 
		 public int read(sbyte b[], int off, int len) {
    		int cnt = inputStream.read(b, off, len);
 			count += cnt;
			byteCount += cnt;
			return cnt;
		 }
  
		 public int available() {
    		 return inputStream.available();
		 }
 
		 public void close() {
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
#endif
}