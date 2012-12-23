using cscodec.h264.decoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace cscodec.h264.player
{
	unsafe public class FrameUtils
	{
		public static Bitmap imageFromFrame(AVFrame f)
		{
			Bitmap bi = new Bitmap(f.imageWidth, f.imageHeight, PixelFormat.Format32bppArgb);
			int[] rgb = new int[f.imageWidth * f.imageHeight];

			YUV2RGB(f, rgb);

			var BitmapData = bi.LockBits(new System.Drawing.Rectangle(0, 0, bi.Width, bi.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
			var Ptr = (int*)BitmapData.Scan0.ToPointer();
			for (int j = 0; j < f.imageHeight; j++)
			{
				int off = j * f.imageWidth;
				for (int i = 0; i < f.imageWidth; i++)
				{
					Ptr[j * f.imageWidth + i] = rgb[off + i];
				}
			}
			bi.UnlockBits(BitmapData);

			return bi;
		}

		static private int Clamp(int Value, int Min, int Max)
		{
			if (Value < Min) return Min;
			if (Value > Max) return Max;
			return Value;
		}

		public static void YUV2RGB(AVFrame f, int[] rgb)
		{
			int[] luma = f.data_base[0];
			int[] cb = f.data_base[1];
			int[] cr = f.data_base[2];
			int stride = f.linesize[0];
			int strideChroma = f.linesize[1];

			for (int y = 0; y < f.imageHeight; y++)
			{
				int lineOffLuma = y * stride;
				int lineOffChroma = (y >> 1) * strideChroma;

				for (int x = 0; x < f.imageWidth; x++)
				{
					int c = luma[lineOffLuma + x] - 16;
					int d = cb[lineOffChroma + (x >> 1)] - 128;
					int e = cr[lineOffChroma + (x >> 1)] - 128;

					int red = Clamp((298 * c + 409 * e + 128) >> 8, 0, 255);
					int green = Clamp((298 * c - 100 * d - 208 * e + 128) >> 8, 0, 255);
					int blue = Clamp((298 * c + 516 * d + 128) >> 8, 0, 255);
					int alpha = 255;

					rgb[lineOffLuma + x] = (alpha << 24) | ((red & 0x0ff) << 16) | ((green & 0x0ff) << 8) | (blue & 0x0ff);
				}
			}
		}
	}
}