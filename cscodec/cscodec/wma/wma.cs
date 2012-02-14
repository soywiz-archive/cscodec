//#define TRACE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cscodec.wma
{
	unsafe public partial class wma
	{
		const int AV_LOG_ERROR = 1;
		const int AV_LOG_WARNING = 2;

		const int AVERROR_INVALIDDATA = -6;

		const int AV_SAMPLE_FMT_S16 = 2;

		const int CODEC_ID_WMAV1 = 10001;
		const int CODEC_ID_WMAV2 = 10002;

		/* size of blocks */
		const int BLOCK_MIN_BITS = 7;
		const int BLOCK_MAX_BITS = 11;
		const int BLOCK_MAX_SIZE = (1 << BLOCK_MAX_BITS);

		const int BLOCK_NB_SIZES = (BLOCK_MAX_BITS - BLOCK_MIN_BITS + 1);

		/* XXX: find exact max size */
		const int HIGH_BAND_MAX_SIZE = 16;

		const int NB_LSP_COEFS = 10;

		/* XXX: is it a suitable value ? */
		const int MAX_CODED_SUPERFRAME_SIZE = 16384;

		const int MAX_CHANNELS = 2;

		const int NOISE_TAB_SIZE = 8192;

		const int LSP_POW_BITS = 7;

		//FIXME should be in wmadec
		const int VLCBITS = 9;
		const int VLCMAX = ((22+VLCBITS-1)/VLCBITS);

		//public enum WMACoef : float { }
		//typedef float WMACoef;          ///< type for decoded coefficients, short would be enough for wma 1/2

		class CoefVLCTable {
			public int n { get { return huffbits.Length; } }                      ///< total number of codes
			public int max_level { get { return levels.Length; } }
			public uint[] huffcodes;  ///< VLC bit values
			public byte[] huffbits;    ///< VLC bit size
			public ushort[] levels;     ///< table to build run/level tables
		}

		struct WMACoef 
		{
			public float Value;
		}

		class AVCodec
		{
			public int id;
		}

		class AVCodecContext {
			public AVCodec codec;
			public Object priv_data;
			public int sample_rate;
			public int channels;
			public int bit_rate;
			public int block_align;
			public unsafe byte* extradata;
			public int extradata_size;
			public int sample_fmt;
			public AVFrame coded_frame;

			public void av_log(int type, String Format, params Object[] Params)
			{
			}

			internal void tprintf(string p)
			{
				throw new NotImplementedException();
			}

			internal unsafe int get_buffer(AVFrame aVFrame)
			{
				throw new NotImplementedException();
			}

			internal unsafe void ff_wma_run_level_decode(GetBitContext getBitContext, VLC vLC, float p, ushort p_2, int p_3, WMACoef* ptr, int p_4, int p_5, int p_6, int p_7, int coef_nb_bits)
			{
				throw new NotImplementedException();
			}

			internal void tprintf(params object[] Params)
			{
				throw new NotImplementedException();
			}
		}

		class AVFrame
		{
			public int nb_samples;
			public void*[] data;

			internal void avcodec_get_frame_defaults()
			{
				throw new NotImplementedException();
			}
		}

		class GetBitContext {
			internal int get_bits1()
			{
				throw new NotImplementedException();
			}

			internal uint get_bits_long(int n_bits)
			{
				throw new NotImplementedException();
			}

			internal int get_bits(int p)
			{
				throw new NotImplementedException();
			}

			internal void align_get_bits()
			{
				throw new NotImplementedException();
			}

			internal int get_vlc2(object p, int HGAINVLCBITS, int HGAINMAX)
			{
				throw new NotImplementedException();
			}

			internal int ff_wma_get_large_val()
			{
				throw new NotImplementedException();
			}

			internal unsafe void init_get_bits(byte* buf, int p)
			{
				throw new NotImplementedException();
			}

			internal void skip_bits(int p)
			{
				throw new NotImplementedException();
			}

			internal int get_bits_count()
			{
				throw new NotImplementedException();
			}

			internal void init_get_bits(byte[] p, int p_2)
			{
				throw new NotImplementedException();
			}
		}

		class PutBitContext {
		}

		struct FFTSample{
		}

		class FFTContext{
			internal void imdct_calc(FFTSample[] fFTSample, float[] p)
			{
				throw new NotImplementedException();
			}

			internal void ff_mdct_init(int p, int p_2, double p_3)
			{
				throw new NotImplementedException();
			}

			internal void ff_mdct_end()
			{
				throw new NotImplementedException();
			}
		}

		class DSPContext {
			internal void butterflies_float(float[] p, float[] p_2, int p_3)
			{
				throw new NotImplementedException();
			}

			internal void dsputil_init(AVCodecContext avctx)
			{
				throw new NotImplementedException();
			}

			internal unsafe void vector_fmul_add(float* Out, float* In, float p, float* Out_2, int block_len)
			{
				throw new NotImplementedException();
			}

			internal unsafe void vector_fmul_reverse(float* Out, float* In, float p, int block_len)
			{
				throw new NotImplementedException();
			}
		}

		class FmtConvertContext {
			internal unsafe void float_to_int16_interleave(short* samples, float[] output, int n, int incr)
			{
				throw new NotImplementedException();
			}

			internal void ff_fmt_convert_init(AVCodecContext avctx)
			{
				throw new NotImplementedException();
			}
		}

		class AVPacket
		{
			public byte* data;
			public int size;
		}

		/// <summary>
		/// @TODO!
		/// </summary>
		static public float[] ff_sine_windows
		{
			get
			{
				throw(new NotImplementedException());
			}
		}

		static readonly CoefVLCTable[] coef_vlcs = new CoefVLCTable[6]
		{
			new CoefVLCTable() { huffcodes = coef0_huffcodes, huffbits = coef0_huffbits, levels = levels0 },
			new CoefVLCTable() { huffcodes = coef1_huffcodes, huffbits = coef1_huffbits, levels = levels1 },
			new CoefVLCTable() { huffcodes = coef2_huffcodes, huffbits = coef2_huffbits, levels = levels2 },
			new CoefVLCTable() { huffcodes = coef3_huffcodes, huffbits = coef3_huffbits, levels = levels3 },
			new CoefVLCTable() { huffcodes = coef4_huffcodes, huffbits = coef4_huffbits, levels = levels4 },
			new CoefVLCTable() { huffcodes = coef5_huffcodes, huffbits = coef5_huffbits, levels = levels5 },
		};


		class WMACodecContext
		{
			public AVCodecContext avctx;
			public AVFrame frame;
			public GetBitContext gb;
			public PutBitContext pb;
			public int sample_rate;
			public int nb_channels;
			public int bit_rate;
			public int version;                            ///< 1 = 0x160 (WMAV1), 2 = 0x161 (WMAV2)
			public int block_align;
			public bool use_bit_reservoir;
			public bool use_variable_block_len;
			public bool use_exp_vlc;                        ///< exponent coding: 0 = lsp, 1 = vlc + delta
			public bool use_noise_coding;                   ///< true if perceptual noise is added
			public int byte_offset_bits;
			public VLC exp_vlc;
			public int[] exponent_sizes = new int[BLOCK_NB_SIZES];
			public ushort[][] exponent_bands = new ushort[BLOCK_NB_SIZES][/*25*/];
			public int[] high_band_start = new int[BLOCK_NB_SIZES];    ///< index of first coef in high band
			public int coefs_start;                        ///< first coded coef
			public int[] coefs_end = new int[BLOCK_NB_SIZES];          ///< max number of coded coefficients
			public int[] exponent_high_sizes = new int[BLOCK_NB_SIZES];
			public int[,] exponent_high_bands = new int[BLOCK_NB_SIZES, HIGH_BAND_MAX_SIZE];
			public VLC hgain_vlc;

			/* coded values in high bands */
			public int[,] high_band_coded = new int[MAX_CHANNELS, HIGH_BAND_MAX_SIZE];
			public int[,] high_band_values = new int[MAX_CHANNELS, HIGH_BAND_MAX_SIZE];

			/* there are two possible tables for spectral coefficients */
		//FIXME the following 3 tables should be shared between decoders
			public VLC[] coef_vlc = new VLC[2];
			public ushort[] run_table = new ushort[2];
			public float[] level_table = new float[2];
			public ushort[] int_table = new ushort[2];
			public CoefVLCTable[] coef_vlcs = new CoefVLCTable[2];
			/* frame info */
			public int frame_len;                          ///< frame length in samples
			public int frame_len_bits;                     ///< frame_len = 1 << frame_len_bits
			public int nb_block_sizes;                     ///< number of block sizes
			/* block info */
			public bool reset_block_lengths;
			public int block_len_bits;                     ///< log2 of current block length
			public int next_block_len_bits;                ///< log2 of next block length
			public int prev_block_len_bits;                ///< log2 of prev block length
			public int block_len;                          ///< block length in samples
			public int block_num;                          ///< block number in current frame
			public int block_pos;                          ///< current position in frame
			public bool ms_stereo;                      ///< true if mid/side stereo mode
			public bool[] channel_coded = new bool[MAX_CHANNELS];    ///< true if channel is coded
			public int[] exponents_bsize = new int[MAX_CHANNELS];      ///< log2 ratio frame/exp. length

			//[Aligned=32]
			//public float[,] exponents = new float[MAX_CHANNELS, BLOCK_MAX_SIZE];
			public float[][] exponents = new float[MAX_CHANNELS][/*BLOCK_MAX_SIZE*/];

			public float[] max_exponent = new float[MAX_CHANNELS];
			//public WMACoef[,] coefs1 = new WMACoef[MAX_CHANNELS, BLOCK_MAX_SIZE];
			public WMACoef[][] coefs1 = new WMACoef[MAX_CHANNELS][/*BLOCK_MAX_SIZE*/];

			//[Aligned=32]
			public float[][] coefs = new float[MAX_CHANNELS][/*BLOCK_MAX_SIZE*/];
			public FFTSample[] output = new FFTSample[BLOCK_MAX_SIZE * 2];
			public FFTContext[] mdct_ctx = new FFTContext[BLOCK_NB_SIZES];
			public float[] windows = new float[BLOCK_NB_SIZES];
			
			/* output buffer for one frame and the last for IMDCT windowing */
			//[Aligned=32]
			public float[][] frame_out = new float[MAX_CHANNELS][/*BLOCK_MAX_SIZE * 2*/];

			/* last frame info */
			public byte[] last_superframe = new byte[MAX_CODED_SUPERFRAME_SIZE + 4]; /* padding added */
			public int last_bitoffset;
			public int last_superframe_len;
			public float[] noise_table = new float[NOISE_TAB_SIZE];
			public int noise_index;
			public float noise_mult; /* XXX: suppress that and integrate it in the noise array */
			/* lsp_to_curve tables */
			public float[] lsp_cos_table = new float[BLOCK_MAX_SIZE];
			public float[] lsp_pow_e_table = new float[256];
			public float[] lsp_pow_m_table1 = new float[(1 << LSP_POW_BITS)];
			public float[] lsp_pow_m_table2 = new float[(1 << LSP_POW_BITS)];
			public DSPContext dsp;
			public FmtConvertContext fmt_conv;

		#if TRACE
			public int frame_count;
		#endif

			internal unsafe void wma_lsp_to_curve(float[] p, float* p_2, int p_3, float[] lsp_coefs)
			{
				throw new NotImplementedException();
			}

			internal void wma_lsp_to_curve(float[] p, float p_2, int p_3, float[] lsp_coefs)
			{
				throw new NotImplementedException();
			}

			internal void wma_lsp_to_curve(float[] p, ref float p_2, int p_3, float[] lsp_coefs)
			{
				throw new NotImplementedException();
			}
		}


		/* XXX: use same run/length optimization as mpeg decoders */
		//FIXME maybe split decode / encode or pass flag
		static void init_coef_vlc(VLC vlc, out ushort[] prun_table, out float[] plevel_table, out ushort[] pint_table, CoefVLCTable vlc_table)
		{
			int n = vlc_table.n;
			byte[] table_bits   = vlc_table.huffbits;
			uint[] table_codes  = vlc_table.huffcodes;
			ushort[] levels_table = vlc_table.levels;
			//ushort* run_table, level_table, int_table;
			ushort[] run_table, level_table, int_table;
			float[] flevel_table;
			int i, l, j, k, level;

			vlc.init_vlc(VLCBITS, n, table_bits, 1, 1, table_codes, 4, 4, 0);

			run_table   = new ushort[n];
			level_table = new ushort[n];
			flevel_table= new float[n];
			int_table   = new ushort[n];
			i = 2;
			level = 1;
			k = 0;
			while (i < n) {
				int_table[k] = (ushort)i;
				l = levels_table[k++];
				for (j = 0; j < l; j++) {
					run_table[i]   = (ushort)j;
					level_table[i] = (ushort)level;
					flevel_table[i]= level;
					i++;
				}
				level++;
			}
			prun_table   = run_table;
			plevel_table = flevel_table;
			pint_table   = int_table;
			level_table = null;
		}

		/**
		 *@brief Get the samples per frame for this stream.
		 *@param sample_rate output sample_rate
		 *@param version wma version
		 *@param decode_flags codec compression features
		 *@return log2 of the number of output samples per frame
		 */
		static int /*av_cold*/ ff_wma_get_frame_len_bits(int sample_rate, int version, uint decode_flags)
		{

			int frame_len_bits;

			if (sample_rate <= 16000) {
				frame_len_bits = 9;
			} else if (sample_rate <= 22050 ||
					 (sample_rate <= 32000 && version == 1)) {
				frame_len_bits = 10;
			} else if (sample_rate <= 48000 || version < 3) {
				frame_len_bits = 11;
			} else if (sample_rate <= 96000) {
				frame_len_bits = 12;
			} else {
				frame_len_bits = 13;
			}

			if (version == 3) {
				uint tmp = decode_flags & 0x6;
				if (tmp == 0x2) {
					++frame_len_bits;
				} else if (tmp == 0x4) {
					--frame_len_bits;
				} else if (tmp == 0x6) {
					frame_len_bits -= 2;
				}
			}

			return frame_len_bits;
		}

		static int ff_wma_init(AVCodecContext avctx, int flags2)
		{
			WMACodecContext s = (WMACodecContext)avctx.priv_data;
			int i;
			float bps1, high_freq;
			float bps;
			int sample_rate1;
			int coef_vlc_table;

			if (   avctx.sample_rate <= 0 || avctx.sample_rate > 50000
				|| avctx.channels    <= 0 || avctx.channels    > 8
				|| avctx.bit_rate    <= 0)
				return -1;

			s.sample_rate = avctx.sample_rate;
			s.nb_channels = avctx.channels;
			s.bit_rate    = avctx.bit_rate;
			s.block_align = avctx.block_align;

			s.dsp.dsputil_init(avctx);
			s.fmt_conv.ff_fmt_convert_init(avctx);

			if (avctx.codec.id == CODEC_ID_WMAV1) {
				s.version = 1;
			} else {
				s.version = 2;
			}

			/* compute MDCT block size */
			s.frame_len_bits = ff_wma_get_frame_len_bits(s.sample_rate, s.version, 0);
			s.next_block_len_bits = s.frame_len_bits;
			s.prev_block_len_bits = s.frame_len_bits;
			s.block_len_bits      = s.frame_len_bits;

			s.frame_len = 1 << s.frame_len_bits;
			if (s.use_variable_block_len) {
				int nb_max, nb;
				nb = ((flags2 >> 3) & 3) + 1;
				if ((s.bit_rate / s.nb_channels) >= 32000)
					nb += 2;
				nb_max = s.frame_len_bits - BLOCK_MIN_BITS;
				if (nb > nb_max)
					nb = nb_max;
				s.nb_block_sizes = nb + 1;
			} else {
				s.nb_block_sizes = 1;
			}

			/* init rate dependent parameters */
			s.use_noise_coding = true;
			high_freq = s.sample_rate * 0.5f;

			/* if version 2, then the rates are normalized */
			sample_rate1 = s.sample_rate;
			if (s.version == 2) {
				if (sample_rate1 >= 44100) {
					sample_rate1 = 44100;
				} else if (sample_rate1 >= 22050) {
					sample_rate1 = 22050;
				} else if (sample_rate1 >= 16000) {
					sample_rate1 = 16000;
				} else if (sample_rate1 >= 11025) {
					sample_rate1 = 11025;
				} else if (sample_rate1 >= 8000) {
					sample_rate1 = 8000;
				}
			}

			bps = (float)s.bit_rate / (float)(s.nb_channels * s.sample_rate);
			s.byte_offset_bits = av_log2((int)(bps * s.frame_len / 8.0 + 0.5)) + 2;

			/* compute high frequency value and choose if noise coding should
			   be activated */
			bps1 = bps;
			if (s.nb_channels == 2)
				bps1 = bps * 1.6f;
			if (sample_rate1 == 44100) {
				if (bps1 >= 0.61) {
					s.use_noise_coding = false;
				} else {
					high_freq = high_freq * 0.4f;
				}
			} else if (sample_rate1 == 22050) {
				if (bps1 >= 1.16) {
					s.use_noise_coding = false;
				} else if (bps1 >= 0.72) {
					high_freq = high_freq * 0.7f;
				} else {
					high_freq = high_freq * 0.6f;
				}
			} else if (sample_rate1 == 16000) {
				if (bps > 0.5) {
					high_freq = high_freq * 0.5f;
				} else {
					high_freq = high_freq * 0.3f;
				}
			} else if (sample_rate1 == 11025) {
				high_freq = high_freq * 0.7f;
			} else if (sample_rate1 == 8000) {
				if (bps <= 0.625) {
					high_freq = high_freq * 0.5f;
				} else if (bps > 0.75) {
					s.use_noise_coding = false;
				} else {
					high_freq = high_freq * 0.65f;
				}
			} else {
				if (bps >= 0.8) {
					high_freq = high_freq * 0.75f;
				} else if (bps >= 0.6) {
					high_freq = high_freq * 0.6f;
				} else {
					high_freq = high_freq * 0.5f;
				}
			}

#if WMA_DEBUG
			av_dlog(s.avctx, "flags2=0x%x\n", flags2);
			av_dlog(s.avctx, "version=%d channels=%d sample_rate=%d bitrate=%d block_align=%d\n", s.version, s.nb_channels, s.sample_rate, s.bit_rate, s.block_align);
			av_dlog(s.avctx, "bps=%f bps1=%f high_freq=%f bitoffset=%d\n", bps, bps1, high_freq, s.byte_offset_bits);
			av_dlog(s.avctx, "use_noise_coding=%d use_exp_vlc=%d nb_block_sizes=%d\n", s.use_noise_coding, s.use_exp_vlc, s.nb_block_sizes);
#endif

			/* compute the scale factor band sizes for each MDCT block size */
			{
				int a, b, pos, lpos, k, block_len, j, n;
				//int i;
				byte[] table;

				if (s.version == 1) {
					s.coefs_start = 3;
				} else {
					s.coefs_start = 0;
				}
				for (k = 0; k < s.nb_block_sizes; k++) {
					block_len = s.frame_len >> k;

					if (s.version == 1) {
						lpos = 0;
						for (i = 0; i < 25; i++) {
							a = ff_wma_critical_freqs[i];
							b = s.sample_rate;
							pos = ((block_len * 2 * a) + (b >> 1)) / b;
							if (pos > block_len)
								pos = block_len;
							s.exponent_bands[0][i] = (ushort)(pos - lpos);
							if (pos >= block_len) {
								i++;
								break;
							}
							lpos = pos;
						}
						s.exponent_sizes[0] = i;
					} else {
						/* hardcoded tables */
						table = null;
						a = s.frame_len_bits - BLOCK_MIN_BITS - k;
						if (a < 3) {
							if (s.sample_rate >= 44100) {
								table = exponent_band_44100[a];
							} else if (s.sample_rate >= 32000) {
								table = exponent_band_32000[a];
							} else if (s.sample_rate >= 22050) {
								table = exponent_band_22050[a];
							}
						}
						if (table != null) {
							n = table.Length;
							for (i = 0; i < n; i++)
								s.exponent_bands[k][i] = table[i];
							s.exponent_sizes[k] = n;
						} else {
							j = 0;
							lpos = 0;
							for (i = 0; i < 25; i++) {
								a = ff_wma_critical_freqs[i];
								b = s.sample_rate;
								pos = ((block_len * 2 * a) + (b << 1)) / (4 * b);
								pos <<= 2;
								if (pos > block_len)
									pos = block_len;
								if (pos > lpos)
									s.exponent_bands[k][j++] = (ushort)(pos - lpos);
								if (pos >= block_len)
									break;
								lpos = pos;
							}
							s.exponent_sizes[k] = j;
						}
					}

					/* max number of coefs */
					s.coefs_end[k] = (s.frame_len - ((s.frame_len * 9) / 100)) >> k;
					/* high freq computation */
					s.high_band_start[k] = (int)((block_len * 2 * high_freq) /
												  s.sample_rate + 0.5);
					n = s.exponent_sizes[k];
					j = 0;
					pos = 0;
					for (i = 0; i < n; i++) {
						int start, end;
						start = pos;
						pos += s.exponent_bands[k][i];
						end = pos;
						if (start < s.high_band_start[k])
							start = s.high_band_start[k];
						if (end > s.coefs_end[k])
							end = s.coefs_end[k];
						if (end > start)
							s.exponent_high_bands[k,j++] = end - start;
					}
					s.exponent_high_sizes[k] = j;
#if WMA_DEBUG
					tprintf(s.avctx, "%5d: coefs_end=%d high_band_start=%d nb_high_bands=%d: ",
							s.frame_len >> k,
							s.coefs_end[k],
							s.high_band_start[k],
							s.exponent_high_sizes[k]);
					for (j = 0; j < s.exponent_high_sizes[k]; j++)
						tprintf(s.avctx, " %d", s.exponent_high_bands[k][j]);
					tprintf(s.avctx, "\n");
#endif
				}
			}

#if WMA_DEBUG
			{
				int i, j;
				for (i = 0; i < s.nb_block_sizes; i++) {
					tprintf(s.avctx, "%5d: n=%2d:",
							s.frame_len >> i,
							s.exponent_sizes[i]);
					for (j = 0; j < s.exponent_sizes[i]; j++)
						tprintf(s.avctx, " %d", s.exponent_bands[i][j]);
					tprintf(s.avctx, "\n");
				}
			}
#endif

			/* init MDCT windows : simple sinus window */
			for (i = 0; i < s.nb_block_sizes; i++) {
				ff_init_ff_sine_windows(s.frame_len_bits - i);
				s.windows[i] = ff_sine_windows[s.frame_len_bits - i];
			}

			s.reset_block_lengths = true;

			if (s.use_noise_coding) {

				/* init the noise generator */
				if (s.use_exp_vlc) {
					s.noise_mult = 0.02f;
				} else {
					s.noise_mult = 0.04f;
				}

		#if TRACE
				for (i = 0; i < NOISE_TAB_SIZE; i++)
					s.noise_table[i] = 1.0f * s.noise_mult;
		#else
				{
					uint seed;
					float norm;
					seed = 1;
					norm = (1.0 / (float)(1LL << 31)) * sqrt(3) * s.noise_mult;
					for (i = 0; i < NOISE_TAB_SIZE; i++) {
						seed = seed * 314159 + 1;
						s.noise_table[i] = (float)((int)seed) * norm;
					}
				}
		#endif
			}

			/* choose the VLC tables for the coefficients */
			coef_vlc_table = 2;
			if (s.sample_rate >= 32000) {
				if (bps1 < 0.72) {
					coef_vlc_table = 0;
				} else if (bps1 < 1.16) {
					coef_vlc_table = 1;
				}
			}
			s.coef_vlcs[0] = coef_vlcs[coef_vlc_table * 2    ];
			s.coef_vlcs[1] = coef_vlcs[coef_vlc_table * 2 + 1];
			s.coef_vlc[0].init_coef_vlc(s.run_table[0], s.level_table[0], s.int_table[0], s.coef_vlcs[0]);
			s.coef_vlc[1].init_coef_vlc(s.run_table[1], s.level_table[1], s.int_table[1], s.coef_vlcs[1]);

			return 0;
		}

		private static int av_log2(int p)
		{
			throw new NotImplementedException();
		}

		private static void ff_init_ff_sine_windows(int p)
		{
			throw new NotImplementedException();
		}

		static int ff_wma_total_gain_to_bits(int total_gain)
		{
				 if (total_gain < 15) return 13;
			else if (total_gain < 32) return 12;
			else if (total_gain < 40) return 11;
			else if (total_gain < 45) return 10;
			else                      return  9;
		}

		static int ff_wma_end(AVCodecContext avctx)
		{
			WMACodecContext s = (WMACodecContext)avctx.priv_data;
			int i;

			for (i = 0; i < s.nb_block_sizes; i++) s.mdct_ctx[i].ff_mdct_end();

			if (s.use_exp_vlc) s.exp_vlc.free_vlc();
			if (s.use_noise_coding) s.hgain_vlc.free_vlc();

			for (i = 0; i < 2; i++) {
				s.coef_vlc[i].free_vlc();
				//s.run_table[i] = null;
				//s.level_table[i] = null;
				//s.int_table[i] = null;
			}

			return 0;
		}

		/**
		 * Decode an uncompressed coefficient.
		 * @param gb GetBitContext
		 * @return the decoded coefficient
		 */
		static uint ff_wma_get_large_val(GetBitContext gb)
		{
			/** consumes up to 34 bits */
			int n_bits = 8;
			/** decode length */
			if (gb.get_bits1() != 0)
			{
				n_bits += 8;
				if (gb.get_bits1() != 0)
				{
					n_bits += 8;
					if (gb.get_bits1() != 0)
					{
						n_bits += 7;
					}
				}
			}
			return gb.get_bits_long(n_bits);
		}

		/**
		 * Decode run level compressed coefficients.
		 * @param avctx codec context
		 * @param gb bitstream reader context
		 * @param vlc vlc table for get_vlc2
		 * @param level_table level codes
		 * @param run_table run codes
		 * @param version 0 for wma1,2 1 for wmapro
		 * @param ptr output buffer
		 * @param offset offset in the output buffer
		 * @param num_coefs number of input coefficents
		 * @param block_len input buffer length (2^n)
		 * @param frame_len_bits number of bits for escaped run codes
		 * @param coef_nb_bits number of bits for escaped level codes
		 * @return 0 on success, -1 otherwise
		 */
		static int ff_wma_run_level_decode(AVCodecContext avctx, GetBitContext gb, VLC vlc, float* level_table, ushort* run_table, int version, WMACoef* ptr, int offset, int num_coefs, int block_len, int frame_len_bits, int coef_nb_bits)
		{
			int code, level, sign;
			uint *ilvl = (uint*)level_table;
			uint *iptr = (uint*)ptr;
			uint coef_mask = (uint)(block_len - 1);
			for (; offset < num_coefs; offset++) {
				code = gb.get_vlc2(vlc.table, VLCBITS, VLCMAX);
				if (code > 1) {
					/** normal code */
					offset += run_table[code];
					sign = gb.get_bits1() - 1;
					iptr[offset & coef_mask] = (uint)(ilvl[code] ^ sign<<31);
				} else if (code == 1) {
					/** EOB */
					break;
				} else {
					/** escape */
					if (version == 0) {
						level = gb.get_bits(coef_nb_bits);
						/** NOTE: this is rather suboptimal. reading
							block_len_bits would be better */
						offset += gb.get_bits(frame_len_bits);
					} else {
						level = gb.ff_wma_get_large_val();
						/** escape decode */
						if (gb.get_bits1() != 0)
						{
							if (gb.get_bits1() != 0)
							{
								if (gb.get_bits1() != 0)
								{
#if WMA_DEBUG
									av_log(avctx,AV_LOG_ERROR, "broken escape sequence\n");
#endif
									return -1;
								} else
									offset += gb.get_bits(frame_len_bits) + 4;
							} else
								offset += gb.get_bits(2) + 1;
						}
					}
					sign = gb.get_bits1() - 1;
					ptr[offset & coef_mask].Value = (level^sign) - sign;
				}
			}
			/** NOTE: EOB can be omitted */
			if (offset > num_coefs) {
#if WMA_DEBUG
				av_log(avctx, AV_LOG_ERROR, "overflow in spectral RLE, ignoring\n");
#endif
				return -1;
			}

			return 0;
		}

	}
}
