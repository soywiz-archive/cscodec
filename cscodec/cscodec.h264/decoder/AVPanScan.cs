using cscodec.h264.util;
namespace cscodec.h264.decoder
{
	public class AVPanScan {
		/**
		 * id
		 * - encoding: Set by user.
		 * - decoding: Set by libavcodec.
		 */
		public int id;

		/**
		 * width and height in 1/16 pel
		 * - encoding: Set by user.
		 * - decoding: Set by libavcodec.
		 */
		public int width;
		public int height;

		/**
		 * position of the top left corner in 1/16 pel for up to 3 fields/frames
		 * - encoding: Set by user.
		 * - decoding: Set by libavcodec.
		 */
		public int[][] position = Arrays.Create2D<int>(3, 2);
	}
}