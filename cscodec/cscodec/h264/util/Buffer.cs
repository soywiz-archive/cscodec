using System;

namespace cscodec.h243.util
{
	public class Buffer
	{
		private int cap = 0;
		private int limit = 0;
		private int pos = 0;
		private int mark = -1;

		public Buffer(int capacity, int limit_, int position, int mark_)
		{
			if (capacity < 0) throw new ArgumentException();
			cap = capacity;
			limit = limit_;
			pos = position;
			if (mark_ > 0)
			{
				if (mark_ > pos)
					throw new ArgumentException();
			} // if
			mark = mark_;
		}

		public int capacity() { return cap; }
		public Buffer clear()
		{
			limit = cap;
			pos = 0;
			mark = -1;
			return this;
		}
		public Buffer flip()
		{
			limit = pos;
			pos = 0;
			mark = -1;
			return this;
		}
		public bool hasRemaining() { return limit > pos; }
		public int limit() { return limit; }
		public Buffer limit(int newLimit)
		{
			if ((newLimit < 0) || (newLimit > cap))
				throw new ArgumentException();
			if (newLimit <= mark) mark = -1;
			if (pos > newLimit) pos = newLimit - 1;
			limit = newLimit;
			return this;
		}
		public Buffer mark() { mark = pos; return this; }
		public int position() { return pos; }
		public Buffer position(int newPosition)
		{
			if ((newPosition < 0) || (newPosition > limit))
				throw new ArgumentException();
			if (newPosition <= mark) mark = -1;
			pos = newPosition;
			return this;
		}
		public int remaining() { return limit - pos; }
		public Buffer reset()
		{
			if (mark == -1)
				throw new ArgumentException();
			pos = mark;
			return this;
		}
		public Buffer rewind()
		{
			pos = 0;
			mark = -1;
			return this;
		}
	}
}