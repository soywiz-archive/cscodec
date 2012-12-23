using cscodec.h264.decoder;
using cscodec.h264.util;
using cscodec.h264.player;
using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
namespace cscodec.h264.player
{

public class H264Player {
	
	public const int INBUF_SIZE = 65535;
	Form frame;
	//private PlayerFrame displayPanel;
	private int[] buffer = null;

	/**
	 * @param args
	 */
	public static void Main(string[] args) {
		args = new string[] { @"C:\projects\h264j\H264Player\sample_clips\admiral.264" };

		// TODO Auto-generated method stub
		if (args.Length < 1)
		{
			Console.WriteLine("Usage: java com.twilight.h264.decoder.H264Player <.h264 raw file>\n");
			return;
		}
		else
		{
			var H264Player = new H264Player();

			H264Player.frame = new Form()
			{
				Text = "Player",
				FormBorderStyle = FormBorderStyle.FixedDialog,
				MinimizeBox = false,
				StartPosition = FormStartPosition.CenterScreen,
			};
			//displayPanel = new PlayerFrame();

			//frame.getContentPane().add(displayPanel, BorderLayout.CENTER);

			// Finish setting up the frame, and show it.
			H264Player.frame.FormClosing += (s, e) =>
			{
				Environment.Exit(0);
			};

			H264Player.frame.HandleCreated += (s, e) =>
			{
				new Thread(() =>
				{
					H264Player.run(args[0]);
				}).Start();
			};

			Application.Run(H264Player.frame);

		} // if
	}

	public H264Player() {
		
	}

	public void run(string fileName)
	{
		Console.WriteLine("Playing "+ fileName);
		playFile(fileName);		
	}

	public static void CenterForm(Form theForm)
	{
		theForm.Location = new Point(
			Screen.PrimaryScreen.WorkingArea.Width / 2 - theForm.Width / 2,
			Screen.PrimaryScreen.WorkingArea.Height / 2 - theForm.Height / 2);
	}

	public bool playFile(string filename) {
	    H264Decoder codec;
	    MpegEncContext c= null;
		int frame, len;
		int[] got_picture = new int[1];
		AVFrame picture;
		//uint8_t inbuf[INBUF_SIZE + H264Context.FF_INPUT_BUFFER_PADDING_SIZE];
		sbyte[] inbuf = new sbyte[INBUF_SIZE + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE];
		int[] inbuf_int = new int[INBUF_SIZE + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE];
		//char buf[1024];
		sbyte[] buf = new sbyte[1024];
		AVPacket avpkt = new AVPacket();

		using (Stream fin = File.OpenRead(filename))
		{
			avpkt.av_init_packet();

			/* set end of buffer to 0 (this ensures that no overreading happens for damaged mpeg streams) */
			Arrays.fill(inbuf, INBUF_SIZE, MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE + INBUF_SIZE, (sbyte)0);

			Console.WriteLine("Video decoding\n");

			/* find the mpeg1 video decoder */
			codec = new H264Decoder();
			if (codec == null)
			{
				Console.WriteLine("codec not found\n");
				Environment.Exit(1);
			} // if

			c = MpegEncContext.avcodec_alloc_context();
			picture = AVFrame.avcodec_alloc_frame();

			if ((codec.capabilities & H264Decoder.CODEC_CAP_TRUNCATED) != 0)
				c.flags |= MpegEncContext.CODEC_FLAG_TRUNCATED; /* we do not send complete frames */

			/* For some codecs, such as msmpeg4 and mpeg4, width and height
			   MUST be initialized there because this information is not
			   available in the bitstream. */

			/* open it */
			if (c.avcodec_open(codec) < 0)
			{
				Console.WriteLine("could not open codec\n");
				Environment.Exit(1);
			}

			/* the codec gives us the frame size, in samples */

			frame = 0;
			int dataPointer;

			// avpkt must contain exactly 1 NAL Unit in order for decoder to decode correctly.
			// thus we must read until we get next NAL header before sending it to decoder.
			// Find 1st NAL
			int[] cacheRead = new int[3];
			cacheRead[0] = fin.ReadByte();
			cacheRead[1] = fin.ReadByte();
			cacheRead[2] = fin.ReadByte();

			while (!(
					cacheRead[0] == 0x00 &&
					cacheRead[1] == 0x00 &&
					cacheRead[2] == 0x01
					))
			{
				cacheRead[0] = cacheRead[1];
				cacheRead[1] = cacheRead[2];
				cacheRead[2] = fin.ReadByte();
			} // while

			bool hasMoreNAL = true;

			// 4 first bytes always indicate NAL header
			inbuf_int[0] = inbuf_int[1] = inbuf_int[2] = 0x00;
			inbuf_int[3] = 0x01;

			while (hasMoreNAL)
			{
				dataPointer = 4;
				// Find next NAL
				cacheRead[0] = fin.ReadByte();
				if (cacheRead[0] == -1) hasMoreNAL = false;
				cacheRead[1] = fin.ReadByte();
				if (cacheRead[1] == -1) hasMoreNAL = false;
				cacheRead[2] = fin.ReadByte();
				if (cacheRead[2] == -1) hasMoreNAL = false;
				while (!(
						cacheRead[0] == 0x00 &&
						cacheRead[1] == 0x00 &&
						cacheRead[2] == 0x01
						) && hasMoreNAL)
				{
					inbuf_int[dataPointer++] = cacheRead[0];
					cacheRead[0] = cacheRead[1];
					cacheRead[1] = cacheRead[2];
					cacheRead[2] = fin.ReadByte();
					if (cacheRead[2] == -1) hasMoreNAL = false;
				} // while

				avpkt.size = dataPointer;

				avpkt.data_base = inbuf_int;
				avpkt.data_offset = 0;

				while (avpkt.size > 0)
				{
					len = c.avcodec_decode_video2(picture, got_picture, avpkt);
					if (len < 0)
					{
						Console.WriteLine("Error while decoding frame " + frame);
						// Discard current packet and proceed to next packet
						break;
					} // if
					if (got_picture[0] != 0)
					{
						picture = c.priv_data.displayPicture;

						int bufferSize = picture.imageWidth * picture.imageHeight;
						if (buffer == null || bufferSize != buffer.Length)
						{
							buffer = new int[bufferSize];
						}
						//FrameUtils.YUV2RGB(picture, buffer);
						var Image = FrameUtils.imageFromFrame(picture);
						if (this.frame.ClientSize.Width < picture.imageWidth || this.frame.ClientSize.Height < picture.imageHeight)
						{
							this.frame.Invoke((Action)(() =>
							{
								this.frame.ClientSize = new Size(picture.imageWidth, picture.imageHeight);
								CenterForm(this.frame);
							}));
						}
						this.frame.CreateGraphics().DrawImage(Image, Point.Empty);
						//displayPanel.lastFrame = displayPanel.createImage(new Bitmap(picture.imageWidth, picture.imageHeight, buffer, 0, picture.imageWidth));
						//displayPanel.invalidate();
						//displayPanel.updateUI();			            	
					}
					avpkt.size -= len;
					avpkt.data_offset += len;
				}

			} // while
		}
	
	    c.avcodec_close();
	    c = null;
	    picture = null;
	    Console.WriteLine("Stop playing video.");
	    
	    return true;
	}
	

}
}