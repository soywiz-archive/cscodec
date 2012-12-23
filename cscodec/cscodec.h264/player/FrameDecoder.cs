using cscodec.h264.decoder;
using cscodec.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace cscodec.h264.player
{
	public class FrameDecoder : IDisposable
	{
		const int INBUF_SIZE = 65535;
		H264Decoder codec;
		MpegEncContext c = null;
		int frame, len;
		int[] got_picture = new int[1];
		AVFrame picture;
		//uint8_t inbuf[INBUF_SIZE + H264Context.FF_INPUT_BUFFER_PADDING_SIZE];
		sbyte[] inbuf = new sbyte[INBUF_SIZE + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE];
		int[] inbuf_int = new int[INBUF_SIZE + MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE];
		//char buf[1024];
		sbyte[] buf = new sbyte[1024];
		AVPacket avpkt = new AVPacket();
		Stream fin;
		bool hasMoreNAL;
		int dataPointer;
		int[] cacheRead = new int[3];
		private int[] buffer = null;

		public FrameDecoder(Stream Stream)
		{
			this.fin = Stream;
			Init();
		}

		private void Close()
		{
			c.avcodec_close();
			c = null;
			picture = null;
		}

		private void Init()
		{
			avpkt.av_init_packet();

			// Set end of buffer to 0 (this ensures that no overreading happens for damaged mpeg streams)
			Arrays.Fill(inbuf, INBUF_SIZE, MpegEncContext.FF_INPUT_BUFFER_PADDING_SIZE + INBUF_SIZE, (sbyte)0);

			Console.WriteLine("Video decoding\n");

			// Find the mpeg1 video decoder
			codec = new H264Decoder();
			if (codec == null)
			{
				throw (new Exception("codec not found"));
			}

			c = MpegEncContext.avcodec_alloc_context();
			picture = AVFrame.avcodec_alloc_frame();

			// We do not send complete frames
			if ((codec.capabilities & H264Decoder.CODEC_CAP_TRUNCATED) != 0)
			{
				c.flags |= MpegEncContext.CODEC_FLAG_TRUNCATED;
			}

			// For some codecs, such as msmpeg4 and mpeg4, width and height
			// MUST be initialized there because this information is not
			// available in the bitstream.

			// Open it
			if (c.avcodec_open(codec) < 0)
			{
				throw (new Exception("could not open codec"));
			}

			// The codec gives us the frame size, in samples

			frame = 0;

			// avpkt must contain exactly 1 NAL Unit in order for decoder to decode correctly.
			// thus we must read until we get next NAL header before sending it to decoder.
			// Find 1st NAL
			cacheRead[0] = fin.ReadByte();
			cacheRead[1] = fin.ReadByte();
			cacheRead[2] = fin.ReadByte();

			while (!(cacheRead[0] == 0x00 && cacheRead[1] == 0x00 && cacheRead[2] == 0x01))
			{
				cacheRead[0] = cacheRead[1];
				cacheRead[1] = cacheRead[2];
				cacheRead[2] = fin.ReadByte();
			} // while

			// 4 first bytes always indicate NAL header
			inbuf_int[0] = inbuf_int[1] = inbuf_int[2] = 0x00;
			inbuf_int[3] = 0x01;

			hasMoreNAL = true;
		}

		public bool HasMorePackets
		{
			get
			{
				return hasMoreNAL;
			}
		}

		private void ReadPacket()
		{
			if (hasMoreNAL)
			{
				dataPointer = 4;
				// Find next NAL
				if ((cacheRead[0] = fin.ReadByte()) == -1) hasMoreNAL = false;
				if ((cacheRead[1] = fin.ReadByte()) == -1) hasMoreNAL = false;
				if ((cacheRead[2] = fin.ReadByte()) == -1) hasMoreNAL = false;
				while (!(cacheRead[0] == 0x00 && cacheRead[1] == 0x00 && cacheRead[2] == 0x01) && hasMoreNAL)
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
			}
		}

		public AVFrame DecodeFrame()
		{
			while (hasMoreNAL)
			{
				//Console.WriteLine(avpkt.size);
				ReadPacket();

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
						return picture;
					}
					avpkt.size -= len;
					avpkt.data_offset += len;
				}

			} // while
			throw (new EndOfStreamException());
		}

		void IDisposable.Dispose()
		{
			Close();
		}
	}
}
