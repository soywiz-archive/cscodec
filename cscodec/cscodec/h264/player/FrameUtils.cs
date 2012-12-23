using cscodec.h264.decoder;
using System.Drawing;
using System.Drawing.Imaging;
namespace cscodec.h264.player
{
public class FrameUtils {
	public static Bitmap imageFromFrame(AVFrame f) {

		Bitmap bi = new Bitmap(f.imageWidth, f.imageHeight, PixelFormat.Format32bppArgb);
		int[] rgb = new int[f.imageWidth * f.imageHeight];

		YUV2RGB(f, rgb);

		for (int j = 0; j < f.imageHeight; j++) {
			int off = j * f.imageWidth;
			for (int i = 0; i < f.imageWidth; i++) {
				bi.SetPixel(i, j, Color.FromArgb(rgb[off + i]));
			}
		}

		return bi;
	}

	public static void YUV2RGB(AVFrame f, int[] rgb) {
		int[] luma = f.data_base[0];
		int[] cb = f.data_base[1];
		int[] cr = f.data_base[2];
		int stride = f.linesize[0];
		int strideChroma = f.linesize[1];

		for (int y = 0; y < f.imageHeight; y++) {
			int lineOffLuma = y * stride;
			int lineOffChroma = (y >> 1) * strideChroma;

			for (int x = 0; x < f.imageWidth; x++) {
				int c = luma[lineOffLuma + x] - 16;
				int d = cb[lineOffChroma + (x >> 1)] - 128;
				int e = cr[lineOffChroma + (x >> 1)] - 128;

				int red = (298 * c + 409 * e + 128) >> 8;
				red = red < 0 ? 0 : (red > 255 ? 255 : red);
				int green = (298 * c - 100 * d - 208 * e + 128) >> 8;
				green = green < 0 ? 0 : (green > 255 ? 255 : green);
				int blue = (298 * c + 516 * d + 128) >> 8;
				blue = blue < 0 ? 0 : (blue > 255 ? 255 : blue);
				int alpha = 255;

				rgb[lineOffLuma + x] = (alpha << 24) | ((red & 0x0ff) << 16)
						| ((green & 0x0ff) << 8) | (blue & 0x0ff);
			}
		}
	}
}
}