using System;
using cscodec.av;
using System.Drawing;
using System.Drawing.Imaging;

namespace cscodec
{
	unsafe public static class FrameUtils
	{
        public static Bitmap ToImageWOEdges(this AVFrame f)
		{
		    Bitmap bi = new Bitmap(f.imageWidthWOEdge, f.imageHeightWOEdge, PixelFormat.Format32bppArgb);
		    int[] rgb = new int[f.imageWidthWOEdge * f.imageHeightWOEdge];

		    YUV2RGB_WOEdge(f, rgb);

		    var BitmapData = bi.LockBits(new Rectangle(0, 0, bi.Width, bi.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
		    var Ptr = (int*)BitmapData.Scan0.ToPointer();
		    for (int j = 0; j < f.imageHeightWOEdge; j++)
		    {
		        int off = j * f.imageWidthWOEdge;
		        for (int i = 0; i < f.imageWidthWOEdge; i++)
		        {
		            Ptr[off + i] = rgb[off + i];
		        }
		    }
		    bi.UnlockBits(BitmapData);

		    return bi;
		}

        public static Bitmap ToImageWOEdges(this AVFrame f, int Width, int Height)
        {
            var Out = new Bitmap(Math.Min(Width, f.imageWidthWOEdge), Math.Min(Height, f.imageHeightWOEdge));
            Graphics.FromImage(Out).DrawImage(f.ToImageWOEdges(), Point.Empty);
            return Out;
        }

        public static Bitmap imageFromFrame(AVFrame f)
		{
			Bitmap bi = new Bitmap(f.imageWidth, f.imageHeight, PixelFormat.Format32bppArgb);
			int[] rgb = new int[f.imageWidth * f.imageHeight];

			YUV2RGB(f, rgb);

			var BitmapData = bi.LockBits(new Rectangle(0, 0, bi.Width, bi.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
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

	    private static void YUV2RGB(AVFrame f, int[] rgb)
		{
			var luma = f.data_base[0];
			var cb = f.data_base[1];
			var cr = f.data_base[2];
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

					byte red = (byte)MathUtils.Clamp((298 * c + 409 * e + 128) >> 8, 0, 255);
					byte green = (byte)MathUtils.Clamp((298 * c - 100 * d - 208 * e + 128) >> 8, 0, 255);
					byte blue = (byte)MathUtils.Clamp((298 * c + 516 * d + 128) >> 8, 0, 255);
					byte alpha = 255;

					rgb[lineOffLuma + x] = (alpha << 24) | (red << 16) | (green << 8) | (blue << 0);
				}
			}
		}

        private static void YUV2RGB_WOEdge(AVFrame f, int[] rgb)
        {
            var luma = f.data_base[0];
            var cb = f.data_base[1];
            var cr = f.data_base[2];
            int stride = f.linesize[0];
            int strideCb = f.linesize[1];
            int strideCr = f.linesize[2];


            for (int y = 0; y < f.imageHeightWOEdge; y++)
            {
                int lineOffLuma = y * stride + f.data_offset[0];
                int lineOffCb = (y >> 1) * strideCb + f.data_offset[1];
                int lineOffCr = (y >> 1) * strideCr + f.data_offset[2];
                int rgbOff = y * f.imageWidthWOEdge;

                for (int x = 0; x < f.imageWidthWOEdge; x++)
                {
                    int c = luma[lineOffLuma + x] - 16;
                    int d = cb[lineOffCb + (x >> 1)] - 128;
                    int e = cr[lineOffCr + (x >> 1)] - 128;

                    byte red = (byte)MathUtils.Clamp((298 * c + 409 * e + 128) >> 8, 0, 255);
                    byte green = (byte)MathUtils.Clamp((298 * c - 100 * d - 208 * e + 128) >> 8, 0, 255);
                    byte blue = (byte)MathUtils.Clamp((298 * c + 516 * d + 128) >> 8, 0, 255);
                    byte alpha = 255;

                    rgb[rgbOff + x] = (alpha << 24) | (red << 16) | (green << 8) | (blue << 0);
                }
            }
        }
	}
}