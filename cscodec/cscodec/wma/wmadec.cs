using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cscodec.wma
{
	unsafe public partial class wma
	{
		const int EXPVLCBITS = 8;
		const int EXPMAX = ((19+EXPVLCBITS-1)/EXPVLCBITS);

		const int HGAINVLCBITS = 9;
		const int HGAINMAX = ((13+HGAINVLCBITS-1)/HGAINVLCBITS);

#if WMA_DEBUG
		static void dump_shorts(WMACodecContext s, char *name, short *tab, int n)
		{
			int i;

			tprintf(s.avctx, "%s[%d]:\n", name, n);
			for(i=0;i<n;i++) {
				if ((i & 7) == 0)
					tprintf(s.avctx, "%4d: ", i);
				tprintf(s.avctx, " %5d.0", tab[i]);
				if ((i & 7) == 7)
					tprintf(s.avctx, "\n");
			}
		}

		static void dump_floats(WMACodecContext *s, char *name, int prec, float *tab, int n)
		{
			int i;

			tprintf(s.avctx, "%s[%d]:\n", name, n);
			for(i=0;i<n;i++) {
				if ((i & 7) == 0)
					tprintf(s.avctx, "%4d: ", i);
				tprintf(s.avctx, " %8.*f", prec, tab[i]);
				if ((i & 7) == 7)
					tprintf(s.avctx, "\n");
			}
			if ((i & 7) != 0)
				tprintf(s.avctx, "\n");
		}
#endif

		static int wma_decode_init(AVCodecContext avctx)
		{
			var s = (WMACodecContext)avctx.priv_data;
			int i, flags2;
			byte *extradata;

			s.avctx = avctx;

			/* extract flag infos */
			flags2 = 0;
			extradata = avctx.extradata;
			if (avctx.codec.id == CODEC_ID_WMAV1 && avctx.extradata_size >= 4) {
				flags2 = AV_RL16(extradata+2);
			} else if (avctx.codec.id == CODEC_ID_WMAV2 && avctx.extradata_size >= 6) {
				flags2 = AV_RL16(extradata+4);
			}
		// for(i=0; i<avctx.extradata_size; i++)
		//     av_log(NULL, AV_LOG_ERROR, "%02X ", extradata[i]);

			s.use_exp_vlc = (flags2 & 0x0001) != 0;
			s.use_bit_reservoir = (flags2 & 0x0002) != 0;
			s.use_variable_block_len = (flags2 & 0x0004) != 0;

			if(avctx.codec.id == CODEC_ID_WMAV2 && avctx.extradata_size >= 8){
				if(AV_RL16(extradata+4)==0xd && s.use_variable_block_len){
					avctx.av_log(AV_LOG_WARNING, "Disabling use_variable_block_len, if this fails contact the ffmpeg developers and send us the file\n");
					s.use_variable_block_len = false; // this fixes issue1503
				}
			}

			if(avctx.channels > MAX_CHANNELS){
				avctx.av_log(AV_LOG_ERROR, "Invalid number of channels (%d)\n", avctx.channels);
				return -1;
			}

			if(ff_wma_init(avctx, flags2)<0)
				return -1;

			/* init MDCT */
			for(i = 0; i < s.nb_block_sizes; i++)
				s.mdct_ctx[i].ff_mdct_init(s.frame_len_bits - i + 1, 1, 1.0);

			if (s.use_noise_coding) {
				s.hgain_vlc.init_vlc(HGAINVLCBITS, ff_wma_hgain_huffbits.Length, ff_wma_hgain_huffbits, 1, 1, ff_wma_hgain_huffcodes, 2, 2, 0);
			}

			if (s.use_exp_vlc) {
				//FIXME move out of context
				s.exp_vlc.init_vlc(EXPVLCBITS, acctab.ff_aac_scalefactor_bits.Length, acctab.ff_aac_scalefactor_bits, 1, 1, acctab.ff_aac_scalefactor_code, 4, 4, 0);
			} else {
				wma_lsp_to_curve_init(s, s.frame_len);
			}

			avctx.sample_fmt = AV_SAMPLE_FMT_S16;

			s.frame.avcodec_get_frame_defaults();
			avctx.coded_frame = s.frame;

			return 0;
		}

		private static int AV_RL16(byte* p)
		{
			throw new NotImplementedException();
		}



		/**
		 * compute x^-0.25 with an exponent and mantissa table. We use linear
		 * interpolation to reduce the mantissa table size at a small speed
		 * expense (linear interpolation approximately doubles the number of
		 * bits of precision).
		 */
		static float pow_m1_4(WMACodecContext s, float x)
		{
			var u = default(IntFloat);
			var t = default(IntFloat);
			uint e, m;
			float a, b;

			u.f = x;
			e = u.v >> 23;
			m = (u.v >> (23 - LSP_POW_BITS)) & ((1 << LSP_POW_BITS) - 1);
			/* build interpolation scale: 1 <= t < 2. */
			t.v = ((u.v << LSP_POW_BITS) & ((1 << 23) - 1)) | (127 << 23);
			a = s.lsp_pow_m_table1[m];
			b = s.lsp_pow_m_table2[m];
			return s.lsp_pow_e_table[e] * (a + b * t.f);
		}

		static void wma_lsp_to_curve_init(WMACodecContext s, int frame_len)
		{
			float wdel, a, b;
			int i, e, m;

			wdel = (float)(Math.PI / frame_len);
			for(i=0;i<frame_len;i++)
				s.lsp_cos_table[i] = 2.0f * (float)Math.Cos(wdel * i);

			/* tables for x^-0.25 computation */
			for(i=0;i<256;i++) {
				e = i - 126;
				s.lsp_pow_e_table[i] = (float)Math.Pow(2.0f, e * -0.25);
			}

			/* NOTE: these two tables are needed to avoid two operations in
			   pow_m1_4 */
			b = 1.0f;
			for(i=(1 << LSP_POW_BITS) - 1;i>=0;i--) {
				m = (1 << LSP_POW_BITS) + i;
				a = (float)(m * (0.5 / (1 << LSP_POW_BITS)));
				a = (float)Math.Pow(a, -0.25);
				s.lsp_pow_m_table1[i] = 2 * a - b;
				s.lsp_pow_m_table2[i] = b - a;
				b = a;
			}
		}

		/**
		 * NOTE: We use the same code as Vorbis here
		 * @todo optimize it further with SSE/3Dnow
		 */
		static void wma_lsp_to_curve(WMACodecContext s, float *Out, float *val_max_ptr, int n, float *lsp)
		{
			int i, j;
			float p, q, w, v, val_max;

			val_max = 0;
			for(i=0;i<n;i++) {
				p = 0.5f;
				q = 0.5f;
				w = s.lsp_cos_table[i];
				for(j=1;j<NB_LSP_COEFS;j+=2){
					q *= w - lsp[j - 1];
					p *= w - lsp[j];
				}
				p *= p * (2.0f - w);
				q *= q * (2.0f + w);
				v = p + q;
				v = pow_m1_4(s, v);
				if (v > val_max)
					val_max = v;
				Out[i] = v;
			}
			*val_max_ptr = val_max;
		}

		/**
		 * decode exponents coded with LSP coefficients (same idea as Vorbis)
		 */
		static void decode_exp_lsp(WMACodecContext s, int ch)
		{
			var lsp_coefs = new float[NB_LSP_COEFS];
			int val, i;

			for(i = 0; i < NB_LSP_COEFS; i++) {
				if (i == 0 || i >= 8)
					val = s.gb.get_bits(3);
				else
					val = s.gb.get_bits(4);
				lsp_coefs[i] = (float)ff_wma_lsp_codebook[i][val];
			}

			s.wma_lsp_to_curve(s.exponents[ch], ref s.max_exponent[ch], s.block_len, lsp_coefs);
		}

		/** pow(10, i / 16.0) for i in -60..95 */
		static readonly float[] pow_tab = new float[]
		{
			1.7782794100389e-04f, 2.0535250264571e-04f,
			2.3713737056617e-04f, 2.7384196342644e-04f,
			3.1622776601684e-04f, 3.6517412725484e-04f,
			4.2169650342858e-04f, 4.8696752516586e-04f,
			5.6234132519035e-04f, 6.4938163157621e-04f,
			7.4989420933246e-04f, 8.6596432336006e-04f,
			1.0000000000000e-03f, 1.1547819846895e-03f,
			1.3335214321633e-03f, 1.5399265260595e-03f,
			1.7782794100389e-03f, 2.0535250264571e-03f,
			2.3713737056617e-03f, 2.7384196342644e-03f,
			3.1622776601684e-03f, 3.6517412725484e-03f,
			4.2169650342858e-03f, 4.8696752516586e-03f,
			5.6234132519035e-03f, 6.4938163157621e-03f,
			7.4989420933246e-03f, 8.6596432336006e-03f,
			1.0000000000000e-02f, 1.1547819846895e-02f,
			1.3335214321633e-02f, 1.5399265260595e-02f,
			1.7782794100389e-02f, 2.0535250264571e-02f,
			2.3713737056617e-02f, 2.7384196342644e-02f,
			3.1622776601684e-02f, 3.6517412725484e-02f,
			4.2169650342858e-02f, 4.8696752516586e-02f,
			5.6234132519035e-02f, 6.4938163157621e-02f,
			7.4989420933246e-02f, 8.6596432336007e-02f,
			1.0000000000000e-01f, 1.1547819846895e-01f,
			1.3335214321633e-01f, 1.5399265260595e-01f,
			1.7782794100389e-01f, 2.0535250264571e-01f,
			2.3713737056617e-01f, 2.7384196342644e-01f,
			3.1622776601684e-01f, 3.6517412725484e-01f,
			4.2169650342858e-01f, 4.8696752516586e-01f,
			5.6234132519035e-01f, 6.4938163157621e-01f,
			7.4989420933246e-01f, 8.6596432336007e-01f,
			1.0000000000000e+00f, 1.1547819846895e+00f,
			1.3335214321633e+00f, 1.5399265260595e+00f,
			1.7782794100389e+00f, 2.0535250264571e+00f,
			2.3713737056617e+00f, 2.7384196342644e+00f,
			3.1622776601684e+00f, 3.6517412725484e+00f,
			4.2169650342858e+00f, 4.8696752516586e+00f,
			5.6234132519035e+00f, 6.4938163157621e+00f,
			7.4989420933246e+00f, 8.6596432336007e+00f,
			1.0000000000000e+01f, 1.1547819846895e+01f,
			1.3335214321633e+01f, 1.5399265260595e+01f,
			1.7782794100389e+01f, 2.0535250264571e+01f,
			2.3713737056617e+01f, 2.7384196342644e+01f,
			3.1622776601684e+01f, 3.6517412725484e+01f,
			4.2169650342858e+01f, 4.8696752516586e+01f,
			5.6234132519035e+01f, 6.4938163157621e+01f,
			7.4989420933246e+01f, 8.6596432336007e+01f,
			1.0000000000000e+02f, 1.1547819846895e+02f,
			1.3335214321633e+02f, 1.5399265260595e+02f,
			1.7782794100389e+02f, 2.0535250264571e+02f,
			2.3713737056617e+02f, 2.7384196342644e+02f,
			3.1622776601684e+02f, 3.6517412725484e+02f,
			4.2169650342858e+02f, 4.8696752516586e+02f,
			5.6234132519035e+02f, 6.4938163157621e+02f,
			7.4989420933246e+02f, 8.6596432336007e+02f,
			1.0000000000000e+03f, 1.1547819846895e+03f,
			1.3335214321633e+03f, 1.5399265260595e+03f,
			1.7782794100389e+03f, 2.0535250264571e+03f,
			2.3713737056617e+03f, 2.7384196342644e+03f,
			3.1622776601684e+03f, 3.6517412725484e+03f,
			4.2169650342858e+03f, 4.8696752516586e+03f,
			5.6234132519035e+03f, 6.4938163157621e+03f,
			7.4989420933246e+03f, 8.6596432336007e+03f,
			1.0000000000000e+04f, 1.1547819846895e+04f,
			1.3335214321633e+04f, 1.5399265260595e+04f,
			1.7782794100389e+04f, 2.0535250264571e+04f,
			2.3713737056617e+04f, 2.7384196342644e+04f,
			3.1622776601684e+04f, 3.6517412725484e+04f,
			4.2169650342858e+04f, 4.8696752516586e+04f,
			5.6234132519035e+04f, 6.4938163157621e+04f,
			7.4989420933246e+04f, 8.6596432336007e+04f,
			1.0000000000000e+05f, 1.1547819846895e+05f,
			1.3335214321633e+05f, 1.5399265260595e+05f,
			1.7782794100389e+05f, 2.0535250264571e+05f,
			2.3713737056617e+05f, 2.7384196342644e+05f,
			3.1622776601684e+05f, 3.6517412725484e+05f,
			4.2169650342858e+05f, 4.8696752516586e+05f,
			5.6234132519035e+05f, 6.4938163157621e+05f,
			7.4989420933246e+05f, 8.6596432336007e+05f,
		};

		/**
		 * decode exponents coded with VLC codes
		 */
		static int decode_exp_vlc(WMACodecContext s, int ch)
		{
			int last_exp, n, code;
			ushort *ptr;
			float v, max_scale;
			uint *q, q_end;
			uint iv;
			float *ptab = pow_tab + 60;
			uint *iptab = (uint*)ptab;

			ptr = s.exponent_bands[s.frame_len_bits - s.block_len_bits];
			q = (uint *)s.exponents[ch];
			q_end = q + s.block_len;
			max_scale = 0;
			if (s.version == 1) {
				last_exp = s.gb.get_bits(5) + 10;
				v = ptab[last_exp];
				iv = iptab[last_exp];
				max_scale = v;
				n = *ptr++;
				do {
					switch (n & 3) {
						case 0: *q++ = iv;
						case 3: *q++ = iv;
						case 2: *q++ = iv;
						case 1: *q++ = iv;
					}
				} while ((n -= 4) > 0);
			}else
				last_exp = 36;

			while (q < q_end) {
				code = s.gb.get_vlc2(s.exp_vlc.table, EXPVLCBITS, EXPMAX);
				if (code < 0){
					s.avctx.av_log(AV_LOG_ERROR, "Exponent vlc invalid\n");
					return -1;
				}
				/* NOTE: this offset is the same as MPEG4 AAC ! */
				last_exp += code - 60;
				if ((uint)last_exp + 60 > pow_tab.Length)
				{
					s.avctx.av_log(AV_LOG_ERROR, "Exponent out of range: %d\n", last_exp);
					return -1;
				}
				v = ptab[last_exp];
				iv = iptab[last_exp];
				if (v > max_scale)
					max_scale = v;
				n = *ptr++;
				do {
					switch (n & 3) {
						case 0: *q++ = iv;
						case 3: *q++ = iv;
						case 2: *q++ = iv;
						case 1: *q++ = iv;
					}
				} while ((n -= 4) > 0);
			}
			s.max_exponent[ch] = max_scale;
			return 0;
		}


		/**
		 * Apply MDCT window and add into output.
		 *
		 * We ensure that when the windows overlap their squared sum
		 * is always 1 (MDCT reconstruction rule).
		 */
		static void wma_window(WMACodecContext s, float *Out)
		{
			float *In = s.output;
			int block_len, bsize, n;

			/* left part */
			if (s.block_len_bits <= s.prev_block_len_bits) {
				block_len = s.block_len;
				bsize = s.frame_len_bits - s.block_len_bits;

				s.dsp.vector_fmul_add(Out, In, s.windows[bsize], Out, block_len);

			} else {
				block_len = 1 << s.prev_block_len_bits;
				n = (s.block_len - block_len) / 2;
				bsize = s.frame_len_bits - s.prev_block_len_bits;

				s.dsp.vector_fmul_add(Out+n, In+n, s.windows[bsize], Out+n, block_len);

				memcpy(Out+n+block_len, In+n+block_len, n*sizeof(float));
			}

			Out += s.block_len;
			In += s.block_len;

			/* right part */
			if (s.block_len_bits <= s.next_block_len_bits) {
				block_len = s.block_len;
				bsize = s.frame_len_bits - s.block_len_bits;

				s.dsp.vector_fmul_reverse(Out, In, s.windows[bsize], block_len);

			} else {
				block_len = 1 << s.next_block_len_bits;
				n = (s.block_len - block_len) / 2;
				bsize = s.frame_len_bits - s.next_block_len_bits;

				memcpy(Out, In, n*sizeof(float));

				s.dsp.vector_fmul_reverse(Out+n, In+n, s.windows[bsize], block_len);

				memset(Out+n+block_len, 0, n*sizeof(float));
			}
		}

		private static void memset(int p, int p_2, int p_3)
		{
			throw new NotImplementedException();
		}

		private static unsafe void memcpy(float* Out, float* In, int p)
		{
			throw new NotImplementedException();
		}

		private static void memcpy(int p, int p_2, int p_3)
		{
			throw new NotImplementedException();
		}


		/**
		 * @return 0 if OK. 1 if last block of frame. return -1 if
		 * unrecorrable error.
		 */
		static int wma_decode_block(WMACodecContext s)
		{
			int n, v, a, ch, bsize;
			int coef_nb_bits, total_gain;
			var nb_coefs = new int[MAX_CHANNELS];
			float mdct_norm;
			FFTContext mdct;

#if WMA_DEBUG
			tprintf(s.avctx, "***decode_block: %d:%d\n", s.frame_count - 1, s.block_num);
#endif

			/* compute current block length */
			if (s.use_variable_block_len) {
				n = av_log2(s.nb_block_sizes - 1) + 1;

				if (s.reset_block_lengths) {
					s.reset_block_lengths = false;
					v = s.gb.get_bits(n);
					if (v >= s.nb_block_sizes){
						s.avctx.av_log(AV_LOG_ERROR, "prev_block_len_bits %d out of range\n", s.frame_len_bits - v);
						return -1;
					}
					s.prev_block_len_bits = s.frame_len_bits - v;
					v = s.gb.get_bits(n);
					if (v >= s.nb_block_sizes){
						s.avctx.av_log(AV_LOG_ERROR, "block_len_bits %d out of range\n", s.frame_len_bits - v);
						return -1;
					}
					s.block_len_bits = s.frame_len_bits - v;
				} else {
					/* update block lengths */
					s.prev_block_len_bits = s.block_len_bits;
					s.block_len_bits = s.next_block_len_bits;
				}
				v = s.gb.get_bits(n);
				if (v >= s.nb_block_sizes){
					s.avctx.av_log(AV_LOG_ERROR, "next_block_len_bits %d out of range\n", s.frame_len_bits - v);
					return -1;
				}
				s.next_block_len_bits = s.frame_len_bits - v;
			} else {
				/* fixed block len */
				s.next_block_len_bits = s.frame_len_bits;
				s.prev_block_len_bits = s.frame_len_bits;
				s.block_len_bits = s.frame_len_bits;
			}

			if (s.frame_len_bits - s.block_len_bits >= s.nb_block_sizes){
				s.avctx.av_log(AV_LOG_ERROR, "block_len_bits not initialized to a valid value\n");
				return -1;
			}

			/* now check if the block length is coherent with the frame length */
			s.block_len = 1 << s.block_len_bits;
			if ((s.block_pos + s.block_len) > s.frame_len){
				s.avctx.av_log(AV_LOG_ERROR, "frame_len overflow\n");
				return -1;
			}

			if (s.nb_channels == 2) {
				s.ms_stereo = (s.gb.get_bits1() != 0);
			}
			v = 0;
			for(ch = 0; ch < s.nb_channels; ch++) {
				a = s.gb.get_bits1();
				s.channel_coded[ch] = (a != 0);
				v |= a;
			}

			bsize = s.frame_len_bits - s.block_len_bits;

			/* if no channel coded, no need to go further */
			/* XXX: fix potential framing problems */
			if (v == 0)
				goto next;

			/* read total gain and extract corresponding number of bits for
			   coef escape coding */
			total_gain = 1;
			for(;;) {
				a = s.gb.get_bits(7);
				total_gain += a;
				if (a != 127)
					break;
			}

			coef_nb_bits= ff_wma_total_gain_to_bits(total_gain);

			/* compute number of coefficients */
			n = s.coefs_end[bsize] - s.coefs_start;
			for(ch = 0; ch < s.nb_channels; ch++)
				nb_coefs[ch] = n;

			/* complex coding */
			if (s.use_noise_coding) {

				for(ch = 0; ch < s.nb_channels; ch++) {
					if (s.channel_coded[ch]) {
						int i;
						//int n;
						//int a;
						n = s.exponent_high_sizes[bsize];
						for(i=0;i<n;i++) {
							a = s.gb.get_bits1();
							s.high_band_coded[ch,i] = a;
							/* if noise coding, the coefficients are not transmitted */
							if (a != 0) nb_coefs[ch] -= s.exponent_high_bands[bsize,i];
						}
					}
				}
				for(ch = 0; ch < s.nb_channels; ch++) {
					if (s.channel_coded[ch]) {
						int i, val, code;
						//int n;

						n = s.exponent_high_sizes[bsize];
						val = unchecked((int)0x80000000);
						for(i=0;i<n;i++) {
							if (s.high_band_coded[ch,i] != 0) {
								if (val == unchecked((int)0x80000000)) {
									val = s.gb.get_bits(7) - 19;
								} else {
									code = s.gb.get_vlc2(s.hgain_vlc.table, HGAINVLCBITS, HGAINMAX);
									if (code < 0){
										s.avctx.av_log(AV_LOG_ERROR, "hgain vlc invalid\n");
										return -1;
									}
									val += code - 18;
								}
								s.high_band_values[ch,i] = val;
							}
						}
					}
				}
			}

			/* exponents can be reused in short blocks. */
			if ((s.block_len_bits == s.frame_len_bits) || (s.gb.get_bits1() != 0)) {
				for(ch = 0; ch < s.nb_channels; ch++) {
					if (s.channel_coded[ch]) {
						if (s.use_exp_vlc) {
							if (decode_exp_vlc(s, ch) < 0)
								return -1;
						} else {
							decode_exp_lsp(s, ch);
						}
						s.exponents_bsize[ch] = bsize;
					}
				}
			}

			/* parse spectral coefficients : just RLE encoding */
			for(ch = 0; ch < s.nb_channels; ch++) {
				if (s.channel_coded[ch]) {
					int tindex;
					WMACoef* ptr = &s.coefs1[ch][0];

					/* special VLC tables are used for ms stereo because
					   there is potentially less energy there */
					tindex = (ch == 1 && s.ms_stereo) ? 1 : 0;
					memset(ptr, 0, s.block_len * sizeof(WMACoef));
					s.avctx.ff_wma_run_level_decode(
						&s.gb, &s.coef_vlc[tindex],
						s.level_table[tindex], s.run_table[tindex],
						0, ptr, 0, nb_coefs[ch],
						s.block_len, s.frame_len_bits, coef_nb_bits
					);
				}
				if (s.version == 1 && s.nb_channels >= 2) {
					s.gb.align_get_bits();
				}
			}

			/* normalize */
			{
				int n4 = s.block_len / 2;
				mdct_norm = 1.0f / (float)n4;
				if (s.version == 1) {
					mdct_norm *= (float)Math.Sqrt(n4);
				}
			}

			/* finally compute the MDCT coefficients */
			for (ch = 0; ch < s.nb_channels; ch++)
			{
				if (s.channel_coded[ch]) {
					WMACoef[] coefs1;
					float* coefs, exponents;
					float mult, mult1, noise;
					int i, j, n1, last_high_band, esize;
					//int n;
					var exp_power = new float[HIGH_BAND_MAX_SIZE];

					coefs1 = s.coefs1[ch];
					exponents = s.exponents[ch];
					esize = s.exponents_bsize[ch];
					mult = (float)Math.Pow(10, total_gain * 0.05) / s.max_exponent[ch];
					mult *= mdct_norm;
					coefs = s.coefs[ch];
					if (s.use_noise_coding) {
						mult1 = mult;
						/* very low freqs : noise */
						for(i = 0;i < s.coefs_start; i++) {
							coefs[coefs_index++] = s.noise_table[s.noise_index] * exponents[i<<bsize>>esize] * mult1;
							s.noise_index = (s.noise_index + 1) & (NOISE_TAB_SIZE - 1);
						}

						n1 = s.exponent_high_sizes[bsize];

						/* compute power of high bands */
						exponents = s.exponents[ch] + (s.high_band_start[bsize]<<bsize>>esize);
						last_high_band = 0; /* avoid warning */
						for(j=0;j<n1;j++) {
							n = s.exponent_high_bands[s.frame_len_bits -
													  s.block_len_bits][j];
							if (s.high_band_coded[ch,j] != 0) {
								float e2;
								float v2;
								e2 = 0;
								for(i = 0;i < n; i++) {
									v2 = exponents[i<<bsize>>esize];
									e2 += v2 * v2;
								}
								exp_power[j] = e2 / n;
								last_high_band = j;
								s.avctx.tprintf("%d: power=%f (%d)\n", j, exp_power[j], n);
							}
							exponents += n<<bsize>>esize;
						}

						/* main freqs and high freqs */
						exponents = s.exponents[ch] + (s.coefs_start<<bsize>>esize);
						for(j=-1;j<n1;j++) {
							if (j < 0) {
								n = s.high_band_start[bsize] -
									s.coefs_start;
							} else {
								n = s.exponent_high_bands[s.frame_len_bits -
														  s.block_len_bits][j];
							}
							if (j >= 0 && (s.high_band_coded[ch,j] != 0)) {
								/* use noise with specified power */
								mult1 = (float)Math.Sqrt(exp_power[j] / exp_power[last_high_band]);
								/* XXX: use a table */
								mult1 = mult1 * (float)Math.Pow(10, s.high_band_values[ch,j] * 0.05);
								mult1 = mult1 / (s.max_exponent[ch] * s.noise_mult);
								mult1 *= mdct_norm;
								for(i = 0;i < n; i++) {
									noise = s.noise_table[s.noise_index];
									s.noise_index = (s.noise_index + 1) & (NOISE_TAB_SIZE - 1);
									*coefs++ = noise * exponents[i << bsize >> esize] * mult1;
								}
								exponents += n << bsize >> esize;
							} else {
								/* coded values + small noise */
								for(i = 0;i < n; i++) {
									noise = s.noise_table[s.noise_index];
									s.noise_index = (s.noise_index + 1) & (NOISE_TAB_SIZE - 1);
									*coefs++ = ((*coefs1++) + noise) *
										exponents[i<<bsize>>esize] * mult;
								}
								exponents += n<<bsize>>esize;
							}
						}

						/* very high freqs : noise */
						n = s.block_len - s.coefs_end[bsize];
						mult1 = mult * exponents[((-1<<bsize))>>esize];
						for(i = 0; i < n; i++) {
							*coefs++ = s.noise_table[s.noise_index] * mult1;
							s.noise_index = (s.noise_index + 1) & (NOISE_TAB_SIZE - 1);
						}
					} else {
						/* XXX: optimize more */
						for(i = 0;i < s.coefs_start; i++)
							*coefs++ = 0.0f;
						n = nb_coefs[ch];
						for(i = 0;i < n; i++) {
							*coefs++ = (float)(coefs1[i].Value * exponents[i<<bsize>>esize] * mult);
						}
						n = s.block_len - s.coefs_end[bsize];
						for(i = 0;i < n; i++)
							*coefs++ = 0.0f;
					}
				}
			}

		#if WMA_DEBUG
			for(ch = 0; ch < s.nb_channels; ch++) {
				if (s.channel_coded[ch]) {
					dump_floats(s, "exponents", 3, s.exponents[ch], s.block_len);
					dump_floats(s, "coefs", 1, s.coefs[ch], s.block_len);
				}
			}
		#endif

			if (s.ms_stereo && s.channel_coded[1]) {
				/* nominal case for ms stereo: we do it before mdct */
				/* no need to optimize this case because it should almost
				   never happen */
				if (!s.channel_coded[0]) {
					s.avctx.tprintf("rare ms-stereo case happened\n");
					memset(s.coefs[0], 0, sizeof(float) * s.block_len);
					s.channel_coded[0] = true;
				}

				s.dsp.butterflies_float(s.coefs[0], s.coefs[1], s.block_len);
			}

		next:
			mdct = s.mdct_ctx[bsize];

			for(ch = 0; ch < s.nb_channels; ch++) {
				int n4, index;

				n4 = s.block_len / 2;
				if(s.channel_coded[ch]){
					mdct.imdct_calc(s.output, s.coefs[ch]);
				}else if(!(s.ms_stereo && ch==1))
					memset(s.output, 0, sizeof(s.output));

				/* multiply by the window and add in the frame */
				index = (s.frame_len / 2) + s.block_pos - n4;
				s.wma_window(&s.frame_out[ch][index]);
			}

			/* update block number */
			s.block_num++;
			s.block_pos += s.block_len;
			if (s.block_pos >= s.frame_len)
				return 1;
			else
				return 0;
		}

		private static unsafe void memset(WMACoef* ptr, int p, int p_2)
		{
			throw new NotImplementedException();
		}

		private static void memset(FFTSample[] fFTSample, int p, int p_2)
		{
			throw new NotImplementedException();
		}

		private static void memset(float[] p, int p_2, int p_3)
		{
			throw new NotImplementedException();
		}

		/* decode a frame of frame_len samples */
		static int wma_decode_frame(WMACodecContext s, short *samples)
		{
			int ret, n, ch, incr;
			var output = new float[MAX_CHANNELS];

		#if WMA_DEBUG
			tprintf(s.avctx, "***decode_frame: %d size=%d\n", s.frame_count++, s.frame_len);
		#endif

			/* read each block */
			s.block_num = 0;
			s.block_pos = 0;
			for(;;) {
				ret = wma_decode_block(s);
				if (ret < 0) return -1;
				if (ret != 0) break;
			}

			/* convert frame to integer */
			n = s.frame_len;
			incr = s.nb_channels;
			for (ch = 0; ch < MAX_CHANNELS; ch++)
				output[ch] = s.frame_out[ch];
			s.fmt_conv.float_to_int16_interleave(samples, output, n, incr);
			for (ch = 0; ch < incr; ch++) {
				/* prepare for next block */
				memmove(&s.frame_out[ch][0], &s.frame_out[ch][n], n * sizeof(float));
			}

#if WMA_DEBUG
			dump_shorts(s, "samples", samples, n * s.nb_channels);
#endif
			return 0;
		}

		private static unsafe void memmove(float* p, float* p_2, int p_3)
		{
			throw new NotImplementedException();
		}


		const int EINVAL = -1;
		static int AVERROR(int Value)
		{
			throw(new NotImplementedException());
			//return Value;
		}

		static int wma_decode_superframe(AVCodecContext avctx, ref AVFrame data, int *got_frame_ptr, AVPacket avpkt)
		{
			byte *buf = avpkt.data;
			int buf_size = avpkt.size;
			var s = (WMACodecContext)avctx.priv_data;
			int nb_frames, bit_offset, i, pos, len, ret;
			byte *q;
			short *samples;

			avctx.tprintf("***decode_superframe:\n");

			if(buf_size==0){
				s.last_superframe_len = 0;
				return 0;
			}
			if (buf_size < s.block_align)
				return AVERROR(EINVAL);
			if(s.block_align != 0)
				buf_size = s.block_align;

			s.gb.init_get_bits(buf, buf_size * 8);

			if (s.use_bit_reservoir) {
				/* read super frame header */
				s.gb.skip_bits(4); /* super frame index */
				nb_frames = (int)(s.gb.get_bits(4) - ((s.last_superframe_len <= 0) ? 1 : 0));
			} else {
				nb_frames = 1;
			}

			/* get output buffer */
			s.frame.nb_samples = nb_frames * s.frame_len;
			if ((ret = avctx.get_buffer(s.frame)) < 0) {
				avctx.av_log(AV_LOG_ERROR, "get_buffer() failed\n");
				return ret;
			}
			samples = (short *)s.frame.data[0];

			if (s.use_bit_reservoir) {
				bit_offset = s.gb.get_bits(s.byte_offset_bits + 3);

				if (s.last_superframe_len > 0) {
					//        printf("skip=%d\n", s.last_bitoffset);
					/* add bit_offset bits to last frame */
					if ((s.last_superframe_len + ((bit_offset + 7) >> 3)) >
						MAX_CODED_SUPERFRAME_SIZE)
						goto fail;
					q = s.last_superframe + s.last_superframe_len;
					len = bit_offset;
					while (len > 7) {
						*q++ = (byte)(s.gb.get_bits(8));
						len -= 8;
					}
					if (len > 0) {
						*q++ = (byte)(s.gb.get_bits(len) << (8 - len));
					}

					/* XXX: bit_offset bits into last frame */
					s.gb.init_get_bits(s.last_superframe, MAX_CODED_SUPERFRAME_SIZE * 8);
					/* skip unused bits */
					if (s.last_bitoffset > 0)
						s.gb.skip_bits(s.last_bitoffset);
					/* this frame is stored in the last superframe and in the
					   current one */
					if (wma_decode_frame(s, samples) < 0)
						goto fail;
					samples += s.nb_channels * s.frame_len;
					nb_frames--;
				}

				/* read each frame starting from bit_offset */
				pos = bit_offset + 4 + 4 + s.byte_offset_bits + 3;
				if (pos >= MAX_CODED_SUPERFRAME_SIZE * 8)
					return AVERROR_INVALIDDATA;
				s.gb.init_get_bits(buf + (pos >> 3), (MAX_CODED_SUPERFRAME_SIZE - (pos >> 3)) * 8);
				len = pos & 7;
				if (len > 0)
					s.gb.skip_bits(len);

				s.reset_block_lengths = true;
				for(i=0;i<nb_frames;i++) {
					if (wma_decode_frame(s, samples) < 0)
						goto fail;
					samples += s.nb_channels * s.frame_len;
				}

				/* we copy the end of the frame in the last frame buffer */
				pos = s.gb.get_bits_count() + ((bit_offset + 4 + 4 + s.byte_offset_bits + 3) & ~7);
				s.last_bitoffset = pos & 7;
				pos >>= 3;
				len = buf_size - pos;
				if (len > MAX_CODED_SUPERFRAME_SIZE || len < 0) {
					s.avctx.av_log(AV_LOG_ERROR, "len %d invalid\n", len);
					goto fail;
				}
				s.last_superframe_len = len;
				memcpy(s.last_superframe, buf + pos, len);
			} else {
				/* single frame decode */
				if (wma_decode_frame(s, samples) < 0)
					goto fail;
				samples += s.nb_channels * s.frame_len;
			}

		//av_log(NULL, AV_LOG_ERROR, "%d %d %d %d outbytes:%d eaten:%d\n", s.frame_len_bits, s.block_len_bits, s.frame_len, s.block_len,        (int8_t *)samples - (int8_t *)data, s.block_align);

			*got_frame_ptr   = 1;
			data = s.frame;

			return buf_size;
		 fail:
			/* when error, we reset the bit reservoir */
			s.last_superframe_len = 0;
			return -1;
		}

		static /*av_cold*/ void flush(AVCodecContext avctx)
		{
			var s = (WMACodecContext)avctx.priv_data;

			s.last_bitoffset = 0;
			s.last_superframe_len = 0;
		}

#if false
		AVCodec ff_wmav1_decoder = {
			.name           = "wmav1",
			.type           = AVMEDIA_TYPE_AUDIO,
			.id             = CODEC_ID_WMAV1,
			.priv_data_size = sizeof(WMACodecContext),
			.init           = wma_decode_init,
			.close          = ff_wma_end,
			.decode         = wma_decode_superframe,
			.flush          = flush,
			.capabilities   = CODEC_CAP_DR1,
			.long_name      = NULL_IF_CONFIG_SMALL("Windows Media Audio 1"),
		};

		AVCodec ff_wmav2_decoder = {
			.name           = "wmav2",
			.type           = AVMEDIA_TYPE_AUDIO,
			.id             = CODEC_ID_WMAV2,
			.priv_data_size = sizeof(WMACodecContext),
			.init           = wma_decode_init,
			.close          = ff_wma_end,
			.decode         = wma_decode_superframe,
			.flush          = flush,
			.capabilities   = CODEC_CAP_DR1,
			.long_name      = NULL_IF_CONFIG_SMALL("Windows Media Audio 2"),
		};
#endif
	}
}
