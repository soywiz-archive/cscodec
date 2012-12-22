namespace cscodec.h243.decoder
{
	public class CABACContext
	{

		public const int CABAC_BITS = 16;
		public const int CABAC_MASK = ((1 << CABAC_BITS) - 1);

		public int low;
		public int range;
		public int outstanding_count;
		public int symCount;
		public int bytestream_start;
		public int bytestream_current;
		public int[] bytestream;
		public int bytestream_end;
		public PutBitContext pb;

		public static readonly short[][] lps_range = new short[][] {
    	new short[] {128,176,208,240}, new short[] {128,167,197,227}, new short[] {128,158,187,216}, new short[] {123,150,178,205},
    	new short[] {116,142,169,195}, new short[] {111,135,160,185}, new short[] {105,128,152,175}, new short[] {100,122,144,166},
    	new short[] { 95,116,137,158}, new short[] { 90,110,130,150}, new short[] { 85,104,123,142}, new short[] { 81, 99,117,135},
    	new short[] { 77, 94,111,128}, new short[] { 73, 89,105,122}, new short[] { 69, 85,100,116}, new short[] { 66, 80, 95,110},
    	new short[] { 62, 76, 90,104}, new short[] { 59, 72, 86, 99}, new short[] { 56, 69, 81, 94}, new short[] { 53, 65, 77, 89},
    	new short[] { 51, 62, 73, 85}, new short[] { 48, 59, 69, 80}, new short[] { 46, 56, 66, 76}, new short[] { 43, 53, 63, 72},
    	new short[] { 41, 50, 59, 69}, new short[] { 39, 48, 56, 65}, new short[] { 37, 45, 54, 62}, new short[] { 35, 43, 51, 59},
    	new short[] { 33, 41, 48, 56}, new short[] { 32, 39, 46, 53}, new short[] { 30, 37, 43, 50}, new short[] { 29, 35, 41, 48},
    	new short[] { 27, 33, 39, 45}, new short[] { 26, 31, 37, 43}, new short[] { 24, 30, 35, 41}, new short[] { 23, 28, 33, 39},
    	new short[] { 22, 27, 32, 37}, new short[] { 21, 26, 30, 35}, new short[] { 20, 24, 29, 33}, new short[] { 19, 23, 27, 31},
    	new short[] { 18, 22, 26, 30}, new short[] { 17, 21, 25, 28}, new short[] { 16, 20, 23, 27}, new short[] { 15, 19, 22, 25},
    	new short[] { 14, 18, 21, 24}, new short[] { 14, 17, 20, 23}, new short[] { 13, 16, 19, 22}, new short[] { 12, 15, 18, 21},
    	new short[] { 12, 14, 17, 20}, new short[] { 11, 14, 16, 19}, new short[] { 11, 13, 15, 18}, new short[] { 10, 12, 15, 17},
    	new short[] { 10, 12, 14, 16}, new short[] {  9, 11, 13, 15}, new short[] {  9, 11, 12, 14}, new short[] {  8, 10, 12, 14},
    	new short[] {  8,  9, 11, 13}, new short[] {  7,  9, 11, 12}, new short[] {  7,  9, 10, 12}, new short[] {  7,  8, 10, 11},
    	new short[] {  6,  8,  9, 11}, new short[] {  6,  7,  9, 10}, new short[] {  6,  7,  8,  9}, new short[] {  2,  2,  2,  2},
    	};

		public short[] ff_h264_mlps_state = new short[4 * 64];
		public short[] ff_h264_lps_range = new short[4 * 2 * 64];
		public short[] ff_h264_lps_state = new short[2 * 64];
		public short[] ff_h264_mps_state = new short[2 * 64];

		public static readonly short[] mps_state = {
    	  1, 2, 3, 4, 5, 6, 7, 8,
    	  9,10,11,12,13,14,15,16,
    	 17,18,19,20,21,22,23,24,
    	 25,26,27,28,29,30,31,32,
    	 33,34,35,36,37,38,39,40,
    	 41,42,43,44,45,46,47,48,
    	 49,50,51,52,53,54,55,56,
    	 57,58,59,60,61,62,62,63,
    	};

		public static readonly short[] lps_state = {
    	  0, 0, 1, 2, 2, 4, 4, 5,
    	  6, 7, 8, 9, 9,11,11,12,
    	 13,13,15,15,16,16,18,18,
    	 19,19,21,21,22,22,23,24,
    	 24,25,26,26,27,27,28,29,
    	 29,30,30,30,31,32,32,33,
    	 33,33,34,34,35,35,35,36,
    	 36,36,37,37,37,38,38,63,
    	};

		public static readonly short[] ff_h264_norm_shift = {
    	 9,8,7,7,6,6,6,6,5,5,5,5,5,5,5,5,
    	 4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
    	 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
    	 3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,3,
    	 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    	 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    	 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    	 2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,2,
    	 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    	 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    	 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    	 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
    	};

		public static readonly sbyte[][] cabac_context_init_I =
		new sbyte[][] {
	    new sbyte[]{ 20, -15 }, new sbyte[]{  2, 54 },  new sbyte[]{  3,  74 }, new sbyte[]{ 20, -15 }, // 0 - 10
	    new sbyte[]{  2,  54 }, new sbyte[]{  3, 74 },  new sbyte[]{ -28,127 }, new sbyte[]{ -23, 104 },
	    new sbyte[]{ -6,  53 }, new sbyte[]{ -1, 54 },  new sbyte[]{  7,  51 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 }, // 11 - 23 unsused for I
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 }, // 24- 39
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 }, // 40 - 53
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },      new sbyte[]{ 0, 0 }, // 54 - 59
	    new sbyte[]{ 0, 0 },    new sbyte[]{ 0, 0 },
	    new sbyte[]{ 0, 41 },   new sbyte[]{ 0, 63 },   new sbyte[]{ 0, 63 },     new sbyte[]{ 0, 63 }, // 60 - 69
	    new sbyte[]{ -9, 83 },  new sbyte[]{ 4, 86 },   new sbyte[]{ 0, 97 },     new sbyte[]{ -7, 72 },
	    new sbyte[]{ 13, 41 },  new sbyte[]{ 3, 62 },
	    new sbyte[]{ 0, 11 },   new sbyte[]{ 1, 55 },   new sbyte[]{ 0, 69 },     new sbyte[]{ -17, 127 }, // 70 -> 87
	    new sbyte[]{ -13, 102 },new sbyte[]{ 0, 82 },   new sbyte[]{ -7, 74 },    new sbyte[]{ -21, 107 },
	    new sbyte[]{ -27, 127 },new sbyte[]{ -31, 127 },new sbyte[]{ -24, 127 },  new sbyte[]{ -18, 95 },
	    new sbyte[]{ -27, 127 },new sbyte[]{ -21, 114 },new sbyte[]{ -30, 127 },  new sbyte[]{ -17, 123 },
	    new sbyte[]{ -12, 115 },new sbyte[]{ -16, 122 },
	    new sbyte[]{ -11, 115 },new sbyte[]{ -12, 63 }, new sbyte[]{ -2, 68 },    new sbyte[]{ -15, 84 }, // 88 -> 104
	    new sbyte[]{ -13, 104 },new sbyte[]{ -3, 70 },  new sbyte[]{ -8, 93 },    new sbyte[]{ -10, 90 },
	    new sbyte[]{ -30, 127 },new sbyte[]{ -1, 74 },  new sbyte[]{ -6, 97 },    new sbyte[]{ -7, 91 },
	    new sbyte[]{ -20, 127 },new sbyte[]{ -4, 56 },  new sbyte[]{ -5, 82 },    new sbyte[]{ -7, 76 },
	    new sbyte[]{ -22, 125 },
	    new sbyte[]{ -7, 93 },  new sbyte[]{ -11, 87 }, new sbyte[]{ -3, 77 },    new sbyte[]{ -5, 71 }, // 105 -> 135
	    new sbyte[]{ -4, 63 },  new sbyte[]{ -4, 68 },  new sbyte[]{ -12, 84 },   new sbyte[]{ -7, 62 },
	    new sbyte[]{ -7, 65 },  new sbyte[]{ 8, 61 },   new sbyte[]{ 5, 56 },     new sbyte[]{ -2, 66 },
	    new sbyte[]{ 1, 64 },   new sbyte[]{ 0, 61 },   new sbyte[]{ -2, 78 },    new sbyte[]{ 1, 50 },
	    new sbyte[]{ 7, 52 },   new sbyte[]{ 10, 35 },  new sbyte[]{ 0, 44 },     new sbyte[]{ 11, 38 },
	    new sbyte[]{ 1, 45 },   new sbyte[]{ 0, 46 },   new sbyte[]{ 5, 44 },     new sbyte[]{ 31, 17 },
	    new sbyte[]{ 1, 51 },   new sbyte[]{ 7, 50 },   new sbyte[]{ 28, 19 },    new sbyte[]{ 16, 33 },
	    new sbyte[]{ 14, 62 },  new sbyte[]{ -13, 108 },new sbyte[]{ -15, 100 },
	    new sbyte[]{ -13, 101 },new sbyte[]{ -13, 91 }, new sbyte[]{ -12, 94 },   new sbyte[]{ -10, 88 }, //136 -> 165
	    new sbyte[]{ -16, 84 }, new sbyte[]{ -10, 86 }, new sbyte[]{ -7, 83 },    new sbyte[]{ -13, 87 },
	    new sbyte[]{ -19, 94 }, new sbyte[]{ 1, 70 },   new sbyte[]{ 0, 72 },     new sbyte[]{ -5, 74 },
	    new sbyte[]{ 18, 59 },  new sbyte[]{ -8, 102 }, new sbyte[]{ -15, 100 },  new sbyte[]{ 0, 95 },
	    new sbyte[]{ -4, 75 },  new sbyte[]{ 2, 72 },   new sbyte[]{ -11, 75 },   new sbyte[]{ -3, 71 },
	    new sbyte[]{ 15, 46 },  new sbyte[]{ -13, 69 }, new sbyte[]{ 0, 62 },     new sbyte[]{ 0, 65 },
	    new sbyte[]{ 21, 37 },  new sbyte[]{ -15, 72 }, new sbyte[]{ 9, 57 },     new sbyte[]{ 16, 54 },
	    new sbyte[]{ 0, 62 },   new sbyte[]{ 12, 72 },
	    new sbyte[]{ 24, 0 },   new sbyte[]{ 15, 9 },   new sbyte[]{ 8, 25 },     new sbyte[]{ 13, 18 }, // 166 -> 196
	    new sbyte[]{ 15, 9 },   new sbyte[]{ 13, 19 },  new sbyte[]{ 10, 37 },    new sbyte[]{ 12, 18 },
	    new sbyte[]{ 6, 29 },   new sbyte[]{ 20, 33 },  new sbyte[]{ 15, 30 },    new sbyte[]{ 4, 45 },
	    new sbyte[]{ 1, 58 },   new sbyte[]{ 0, 62 },   new sbyte[]{ 7, 61 },     new sbyte[]{ 12, 38 },
	    new sbyte[]{ 11, 45 },  new sbyte[]{ 15, 39 },  new sbyte[]{ 11, 42 },    new sbyte[]{ 13, 44 },
	    new sbyte[]{ 16, 45 },  new sbyte[]{ 12, 41 },  new sbyte[]{ 10, 49 },    new sbyte[]{ 30, 34 },
	    new sbyte[]{ 18, 42 },  new sbyte[]{ 10, 55 },  new sbyte[]{ 17, 51 },    new sbyte[]{ 17, 46 },
	    new sbyte[]{ 0, 89 },   new sbyte[]{ 26, -19 }, new sbyte[]{ 22, -17 },	 
	    new sbyte[]{ 26, -17 }, new sbyte[]{ 30, -25 }, new sbyte[]{ 28, -20 },   new sbyte[]{ 33, -23 }, // 197 -> 226
	    new sbyte[]{ 37, -27 }, new sbyte[]{ 33, -23 }, new sbyte[]{ 40, -28 },   new sbyte[]{ 38, -17 },
	    new sbyte[]{ 33, -11 }, new sbyte[]{ 40, -15 }, new sbyte[]{ 41, -6 },    new sbyte[]{ 38, 1 },
	    new sbyte[]{ 41, 17 },  new sbyte[]{ 30, -6 },  new sbyte[]{ 27, 3 },     new sbyte[]{ 26, 22 },
	    new sbyte[]{ 37, -16 }, new sbyte[]{ 35, -4 },  new sbyte[]{ 38, -8 },    new sbyte[]{ 38, -3 },
	    new sbyte[]{ 37, 3 },   new sbyte[]{ 38, 5 },   new sbyte[]{ 42, 0 },     new sbyte[]{ 35, 16 },
	    new sbyte[]{ 39, 22 },  new sbyte[]{ 14, 48 },  new sbyte[]{ 27, 37 },    new sbyte[]{ 21, 60 },
	    new sbyte[]{ 12, 68 },  new sbyte[]{ 2, 97 },							 
	    new sbyte[]{ -3, 71 },  new sbyte[]{ -6, 42 },  new sbyte[]{ -5, 50 },    new sbyte[]{ -3, 54 }, // 227 -> 251
	    new sbyte[]{ -2, 62 },  new sbyte[]{ 0, 58 },   new sbyte[]{ 1, 63 },     new sbyte[]{ -2, 72 },
	    new sbyte[]{ -1, 74 },  new sbyte[]{ -9, 91 },  new sbyte[]{ -5, 67 },    new sbyte[]{ -5, 27 },
	    new sbyte[]{ -3, 39 },  new sbyte[]{ -2, 44 },  new sbyte[]{ 0, 46 },     new sbyte[]{ -16, 64 },
	    new sbyte[]{ -8, 68 },  new sbyte[]{ -10, 78 }, new sbyte[]{ -6, 77 },    new sbyte[]{ -10, 86 },
	    new sbyte[]{ -12, 92 }, new sbyte[]{ -15, 55 }, new sbyte[]{ -10, 60 },   new sbyte[]{ -6, 62 },
	    new sbyte[]{ -4, 65 },													
	    new sbyte[]{ -12, 73 }, new sbyte[]{ -8, 76 },  new sbyte[]{ -7, 80 },    new sbyte[]{ -9, 88 }, // 252 -> 275
	    new sbyte[]{ -17, 110 },new sbyte[]{ -11, 97 }, new sbyte[]{ -20, 84 },   new sbyte[]{ -11, 79 },
	    new sbyte[]{ -6, 73 },  new sbyte[]{ -4, 74 },  new sbyte[]{ -13, 86 },   new sbyte[]{ -13, 96 },
	    new sbyte[]{ -11, 97 }, new sbyte[]{ -19, 117 },new sbyte[]{ -8, 78 },    new sbyte[]{ -5, 33 },
	    new sbyte[]{ -4, 48 },  new sbyte[]{ -2, 53 },  new sbyte[]{ -3, 62 },    new sbyte[]{ -13, 71 },
	    new sbyte[]{ -10, 79 }, new sbyte[]{ -12, 86 }, new sbyte[]{ -13, 90 },   new sbyte[]{ -14, 97 },
	    new sbyte[]{ 0, 0 }, // 276 a bit special (not used, bypass is used instead)
	    new sbyte[]{ -6, 93 },  new sbyte[]{ -6, 84 },  new sbyte[]{ -8, 79 },    new sbyte[]{ 0, 66 }, // 277 -> 307
	    new sbyte[]{ -1, 71 },  new sbyte[]{ 0, 62 },   new sbyte[]{ -2, 60 },    new sbyte[]{ -2, 59 },
	    new sbyte[]{ -5, 75 },  new sbyte[]{ -3, 62 },  new sbyte[]{ -4, 58 },    new sbyte[]{ -9, 66 },
	    new sbyte[]{ -1, 79 },  new sbyte[]{ 0, 71 },   new sbyte[]{ 3, 68 },     new sbyte[]{ 10, 44 },
	    new sbyte[]{ -7, 62 },  new sbyte[]{ 15, 36 },  new sbyte[]{ 14, 40 },    new sbyte[]{ 16, 27 },
	    new sbyte[]{ 12, 29 },  new sbyte[]{ 1, 44 },   new sbyte[]{ 20, 36 },    new sbyte[]{ 18, 32 },
	    new sbyte[]{ 5, 42 },   new sbyte[]{ 1, 48 },   new sbyte[]{ 10, 62 },    new sbyte[]{ 17, 46 },
	    new sbyte[]{ 9, 64 },   new sbyte[]{ -12, 104 },new sbyte[]{ -11, 97 },	  
	    new sbyte[]{ -16, 96 }, new sbyte[]{ -7, 88 },  new sbyte[]{ -8, 85 },    new sbyte[]{ -7, 85 }, // 308 -> 337
	    new sbyte[]{ -9, 85 },  new sbyte[]{ -13, 88 }, new sbyte[]{ 4, 66 },     new sbyte[]{ -3, 77 },
	    new sbyte[]{ -3, 76 },  new sbyte[]{ -6, 76 },  new sbyte[]{ 10, 58 },    new sbyte[]{ -1, 76 },
	    new sbyte[]{ -1, 83 },  new sbyte[]{ -7, 99 },  new sbyte[]{ -14, 95 },   new sbyte[]{ 2, 95 },
	    new sbyte[]{ 0, 76 },   new sbyte[]{ -5, 74 },  new sbyte[]{ 0, 70 },     new sbyte[]{ -11, 75 },
	    new sbyte[]{ 1, 68 },   new sbyte[]{ 0, 65 },   new sbyte[]{ -14, 73 },   new sbyte[]{ 3, 62 },
	    new sbyte[]{ 4, 62 },   new sbyte[]{ -1, 68 },  new sbyte[]{ -13, 75 },   new sbyte[]{ 11, 55 },
	    new sbyte[]{ 5, 64 },   new sbyte[]{ 12, 70 },							 
	    new sbyte[]{ 15, 6 },   new sbyte[]{ 6, 19 },   new sbyte[]{ 7, 16 },     new sbyte[]{ 12, 14 }, // 338 -> 368
	    new sbyte[]{ 18, 13 },  new sbyte[]{ 13, 11 },  new sbyte[]{ 13, 15 },    new sbyte[]{ 15, 16 },
	    new sbyte[]{ 12, 23 },  new sbyte[]{ 13, 23 },  new sbyte[]{ 15, 20 },    new sbyte[]{ 14, 26 },
	    new sbyte[]{ 14, 44 },  new sbyte[]{ 17, 40 },  new sbyte[]{ 17, 47 },    new sbyte[]{ 24, 17 },
	    new sbyte[]{ 21, 21 },  new sbyte[]{ 25, 22 },  new sbyte[]{ 31, 27 },    new sbyte[]{ 22, 29 },
	    new sbyte[]{ 19, 35 },  new sbyte[]{ 14, 50 },  new sbyte[]{ 10, 57 },    new sbyte[]{ 7, 63 },
	    new sbyte[]{ -2, 77 },  new sbyte[]{ -4, 82 },  new sbyte[]{ -3, 94 },    new sbyte[]{ 9, 69 },
	    new sbyte[]{ -12, 109 },new sbyte[]{ 36, -35 }, new sbyte[]{ 36, -34 },	  
	    new sbyte[]{ 32, -26 }, new sbyte[]{ 37, -30 }, new sbyte[]{ 44, -32 },   new sbyte[]{ 34, -18 }, // 369 -> 398
	    new sbyte[]{ 34, -15 }, new sbyte[]{ 40, -15 }, new sbyte[]{ 33, -7 },    new sbyte[]{ 35, -5 },
	    new sbyte[]{ 33, 0 },   new sbyte[]{ 38, 2 },   new sbyte[]{ 33, 13 },    new sbyte[]{ 23, 35 },
	    new sbyte[]{ 13, 58 },  new sbyte[]{ 29, -3 },  new sbyte[]{ 26, 0 },     new sbyte[]{ 22, 30 },
	    new sbyte[]{ 31, -7 },  new sbyte[]{ 35, -15 }, new sbyte[]{ 34, -3 },    new sbyte[]{ 34, 3 },
	    new sbyte[]{ 36, -1 },  new sbyte[]{ 34, 5 },   new sbyte[]{ 32, 11 },    new sbyte[]{ 35, 5 },
	    new sbyte[]{ 34, 12 },  new sbyte[]{ 39, 11 },  new sbyte[]{ 30, 29 },    new sbyte[]{ 34, 26 },
	    new sbyte[]{ 29, 39 },  new sbyte[]{ 19, 66 },
	    new sbyte[]{  31,  21 }, new sbyte[]{  31,  31 }, new sbyte[]{  25,  50 }, // 399 -> 435
	    new sbyte[]{ -17, 120 }, new sbyte[]{ -20, 112 }, new sbyte[]{ -18, 114 }, new sbyte[]{ -11,  85 },
	    new sbyte[]{ -15,  92 }, new sbyte[]{ -14,  89 }, new sbyte[]{ -26,  71 }, new sbyte[]{ -15,  81 },
	    new sbyte[]{ -14,  80 }, new sbyte[]{   0,  68 }, new sbyte[]{ -14,  70 }, new sbyte[]{ -24,  56 },
	    new sbyte[]{ -23,  68 }, new sbyte[]{ -24,  50 }, new sbyte[]{ -11,  74 }, new sbyte[]{  23, -13 },
	    new sbyte[]{  26, -13 }, new sbyte[]{  40, -15 }, new sbyte[]{  49, -14 }, new sbyte[]{  44,   3 },
	    new sbyte[]{  45,   6 }, new sbyte[]{  44,  34 }, new sbyte[]{  33,  54 }, new sbyte[]{  19,  82 },
	    new sbyte[]{  -3,  75 }, new sbyte[]{  -1,  23 }, new sbyte[]{   1,  34 }, new sbyte[]{   1,  43 },
	    new sbyte[]{   0,  54 }, new sbyte[]{  -2,  55 }, new sbyte[]{   0,  61 }, new sbyte[]{   1,  64 },
	    new sbyte[]{   0,  68 }, new sbyte[]{  -9,  92 },						   
	    new sbyte[]{ -14, 106 }, new sbyte[]{ -13,  97 }, new sbyte[]{ -15,  90 }, new sbyte[]{ -12,  90 }, // 436 -> 459
	    new sbyte[]{ -18,  88 }, new sbyte[]{ -10,  73 }, new sbyte[]{  -9,  79 }, new sbyte[]{ -14,  86 },
	    new sbyte[]{ -10,  73 }, new sbyte[]{ -10,  70 }, new sbyte[]{ -10,  69 }, new sbyte[]{  -5,  66 },
	    new sbyte[]{  -9,  64 }, new sbyte[]{  -5,  58 }, new sbyte[]{   2,  59 }, new sbyte[]{  21, -10 },
	    new sbyte[]{  24, -11 }, new sbyte[]{  28,  -8 }, new sbyte[]{  28,  -1 }, new sbyte[]{  29,   3 },
	    new sbyte[]{  29,   9 }, new sbyte[]{  35,  20 }, new sbyte[]{  29,  36 }, new sbyte[]{  14,  67 }
	};

		public const sbyte[][][] cabac_context_init_PB = new sbyte[][][]
	{
	    // i_cabac_init_idc == 0
	    new sbyte[][] {
	        new sbyte[]{  20, -15 }, new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{  20, -15 }, // 0 - 10
	        new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{ -28, 127 }, new sbyte[]{ -23, 104 },
	        new sbyte[]{  -6,  53 }, new sbyte[]{  -1,  54 }, new sbyte[]{   7,  51 },
	        new sbyte[]{  23,  33 }, new sbyte[]{  23,   2 }, new sbyte[]{  21,   0 }, new sbyte[]{   1,   9 }, // 11 - 23
	        new sbyte[]{   0,  49 }, new sbyte[]{ -37, 118 }, new sbyte[]{   5,  57 }, new sbyte[]{ -13,  78 },
	        new sbyte[]{ -11,  65 }, new sbyte[]{   1,  62 }, new sbyte[]{  12,  49 }, new sbyte[]{  -4,  73 },
	        new sbyte[]{  17,  50 }, 												
	        new sbyte[]{  18,  64 }, new sbyte[]{   9,  43 }, new sbyte[]{  29,   0 }, new sbyte[]{  26,  67 }, // 24 - 39
	        new sbyte[]{  16,  90 }, new sbyte[]{   9, 104 }, new sbyte[]{ -46, 127 }, new sbyte[]{ -20, 104 },
	        new sbyte[]{   1,  67 }, new sbyte[]{ -13,  78 }, new sbyte[]{ -11,  65 }, new sbyte[]{   1,  62 },
	        new sbyte[]{  -6,  86 }, new sbyte[]{ -17,  95 }, new sbyte[]{  -6,  61 }, new sbyte[]{   9,  45 },
	        new sbyte[]{  -3,  69 }, new sbyte[]{  -6,  81 }, new sbyte[]{ -11,  96 }, new sbyte[]{   6,  55 }, // 40 - 53
	        new sbyte[]{   7,  67 }, new sbyte[]{  -5,  86 }, new sbyte[]{   2,  88 }, new sbyte[]{   0,  58 },
	        new sbyte[]{  -3,  76 }, new sbyte[]{ -10,  94 }, new sbyte[]{   5,  54 }, new sbyte[]{   4,  69 },
	        new sbyte[]{  -3,  81 }, new sbyte[]{   0,  88 }, 						
	        new sbyte[]{  -7,  67 }, new sbyte[]{  -5,  74 }, new sbyte[]{  -4,  74 }, new sbyte[]{  -5,  80 }, // 54 - 59
	        new sbyte[]{  -7,  72 }, new sbyte[]{   1,  58 }, 				
	        new sbyte[]{   0,  41 }, new sbyte[]{   0,  63 }, new sbyte[]{   0,  63 }, new sbyte[]{ 0, 63 }, // 60 - 69
	        new sbyte[]{  -9,  83 }, new sbyte[]{   4,  86 }, new sbyte[]{   0,  97 }, new sbyte[]{ -7, 72 },
	        new sbyte[]{  13,  41 }, new sbyte[]{   3,  62 }, 						
	        new sbyte[]{   0,  45 }, new sbyte[]{  -4,  78 }, new sbyte[]{  -3,  96 }, new sbyte[]{ -27,  126 }, // 70 - 87
	        new sbyte[]{ -28,  98 }, new sbyte[]{ -25, 101 }, new sbyte[]{ -23,  67 }, new sbyte[]{ -28,  82 },
	        new sbyte[]{ -20,  94 }, new sbyte[]{ -16,  83 }, new sbyte[]{ -22, 110 }, new sbyte[]{ -21,  91 },
	        new sbyte[]{ -18, 102 }, new sbyte[]{ -13,  93 }, new sbyte[]{ -29, 127 }, new sbyte[]{  -7,  92 },
	        new sbyte[]{  -5,  89 }, new sbyte[]{  -7,  96 }, new sbyte[]{ -13, 108 }, new sbyte[]{  -3,  46 },
	        new sbyte[]{  -1,  65 }, new sbyte[]{  -1,  57 }, new sbyte[]{  -9,  93 }, new sbyte[]{  -3,  74 },
	        new sbyte[]{  -9,  92 }, new sbyte[]{  -8,  87 }, new sbyte[]{ -23, 126 }, new sbyte[]{   5,  54 },
	        new sbyte[]{   6,  60 }, new sbyte[]{   6,  59 }, new sbyte[]{   6,  69 }, new sbyte[]{  -1,  48 },
	        new sbyte[]{   0,  68 }, new sbyte[]{  -4,  69 }, new sbyte[]{  -8,  88 }, 
	        new sbyte[]{  -2,  85 }, new sbyte[]{  -6,  78 }, new sbyte[]{  -1,  75 }, new sbyte[]{  -7,  77 }, // 105 -> 165
	        new sbyte[]{   2,  54 }, new sbyte[]{   5,  50 }, new sbyte[]{  -3,  68 }, new sbyte[]{   1,  50 },
	        new sbyte[]{   6,  42 }, new sbyte[]{  -4,  81 }, new sbyte[]{   1,  63 }, new sbyte[]{  -4,  70 },
	        new sbyte[]{   0,  67 }, new sbyte[]{   2,  57 }, new sbyte[]{  -2,  76 }, new sbyte[]{  11,  35 },
	        new sbyte[]{   4,  64 }, new sbyte[]{   1,  61 }, new sbyte[]{  11,  35 }, new sbyte[]{  18,  25 },
	        new sbyte[]{  12,  24 }, new sbyte[]{  13,  29 }, new sbyte[]{  13,  36 }, new sbyte[]{ -10,  93 },
	        new sbyte[]{  -7,  73 }, new sbyte[]{  -2,  73 }, new sbyte[]{  13,  46 }, new sbyte[]{   9,  49 },
	        new sbyte[]{  -7, 100 }, new sbyte[]{   9,  53 }, new sbyte[]{   2,  53 }, new sbyte[]{   5,  53 },
	        new sbyte[]{  -2,  61 }, new sbyte[]{   0,  56 }, new sbyte[]{   0,  56 }, new sbyte[]{ -13,  63 },
	        new sbyte[]{  -5,  60 }, new sbyte[]{  -1,  62 }, new sbyte[]{   4,  57 }, new sbyte[]{  -6,  69 },
	        new sbyte[]{   4,  57 }, new sbyte[]{  14,  39 }, new sbyte[]{   4,  51 }, new sbyte[]{  13,  68 },
	        new sbyte[]{   3,  64 }, new sbyte[]{   1,  61 }, new sbyte[]{   9,  63 }, new sbyte[]{   7,  50 },
	        new sbyte[]{  16,  39 }, new sbyte[]{   5,  44 }, new sbyte[]{   4,  52 }, new sbyte[]{  11,  48 },
	        new sbyte[]{  -5,  60 }, new sbyte[]{  -1,  59 }, new sbyte[]{   0,  59 }, new sbyte[]{  22,  33 },
	        new sbyte[]{   5,  44 }, new sbyte[]{  14,  43 }, new sbyte[]{  -1,  78 }, new sbyte[]{   0,  60 },
	        new sbyte[]{   9,  69 }, 												
	        new sbyte[]{  11,  28 }, new sbyte[]{   2,  40 }, new sbyte[]{   3,  44 }, new sbyte[]{   0,  49 }, // 166 - 226
	        new sbyte[]{   0,  46 }, new sbyte[]{   2,  44 }, new sbyte[]{   2,  51 }, new sbyte[]{   0,  47 },
	        new sbyte[]{   4,  39 }, new sbyte[]{   2,  62 }, new sbyte[]{   6,  46 }, new sbyte[]{   0,  54 },
	        new sbyte[]{   3,  54 }, new sbyte[]{   2,  58 }, new sbyte[]{   4,  63 }, new sbyte[]{   6,  51 },
	        new sbyte[]{   6,  57 }, new sbyte[]{   7,  53 }, new sbyte[]{   6,  52 }, new sbyte[]{   6,  55 },
	        new sbyte[]{  11,  45 }, new sbyte[]{  14,  36 }, new sbyte[]{   8,  53 }, new sbyte[]{  -1,  82 },
	        new sbyte[]{   7,  55 }, new sbyte[]{  -3,  78 }, new sbyte[]{  15,  46 }, new sbyte[]{  22,  31 },
	        new sbyte[]{  -1,  84 }, new sbyte[]{  25,   7 }, new sbyte[]{  30,  -7 }, new sbyte[]{  28,   3 },
	        new sbyte[]{  28,   4 }, new sbyte[]{  32,   0 }, new sbyte[]{  34,  -1 }, new sbyte[]{  30,   6 },
	        new sbyte[]{  30,   6 }, new sbyte[]{  32,   9 }, new sbyte[]{  31,  19 }, new sbyte[]{  26,  27 },
	        new sbyte[]{  26,  30 }, new sbyte[]{  37,  20 }, new sbyte[]{  28,  34 }, new sbyte[]{  17,  70 },
	        new sbyte[]{   1,  67 }, new sbyte[]{   5,  59 }, new sbyte[]{   9,  67 }, new sbyte[]{  16,  30 },
	        new sbyte[]{  18,  32 }, new sbyte[]{  18,  35 }, new sbyte[]{  22,  29 }, new sbyte[]{  24,  31 },
	        new sbyte[]{  23,  38 }, new sbyte[]{  18,  43 }, new sbyte[]{  20,  41 }, new sbyte[]{  11,  63 },
	        new sbyte[]{   9,  59 }, new sbyte[]{   9,  64 }, new sbyte[]{  -1,  94 }, new sbyte[]{  -2,  89 },
	        new sbyte[]{  -9, 108 }, 											
	        new sbyte[]{  -6,  76 }, new sbyte[]{  -2,  44 }, new sbyte[]{   0,  45 }, new sbyte[]{   0,  52 }, // 227 - 275
	        new sbyte[]{  -3,  64 }, new sbyte[]{  -2,  59 }, new sbyte[]{  -4,  70 }, new sbyte[]{  -4,  75 },
	        new sbyte[]{  -8,  82 }, new sbyte[]{ -17, 102 }, new sbyte[]{  -9,  77 }, new sbyte[]{   3,  24 },
	        new sbyte[]{   0,  42 }, new sbyte[]{   0,  48 }, new sbyte[]{   0,  55 }, new sbyte[]{  -6,  59 },
	        new sbyte[]{  -7,  71 }, new sbyte[]{ -12,  83 }, new sbyte[]{ -11,  87 }, new sbyte[]{ -30, 119 },
	        new sbyte[]{   1,  58 }, new sbyte[]{  -3,  29 }, new sbyte[]{  -1,  36 }, new sbyte[]{   1,  38 },
	        new sbyte[]{   2,  43 }, new sbyte[]{  -6,  55 }, new sbyte[]{   0,  58 }, new sbyte[]{   0,  64 },
	        new sbyte[]{  -3,  74 }, new sbyte[]{ -10,  90 }, new sbyte[]{   0,  70 }, new sbyte[]{  -4,  29 },
	        new sbyte[]{   5,  31 }, new sbyte[]{   7,  42 }, new sbyte[]{   1,  59 }, new sbyte[]{  -2,  58 },
	        new sbyte[]{  -3,  72 }, new sbyte[]{  -3,  81 }, new sbyte[]{ -11,  97 }, new sbyte[]{   0,  58 },
	        new sbyte[]{   8,   5 }, new sbyte[]{  10,  14 }, new sbyte[]{  14,  18 }, new sbyte[]{  13,  27 },
	        new sbyte[]{   2,  40 }, new sbyte[]{   0,  58 }, new sbyte[]{  -3,  70 }, new sbyte[]{  -6,  79 },
	        new sbyte[]{  -8,  85 },            								
	        new sbyte[]{ 0, 0 },                                                                                // 276 a bit special (not used, bypass is used instead)
	        new sbyte[]{ -13, 106 }, new sbyte[]{ -16, 106 }, new sbyte[]{ -10,  87 }, new sbyte[]{ -21, 114 }, // 277 - 337
	        new sbyte[]{ -18, 110 }, new sbyte[]{ -14,  98 }, new sbyte[]{ -22, 110 }, new sbyte[]{ -21, 106 },
	        new sbyte[]{ -18, 103 }, new sbyte[]{ -21, 107 }, new sbyte[]{ -23, 108 }, new sbyte[]{ -26, 112 },
	        new sbyte[]{ -10,  96 }, new sbyte[]{ -12,  95 }, new sbyte[]{  -5,  91 }, new sbyte[]{  -9,  93 },
	        new sbyte[]{ -22,  94 }, new sbyte[]{  -5,  86 }, new sbyte[]{   9,  67 }, new sbyte[]{  -4,  80 },
	        new sbyte[]{ -10,  85 }, new sbyte[]{  -1,  70 }, new sbyte[]{   7,  60 }, new sbyte[]{   9,  58 },
	        new sbyte[]{   5,  61 }, new sbyte[]{  12,  50 }, new sbyte[]{  15,  50 }, new sbyte[]{  18,  49 },
	        new sbyte[]{  17,  54 }, new sbyte[]{  10,  41 }, new sbyte[]{   7,  46 }, new sbyte[]{  -1,  51 },
	        new sbyte[]{   7,  49 }, new sbyte[]{   8,  52 }, new sbyte[]{   9,  41 }, new sbyte[]{   6,  47 },
	        new sbyte[]{   2,  55 }, new sbyte[]{  13,  41 }, new sbyte[]{  10,  44 }, new sbyte[]{   6,  50 },
	        new sbyte[]{   5,  53 }, new sbyte[]{  13,  49 }, new sbyte[]{   4,  63 }, new sbyte[]{   6,  64 },
	        new sbyte[]{  -2,  69 }, new sbyte[]{  -2,  59 }, new sbyte[]{   6,  70 }, new sbyte[]{  10,  44 },
	        new sbyte[]{   9,  31 }, new sbyte[]{  12,  43 }, new sbyte[]{   3,  53 }, new sbyte[]{  14,  34 },
	        new sbyte[]{  10,  38 }, new sbyte[]{  -3,  52 }, new sbyte[]{  13,  40 }, new sbyte[]{  17,  32 },
	        new sbyte[]{   7,  44 }, new sbyte[]{   7,  38 }, new sbyte[]{  13,  50 }, new sbyte[]{  10,  57 },
	        new sbyte[]{  26,  43 }, 						 						
	        new sbyte[]{  14,  11 }, new sbyte[]{  11,  14 }, new sbyte[]{   9,  11 }, new sbyte[]{  18,  11 }, // 338 - 398
	        new sbyte[]{  21,   9 }, new sbyte[]{  23,  -2 }, new sbyte[]{  32, -15 }, new sbyte[]{  32, -15 },
	        new sbyte[]{  34, -21 }, new sbyte[]{  39, -23 }, new sbyte[]{  42, -33 }, new sbyte[]{  41, -31 },
	        new sbyte[]{  46, -28 }, new sbyte[]{  38, -12 }, new sbyte[]{  21,  29 }, new sbyte[]{  45, -24 },
	        new sbyte[]{  53, -45 }, new sbyte[]{  48, -26 }, new sbyte[]{  65, -43 }, new sbyte[]{  43, -19 },
	        new sbyte[]{  39, -10 }, new sbyte[]{  30,   9 }, new sbyte[]{  18,  26 }, new sbyte[]{  20,  27 },
	        new sbyte[]{   0,  57 }, new sbyte[]{ -14,  82 }, new sbyte[]{  -5,  75 }, new sbyte[]{ -19,  97 },
	        new sbyte[]{ -35, 125 }, new sbyte[]{  27,   0 }, new sbyte[]{  28,   0 }, new sbyte[]{  31,  -4 },
	        new sbyte[]{  27,   6 }, new sbyte[]{  34,   8 }, new sbyte[]{  30,  10 }, new sbyte[]{  24,  22 },
	        new sbyte[]{  33,  19 }, new sbyte[]{  22,  32 }, new sbyte[]{  26,  31 }, new sbyte[]{  21,  41 },
	        new sbyte[]{  26,  44 }, new sbyte[]{  23,  47 }, new sbyte[]{  16,  65 }, new sbyte[]{  14,  71 },
	        new sbyte[]{   8,  60 }, new sbyte[]{   6,  63 }, new sbyte[]{  17,  65 }, new sbyte[]{  21,  24 },
	        new sbyte[]{  23,  20 }, new sbyte[]{  26,  23 }, new sbyte[]{  27,  32 }, new sbyte[]{  28,  23 },
	        new sbyte[]{  28,  24 }, new sbyte[]{  23,  40 }, new sbyte[]{  24,  32 }, new sbyte[]{  28,  29 },
	        new sbyte[]{  23,  42 }, new sbyte[]{  19,  57 }, new sbyte[]{  22,  53 }, new sbyte[]{  22,  61 },
	        new sbyte[]{  11,  86 }, 						  						  
	        new sbyte[]{  12,  40 }, new sbyte[]{  11,  51 }, new sbyte[]{  14,  59 },         　　　　　　　　　 // 399 - 435
	        new sbyte[]{  -4,  79 }, new sbyte[]{  -7,  71 }, new sbyte[]{  -5,  69 }, new sbyte[]{  -9,  70 },
	        new sbyte[]{  -8,  66 }, new sbyte[]{ -10,  68 }, new sbyte[]{ -19,  73 }, new sbyte[]{ -12,  69 },
	        new sbyte[]{ -16,  70 }, new sbyte[]{ -15,  67 }, new sbyte[]{ -20,  62 }, new sbyte[]{ -19,  70 },
	        new sbyte[]{ -16,  66 }, new sbyte[]{ -22,  65 }, new sbyte[]{ -20,  63 }, new sbyte[]{   9,  -2 },
	        new sbyte[]{  26,  -9 }, new sbyte[]{  33,  -9 }, new sbyte[]{  39,  -7 }, new sbyte[]{  41,  -2 },
	        new sbyte[]{  45,   3 }, new sbyte[]{  49,   9 }, new sbyte[]{  45,  27 }, new sbyte[]{  36,  59 },
	        new sbyte[]{  -6,  66 }, new sbyte[]{  -7,  35 }, new sbyte[]{  -7,  42 }, new sbyte[]{  -8,  45 },
	        new sbyte[]{  -5,  48 }, new sbyte[]{ -12,  56 }, new sbyte[]{  -6,  60 }, new sbyte[]{  -5,  62 },
	        new sbyte[]{  -8,  66 }, new sbyte[]{  -8,  76 }, 
	        new sbyte[]{  -5,  85 }, new sbyte[]{  -6,  81 }, new sbyte[]{ -10,  77 }, new sbyte[]{  -7,  81 }, // 436 - 459
	        new sbyte[]{ -17,  80 }, new sbyte[]{ -18,  73 }, new sbyte[]{  -4,  74 }, new sbyte[]{ -10,  83 },
	        new sbyte[]{  -9,  71 }, new sbyte[]{  -9,  67 }, new sbyte[]{  -1,  61 }, new sbyte[]{  -8,  66 },
	        new sbyte[]{ -14,  66 }, new sbyte[]{   0,  59 }, new sbyte[]{   2,  59 }, new sbyte[]{  21, -13 },
	        new sbyte[]{  33, -14 }, new sbyte[]{  39,  -7 }, new sbyte[]{  46,  -2 }, new sbyte[]{  51,   2 },
	        new sbyte[]{  60,   6 }, new sbyte[]{  61,  17 }, new sbyte[]{  55,  34 }, new sbyte[]{  42,  62 },
	    },

	    /* i_cabac_init_idc == 1 */
	    new sbyte[][] {
	        new sbyte[]{  20, -15 }, new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{  20, -15 }, // 0 - 10
	        new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{ -28, 127 }, new sbyte[]{ -23, 104 },
	        new sbyte[]{  -6,  53 }, new sbyte[]{  -1,  54 }, new sbyte[]{   7,  51 },
	        new sbyte[]{  22,  25 }, new sbyte[]{  34,   0 }, new sbyte[]{  16,   0 }, new sbyte[]{  -2,   9 }, // 11 - 23
	        new sbyte[]{   4,  41 }, new sbyte[]{ -29, 118 }, new sbyte[]{   2,  65 }, new sbyte[]{  -6,  71 },
	        new sbyte[]{ -13,  79 }, new sbyte[]{   5,  52 }, new sbyte[]{   9,  50 }, new sbyte[]{  -3,  70 },
	        new sbyte[]{  10,  54 },
	        new sbyte[]{  26,  34 }, new sbyte[]{  19,  22 }, new sbyte[]{  40,   0 }, new sbyte[]{  57,   2 }, // 24 - 39
	        new sbyte[]{  41,  36 }, new sbyte[]{  26,  69 }, new sbyte[]{ -45, 127 }, new sbyte[]{ -15, 101 },
	        new sbyte[]{  -4,  76 }, new sbyte[]{  -6,  71 }, new sbyte[]{ -13,  79 }, new sbyte[]{   5,  52 },
	        new sbyte[]{   6,  69 }, new sbyte[]{ -13,  90 }, new sbyte[]{   0,  52 }, new sbyte[]{   8,  43 },
	        new sbyte[]{  -2,  69 }, new sbyte[]{  -5,  82 }, new sbyte[]{ -10,  96 }, new sbyte[]{   2,  59 }, // 40 - 53
	        new sbyte[]{   2,  75 }, new sbyte[]{  -3,  87 }, new sbyte[]{  -3,  100}, new sbyte[]{   1,  56 },
	        new sbyte[]{  -3,  74 }, new sbyte[]{  -6,  85 }, new sbyte[]{   0,  59 }, new sbyte[]{  -3,  81 },
	        new sbyte[]{  -7,  86 }, new sbyte[]{  -5,  95 },
	        new sbyte[]{  -1,  66 }, new sbyte[]{  -1,  77 }, new sbyte[]{   1,  70 }, new sbyte[]{  -2,  86 }, // 54 - 59
	        new sbyte[]{  -5,  72 }, new sbyte[]{   0,  61 },
	        new sbyte[]{ 0, 41 },    new sbyte[]{   0,  63 }, new sbyte[]{   0,  63 }, new sbyte[]{ 0, 63 }, // 60 - 69
	        new sbyte[]{ -9, 83 },   new sbyte[]{   4,  86 }, new sbyte[]{   0,  97 }, new sbyte[]{ -7, 72 },
	        new sbyte[]{ 13, 41 },   new sbyte[]{   3,  62 },
	        new sbyte[]{  13,  15 }, new sbyte[]{   7,  51 }, new sbyte[]{   2,  80 }, new sbyte[]{ -39, 127 }, // 70 - 104
	        new sbyte[]{ -18,  91 }, new sbyte[]{ -17,  96 }, new sbyte[]{ -26,  81 }, new sbyte[]{ -35,  98 },
	        new sbyte[]{ -24, 102 }, new sbyte[]{ -23,  97 }, new sbyte[]{ -27, 119 }, new sbyte[]{ -24,  99 },
	        new sbyte[]{ -21, 110 }, new sbyte[]{ -18, 102 }, new sbyte[]{ -36, 127 }, new sbyte[]{   0,  80 },
	        new sbyte[]{  -5,  89 }, new sbyte[]{  -7,  94 }, new sbyte[]{  -4,  92 }, new sbyte[]{   0,  39 },
	        new sbyte[]{   0,  65 }, new sbyte[]{ -15,  84 }, new sbyte[]{ -35, 127 }, new sbyte[]{  -2,  73 },
	        new sbyte[]{ -12, 104 }, new sbyte[]{  -9,  91 }, new sbyte[]{ -31, 127 }, new sbyte[]{   3,  55 },
	        new sbyte[]{   7,  56 }, new sbyte[]{   7,  55 }, new sbyte[]{   8,  61 }, new sbyte[]{  -3,  53 },
	        new sbyte[]{   0,  68 }, new sbyte[]{  -7,  74 }, new sbyte[]{  -9,  88 },
	        new sbyte[]{ -13, 103 }, new sbyte[]{ -13,  91 }, new sbyte[]{  -9,  89 }, new sbyte[]{ -14,  92 }, // 105 -> 165
	        new sbyte[]{  -8,  76 }, new sbyte[]{ -12,  87 }, new sbyte[]{ -23, 110 }, new sbyte[]{ -24, 105 },
	        new sbyte[]{ -10,  78 }, new sbyte[]{ -20, 112 }, new sbyte[]{ -17,  99 }, new sbyte[]{ -78, 127 },
	        new sbyte[]{ -70, 127 }, new sbyte[]{ -50, 127 }, new sbyte[]{ -46, 127 }, new sbyte[]{  -4,  66 },
	        new sbyte[]{  -5,  78 }, new sbyte[]{  -4,  71 }, new sbyte[]{  -8,  72 }, new sbyte[]{   2,  59 },
	        new sbyte[]{  -1,  55 }, new sbyte[]{  -7,  70 }, new sbyte[]{  -6,  75 }, new sbyte[]{  -8,  89 },
	        new sbyte[]{ -34, 119 }, new sbyte[]{  -3,  75 }, new sbyte[]{  32,  20 }, new sbyte[]{  30,  22 },
	        new sbyte[]{ -44, 127 }, new sbyte[]{   0,  54 }, new sbyte[]{  -5,  61 }, new sbyte[]{   0,  58 },
	        new sbyte[]{  -1,  60 }, new sbyte[]{  -3,  61 }, new sbyte[]{  -8,  67 }, new sbyte[]{ -25,  84 },
	        new sbyte[]{ -14,  74 }, new sbyte[]{  -5,  65 }, new sbyte[]{   5,  52 }, new sbyte[]{   2,  57 },
	        new sbyte[]{   0,  61 }, new sbyte[]{  -9,  69 }, new sbyte[]{ -11,  70 }, new sbyte[]{  18,  55 },
	        new sbyte[]{  -4,  71 }, new sbyte[]{   0,  58 }, new sbyte[]{   7,  61 }, new sbyte[]{   9,  41 },
	        new sbyte[]{  18,  25 }, new sbyte[]{   9,  32 }, new sbyte[]{   5,  43 }, new sbyte[]{   9,  47 },
	        new sbyte[]{   0,  44 }, new sbyte[]{   0,  51 }, new sbyte[]{   2,  46 }, new sbyte[]{  19,  38 },
	        new sbyte[]{  -4,  66 }, new sbyte[]{  15,  38 }, new sbyte[]{  12,  42 }, new sbyte[]{   9,  34 },
	        new sbyte[]{   0,  89 },
	        new sbyte[]{   4,  45 }, new sbyte[]{  10,  28 }, new sbyte[]{  10,  31 }, new sbyte[]{  33, -11 }, // 166 - 226
	        new sbyte[]{  52, -43 }, new sbyte[]{  18,  15 }, new sbyte[]{  28,   0 }, new sbyte[]{  35, -22 },
	        new sbyte[]{  38, -25 }, new sbyte[]{  34,   0 }, new sbyte[]{  39, -18 }, new sbyte[]{  32, -12 },
	        new sbyte[]{ 102, -94 }, new sbyte[]{   0,   0 }, new sbyte[]{  56, -15 }, new sbyte[]{  33,  -4 },
	        new sbyte[]{  29,  10 }, new sbyte[]{  37,  -5 }, new sbyte[]{  51, -29 }, new sbyte[]{  39,  -9 },
	        new sbyte[]{  52, -34 }, new sbyte[]{  69, -58 }, new sbyte[]{  67, -63 }, new sbyte[]{  44,  -5 },
	        new sbyte[]{  32,   7 }, new sbyte[]{  55, -29 }, new sbyte[]{  32,   1 }, new sbyte[]{   0,   0 },
	        new sbyte[]{  27,  36 }, new sbyte[]{  33, -25 }, new sbyte[]{  34, -30 }, new sbyte[]{  36, -28 },
	        new sbyte[]{  38, -28 }, new sbyte[]{  38, -27 }, new sbyte[]{  34, -18 }, new sbyte[]{  35, -16 },
	        new sbyte[]{  34, -14 }, new sbyte[]{  32,  -8 }, new sbyte[]{  37,  -6 }, new sbyte[]{  35,   0 },
	        new sbyte[]{  30,  10 }, new sbyte[]{  28,  18 }, new sbyte[]{  26,  25 }, new sbyte[]{  29,  41 },
	        new sbyte[]{   0,  75 }, new sbyte[]{   2,  72 }, new sbyte[]{   8,  77 }, new sbyte[]{  14,  35 },
	        new sbyte[]{  18,  31 }, new sbyte[]{  17,  35 }, new sbyte[]{  21,  30 }, new sbyte[]{  17,  45 },
	        new sbyte[]{  20,  42 }, new sbyte[]{  18,  45 }, new sbyte[]{  27,  26 }, new sbyte[]{  16,  54 },
	        new sbyte[]{   7,  66 }, new sbyte[]{  16,  56 }, new sbyte[]{  11,  73 }, new sbyte[]{  10,  67 },
	        new sbyte[]{ -10, 116 },
	        new sbyte[]{ -23, 112 }, new sbyte[]{ -15,  71 }, new sbyte[]{  -7,  61 }, new sbyte[]{   0,  53 }, // 227 - 275
	        new sbyte[]{  -5,  66 }, new sbyte[]{ -11,  77 }, new sbyte[]{  -9,  80 }, new sbyte[]{  -9,  84 },
	        new sbyte[]{ -10,  87 }, new sbyte[]{ -34, 127 }, new sbyte[]{ -21, 101 }, new sbyte[]{  -3,  39 },
	        new sbyte[]{  -5,  53 }, new sbyte[]{  -7,  61 }, new sbyte[]{ -11,  75 }, new sbyte[]{ -15,  77 },
	        new sbyte[]{ -17,  91 }, new sbyte[]{ -25, 107 }, new sbyte[]{ -25, 111 }, new sbyte[]{ -28, 122 },
	        new sbyte[]{ -11,  76 }, new sbyte[]{ -10,  44 }, new sbyte[]{ -10,  52 }, new sbyte[]{ -10,  57 },
	        new sbyte[]{  -9,  58 }, new sbyte[]{ -16,  72 }, new sbyte[]{  -7,  69 }, new sbyte[]{  -4,  69 },
	        new sbyte[]{  -5,  74 }, new sbyte[]{  -9,  86 }, new sbyte[]{   2,  66 }, new sbyte[]{  -9,  34 },
	        new sbyte[]{   1,  32 }, new sbyte[]{  11,  31 }, new sbyte[]{   5,  52 }, new sbyte[]{  -2,  55 },
	        new sbyte[]{  -2,  67 }, new sbyte[]{   0,  73 }, new sbyte[]{  -8,  89 }, new sbyte[]{   3,  52 },
	        new sbyte[]{   7,   4 }, new sbyte[]{  10,   8 }, new sbyte[]{  17,   8 }, new sbyte[]{  16,  19 },
	        new sbyte[]{   3,  37 }, new sbyte[]{  -1,  61 }, new sbyte[]{  -5,  73 }, new sbyte[]{  -1,  70 },
	        new sbyte[]{  -4,  78 }, 
	        new sbyte[]{   0,   0 },                                                                            // 276 a bit special (not used, bypass is used instead)
	        new sbyte[]{ -21, 126 }, new sbyte[]{ -23, 124 }, new sbyte[]{ -20, 110 }, new sbyte[]{ -26, 126 }, // 277 - 337
	        new sbyte[]{ -25, 124 }, new sbyte[]{ -17, 105 }, new sbyte[]{ -27, 121 }, new sbyte[]{ -27, 117 },
	        new sbyte[]{ -17, 102 }, new sbyte[]{ -26, 117 }, new sbyte[]{ -27, 116 }, new sbyte[]{ -33, 122 },
	        new sbyte[]{ -10,  95 }, new sbyte[]{ -14, 100 }, new sbyte[]{  -8,  95 }, new sbyte[]{ -17, 111 },
	        new sbyte[]{ -28, 114 }, new sbyte[]{  -6,  89 }, new sbyte[]{  -2,  80 }, new sbyte[]{  -4,  82 },
	        new sbyte[]{  -9,  85 }, new sbyte[]{  -8,  81 }, new sbyte[]{  -1,  72 }, new sbyte[]{   5,  64 },
	        new sbyte[]{   1,  67 }, new sbyte[]{   9,  56 }, new sbyte[]{   0,  69 }, new sbyte[]{   1,  69 },
	        new sbyte[]{   7,  69 }, new sbyte[]{  -7,  69 }, new sbyte[]{  -6,  67 }, new sbyte[]{ -16,  77 },
	        new sbyte[]{  -2,  64 }, new sbyte[]{   2,  61 }, new sbyte[]{  -6,  67 }, new sbyte[]{  -3,  64 },
	        new sbyte[]{   2,  57 }, new sbyte[]{  -3,  65 }, new sbyte[]{  -3,  66 }, new sbyte[]{   0,  62 },
	        new sbyte[]{   9,  51 }, new sbyte[]{  -1,  66 }, new sbyte[]{  -2,  71 }, new sbyte[]{  -2,  75 },
	        new sbyte[]{  -1,  70 }, new sbyte[]{  -9,  72 }, new sbyte[]{  14,  60 }, new sbyte[]{  16,  37 },
	        new sbyte[]{   0,  47 }, new sbyte[]{  18,  35 }, new sbyte[]{  11,  37 }, new sbyte[]{  12,  41 },
	        new sbyte[]{  10,  41 }, new sbyte[]{   2,  48 }, new sbyte[]{  12,  41 }, new sbyte[]{  13,  41 },
	        new sbyte[]{   0,  59 }, new sbyte[]{   3,  50 }, new sbyte[]{  19,  40 }, new sbyte[]{   3,  66 },
	        new sbyte[]{  18,  50 }, 
	        new sbyte[]{  19,  -6 }, new sbyte[]{  18,  -6 }, new sbyte[]{  14,   0 }, new sbyte[]{  26, -12 }, // 338 - 398
	        new sbyte[]{  31, -16 }, new sbyte[]{  33, -25 }, new sbyte[]{  33, -22 }, new sbyte[]{  37, -28 },
	        new sbyte[]{  39, -30 }, new sbyte[]{  42, -30 }, new sbyte[]{  47, -42 }, new sbyte[]{  45, -36 },
	        new sbyte[]{  49, -34 }, new sbyte[]{  41, -17 }, new sbyte[]{  32,   9 }, new sbyte[]{  69, -71 },
	        new sbyte[]{  63, -63 }, new sbyte[]{  66, -64 }, new sbyte[]{  77, -74 }, new sbyte[]{  54, -39 },
	        new sbyte[]{  52, -35 }, new sbyte[]{  41, -10 }, new sbyte[]{  36,   0 }, new sbyte[]{  40,  -1 },
	        new sbyte[]{  30,  14 }, new sbyte[]{  28,  26 }, new sbyte[]{  23,  37 }, new sbyte[]{  12,  55 },
	        new sbyte[]{  11,  65 }, new sbyte[]{  37, -33 }, new sbyte[]{  39, -36 }, new sbyte[]{  40, -37 },
	        new sbyte[]{  38, -30 }, new sbyte[]{  46, -33 }, new sbyte[]{  42, -30 }, new sbyte[]{  40, -24 },
	        new sbyte[]{  49, -29 }, new sbyte[]{  38, -12 }, new sbyte[]{  40, -10 }, new sbyte[]{  38,  -3 },
	        new sbyte[]{  46,  -5 }, new sbyte[]{  31,  20 }, new sbyte[]{  29,  30 }, new sbyte[]{  25,  44 },
	        new sbyte[]{  12,  48 }, new sbyte[]{  11,  49 }, new sbyte[]{  26,  45 }, new sbyte[]{  22,  22 },
	        new sbyte[]{  23,  22 }, new sbyte[]{  27,  21 }, new sbyte[]{  33,  20 }, new sbyte[]{  26,  28 },
	        new sbyte[]{  30,  24 }, new sbyte[]{  27,  34 }, new sbyte[]{  18,  42 }, new sbyte[]{  25,  39 },
	        new sbyte[]{  18,  50 }, new sbyte[]{  12,  70 }, new sbyte[]{  21,  54 }, new sbyte[]{  14,  71 },
	        new sbyte[]{  11,  83 }, 
	        new sbyte[]{  25,  32 }, new sbyte[]{  21,  49 }, new sbyte[]{  21,  54 },                          // 399 - 435
	        new sbyte[]{  -5,  85 }, new sbyte[]{  -6,  81 }, new sbyte[]{ -10,  77 }, new sbyte[]{  -7,  81 },
	        new sbyte[]{ -17,  80 }, new sbyte[]{ -18,  73 }, new sbyte[]{  -4,  74 }, new sbyte[]{ -10,  83 },
	        new sbyte[]{  -9,  71 }, new sbyte[]{  -9,  67 }, new sbyte[]{  -1,  61 }, new sbyte[]{  -8,  66 },
	        new sbyte[]{ -14,  66 }, new sbyte[]{   0,  59 }, new sbyte[]{   2,  59 }, new sbyte[]{  17, -10 },
	        new sbyte[]{  32, -13 }, new sbyte[]{  42,  -9 }, new sbyte[]{  49,  -5 }, new sbyte[]{  53,   0 },
	        new sbyte[]{  64,   3 }, new sbyte[]{  68,  10 }, new sbyte[]{  66,  27 }, new sbyte[]{  47,  57 },
	        new sbyte[]{  -5,  71 }, new sbyte[]{   0,  24 }, new sbyte[]{  -1,  36 }, new sbyte[]{  -2,  42 },
	        new sbyte[]{  -2,  52 }, new sbyte[]{  -9,  57 }, new sbyte[]{  -6,  63 }, new sbyte[]{  -4,  65 },
	        new sbyte[]{  -4,  67 }, new sbyte[]{  -7,  82 },
	        new sbyte[]{  -3,  81 }, new sbyte[]{  -3,  76 }, new sbyte[]{  -7,  72 }, new sbyte[]{  -6,  78 }, // 436 - 459
	        new sbyte[]{ -12,  72 }, new sbyte[]{ -14,  68 }, new sbyte[]{  -3,  70 }, new sbyte[]{  -6,  76 },
	        new sbyte[]{  -5,  66 }, new sbyte[]{  -5,  62 }, new sbyte[]{   0,  57 }, new sbyte[]{  -4,  61 },
	        new sbyte[]{  -9,  60 }, new sbyte[]{   1,  54 }, new sbyte[]{   2,  58 }, new sbyte[]{  17, -10 },
	        new sbyte[]{  32, -13 }, new sbyte[]{  42,  -9 }, new sbyte[]{  49,  -5 }, new sbyte[]{  53,   0 },
	        new sbyte[]{  64,   3 }, new sbyte[]{  68,  10 }, new sbyte[]{  66,  27 }, new sbyte[]{  47,  57 },
	    },

	    /* i_cabac_init_idc == 2 */
	    new sbyte[][] {
	        new sbyte[]{  20, -15 }, new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{  20, -15 }, // 0 - 10
	        new sbyte[]{   2,  54 }, new sbyte[]{   3,  74 }, new sbyte[]{ -28, 127 }, new sbyte[]{ -23, 104 },
	        new sbyte[]{  -6,  53 }, new sbyte[]{  -1,  54 }, new sbyte[]{   7,  51 },
	        new sbyte[]{  29,  16 }, new sbyte[]{  25,   0 }, new sbyte[]{  14,   0 }, new sbyte[]{ -10,  51 }, // 11 - 23
	        new sbyte[]{  -3,  62 }, new sbyte[]{ -27,  99 }, new sbyte[]{  26,  16 }, new sbyte[]{  -4,  85 },
	        new sbyte[]{ -24, 102 }, new sbyte[]{   5,  57 }, new sbyte[]{   6,  57 }, new sbyte[]{ -17,  73 },
	        new sbyte[]{  14,  57 }, 
	        new sbyte[]{  20,  40 }, new sbyte[]{  20,  10 }, new sbyte[]{  29,   0 }, new sbyte[]{  54,   0 }, // 24 - 39
	        new sbyte[]{  37,  42 }, new sbyte[]{  12,  97 }, new sbyte[]{ -32, 127 }, new sbyte[]{ -22, 117 },
	        new sbyte[]{  -2,  74 }, new sbyte[]{  -4,  85 }, new sbyte[]{ -24, 102 }, new sbyte[]{   5,  57 },
	        new sbyte[]{  -6,  93 }, new sbyte[]{ -14,  88 }, new sbyte[]{  -6,  44 }, new sbyte[]{   4,  55 },
	        new sbyte[]{ -11,  89 }, new sbyte[]{ -15,  103}, new sbyte[]{ -21,  116}, new sbyte[]{  19,  57 }, // 40 - 53
	        new sbyte[]{  20,  58 }, new sbyte[]{   4,  84 }, new sbyte[]{   6,  96 }, new sbyte[]{   1,  63 },
	        new sbyte[]{  -5,  85 }, new sbyte[]{ -13,  106}, new sbyte[]{   5,  63 }, new sbyte[]{   6,  75 },
	        new sbyte[]{  -3,  90 }, new sbyte[]{  -1,  101},
	        new sbyte[]{   3,  55 }, new sbyte[]{  -4,  79 }, new sbyte[]{  -2,  75 }, new sbyte[]{ -12,  97 }, // 54 - 59
	        new sbyte[]{  -7,  50 }, new sbyte[]{   1,  60 },
	        new sbyte[]{ 0, 41 },    new sbyte[]{   0, 63  }, new sbyte[]{ 0, 63    }, new sbyte[]{ 0, 63 }, // 60 - 69
	        new sbyte[]{ -9, 83 },   new sbyte[]{   4, 86  }, new sbyte[]{ 0, 97    }, new sbyte[]{ -7, 72 },
	        new sbyte[]{ 13, 41 },   new sbyte[]{   3, 62  },
	        new sbyte[]{   7,  34 }, new sbyte[]{  -9,  88 }, new sbyte[]{ -20, 127 }, new sbyte[]{ -36, 127 }, // 70 - 104
	        new sbyte[]{ -17,  91 }, new sbyte[]{ -14,  95 }, new sbyte[]{ -25,  84 }, new sbyte[]{ -25,  86 },
	        new sbyte[]{ -12,  89 }, new sbyte[]{ -17,  91 }, new sbyte[]{ -31, 127 }, new sbyte[]{ -14,  76 },
	        new sbyte[]{ -18, 103 }, new sbyte[]{ -13,  90 }, new sbyte[]{ -37, 127 }, new sbyte[]{  11,  80 },
	        new sbyte[]{   5,  76 }, new sbyte[]{   2,  84 }, new sbyte[]{   5,  78 }, new sbyte[]{  -6,  55 },
	        new sbyte[]{   4,  61 }, new sbyte[]{ -14,  83 }, new sbyte[]{ -37, 127 }, new sbyte[]{  -5,  79 },
	        new sbyte[]{ -11, 104 }, new sbyte[]{ -11,  91 }, new sbyte[]{ -30, 127 }, new sbyte[]{   0,  65 },
	        new sbyte[]{  -2,  79 }, new sbyte[]{   0,  72 }, new sbyte[]{  -4,  92 }, new sbyte[]{  -6,  56 },
	        new sbyte[]{   3,  68 }, new sbyte[]{  -8,  71 }, new sbyte[]{ -13,  98 },
	        new sbyte[]{  -4,  86 }, new sbyte[]{ -12,  88 }, new sbyte[]{  -5,  82 }, new sbyte[]{  -3,  72 }, // 105 -> 165
	        new sbyte[]{  -4,  67 }, new sbyte[]{  -8,  72 }, new sbyte[]{ -16,  89 }, new sbyte[]{  -9,  69 },
	        new sbyte[]{  -1,  59 }, new sbyte[]{   5,  66 }, new sbyte[]{   4,  57 }, new sbyte[]{  -4,  71 },
	        new sbyte[]{  -2,  71 }, new sbyte[]{   2,  58 }, new sbyte[]{  -1,  74 }, new sbyte[]{  -4,  44 },
	        new sbyte[]{  -1,  69 }, new sbyte[]{   0,  62 }, new sbyte[]{  -7,  51 }, new sbyte[]{  -4,  47 },
	        new sbyte[]{  -6,  42 }, new sbyte[]{  -3,  41 }, new sbyte[]{  -6,  53 }, new sbyte[]{   8,  76 },
	        new sbyte[]{  -9,  78 }, new sbyte[]{ -11,  83 }, new sbyte[]{   9,  52 }, new sbyte[]{   0,  67 },
	        new sbyte[]{  -5,  90 }, new sbyte[]{   1,  67 }, new sbyte[]{ -15,  72 }, new sbyte[]{  -5,  75 },
	        new sbyte[]{  -8,  80 }, new sbyte[]{ -21,  83 }, new sbyte[]{ -21,  64 }, new sbyte[]{ -13,  31 },
	        new sbyte[]{ -25,  64 }, new sbyte[]{ -29,  94 }, new sbyte[]{   9,  75 }, new sbyte[]{  17,  63 },
	        new sbyte[]{  -8,  74 }, new sbyte[]{  -5,  35 }, new sbyte[]{  -2,  27 }, new sbyte[]{  13,  91 },
	        new sbyte[]{   3,  65 }, new sbyte[]{  -7,  69 }, new sbyte[]{   8,  77 }, new sbyte[]{ -10,  66 },
	        new sbyte[]{   3,  62 }, new sbyte[]{  -3,  68 }, new sbyte[]{ -20,  81 }, new sbyte[]{   0,  30 },
	        new sbyte[]{   1,   7 }, new sbyte[]{  -3,  23 }, new sbyte[]{ -21,  74 }, new sbyte[]{  16,  66 },
	        new sbyte[]{ -23, 124 }, new sbyte[]{  17,  37 }, new sbyte[]{  44, -18 }, new sbyte[]{  50, -34 },
	        new sbyte[]{ -22, 127 },
	        new sbyte[]{   4,  39 }, new sbyte[]{   0,  42 }, new sbyte[]{   7,  34 }, new sbyte[]{  11,  29 }, // 166 - 226
	        new sbyte[]{   8,  31 }, new sbyte[]{   6,  37 }, new sbyte[]{   7,  42 }, new sbyte[]{   3,  40 },
	        new sbyte[]{   8,  33 }, new sbyte[]{  13,  43 }, new sbyte[]{  13,  36 }, new sbyte[]{   4,  47 },
	        new sbyte[]{   3,  55 }, new sbyte[]{   2,  58 }, new sbyte[]{   6,  60 }, new sbyte[]{   8,  44 },
	        new sbyte[]{  11,  44 }, new sbyte[]{  14,  42 }, new sbyte[]{   7,  48 }, new sbyte[]{   4,  56 },
	        new sbyte[]{   4,  52 }, new sbyte[]{  13,  37 }, new sbyte[]{   9,  49 }, new sbyte[]{  19,  58 },
	        new sbyte[]{  10,  48 }, new sbyte[]{  12,  45 }, new sbyte[]{   0,  69 }, new sbyte[]{  20,  33 },
	        new sbyte[]{   8,  63 }, new sbyte[]{  35, -18 }, new sbyte[]{  33, -25 }, new sbyte[]{  28,  -3 },
	        new sbyte[]{  24,  10 }, new sbyte[]{  27,   0 }, new sbyte[]{  34, -14 }, new sbyte[]{  52, -44 },
	        new sbyte[]{  39, -24 }, new sbyte[]{  19,  17 }, new sbyte[]{  31,  25 }, new sbyte[]{  36,  29 },
	        new sbyte[]{  24,  33 }, new sbyte[]{  34,  15 }, new sbyte[]{  30,  20 }, new sbyte[]{  22,  73 },
	        new sbyte[]{  20,  34 }, new sbyte[]{  19,  31 }, new sbyte[]{  27,  44 }, new sbyte[]{  19,  16 },
	        new sbyte[]{  15,  36 }, new sbyte[]{  15,  36 }, new sbyte[]{  21,  28 }, new sbyte[]{  25,  21 },
	        new sbyte[]{  30,  20 }, new sbyte[]{  31,  12 }, new sbyte[]{  27,  16 }, new sbyte[]{  24,  42 },
	        new sbyte[]{   0,  93 }, new sbyte[]{  14,  56 }, new sbyte[]{  15,  57 }, new sbyte[]{  26,  38 },
	        new sbyte[]{ -24, 127 },
	        new sbyte[]{ -24, 115 }, new sbyte[]{ -22,  82 }, new sbyte[]{  -9,  62 }, new sbyte[]{   0,  53 }, // 227 - 275
	        new sbyte[]{   0,  59 }, new sbyte[]{ -14,  85 }, new sbyte[]{ -13,  89 }, new sbyte[]{ -13,  94 },
	        new sbyte[]{ -11,  92 }, new sbyte[]{ -29, 127 }, new sbyte[]{ -21, 100 }, new sbyte[]{ -14,  57 },
	        new sbyte[]{ -12,  67 }, new sbyte[]{ -11,  71 }, new sbyte[]{ -10,  77 }, new sbyte[]{ -21,  85 },
	        new sbyte[]{ -16,  88 }, new sbyte[]{ -23, 104 }, new sbyte[]{ -15,  98 }, new sbyte[]{ -37, 127 },
	        new sbyte[]{ -10,  82 }, new sbyte[]{  -8,  48 }, new sbyte[]{  -8,  61 }, new sbyte[]{  -8,  66 },
	        new sbyte[]{  -7,  70 }, new sbyte[]{ -14,  75 }, new sbyte[]{ -10,  79 }, new sbyte[]{  -9,  83 },
	        new sbyte[]{ -12,  92 }, new sbyte[]{ -18, 108 }, new sbyte[]{  -4,  79 }, new sbyte[]{ -22,  69 },
	        new sbyte[]{ -16,  75 }, new sbyte[]{  -2,  58 }, new sbyte[]{   1,  58 }, new sbyte[]{ -13,  78 },
	        new sbyte[]{  -9,  83 }, new sbyte[]{  -4,  81 }, new sbyte[]{ -13,  99 }, new sbyte[]{ -13,  81 },
	        new sbyte[]{  -6,  38 }, new sbyte[]{ -13,  62 }, new sbyte[]{  -6,  58 }, new sbyte[]{  -2,  59 },
	        new sbyte[]{ -16,  73 }, new sbyte[]{ -10,  76 }, new sbyte[]{ -13,  86 }, new sbyte[]{  -9,  83 },
	        new sbyte[]{ -10,  87 },
	        new sbyte[]{ 0, 0 },                                                                                // 276 a bit special (not used, bypass is used instead)
	        new sbyte[]{ -22, 127 }, new sbyte[]{ -25, 127 }, new sbyte[]{ -25, 120 }, new sbyte[]{ -27, 127 }, // 277 - 337
	        new sbyte[]{ -19, 114 }, new sbyte[]{ -23, 117 }, new sbyte[]{ -25, 118 }, new sbyte[]{ -26, 117 },
	        new sbyte[]{ -24, 113 }, new sbyte[]{ -28, 118 }, new sbyte[]{ -31, 120 }, new sbyte[]{ -37, 124 },
	        new sbyte[]{ -10,  94 }, new sbyte[]{ -15, 102 }, new sbyte[]{ -10,  99 }, new sbyte[]{ -13, 106 },
	        new sbyte[]{ -50, 127 }, new sbyte[]{  -5,  92 }, new sbyte[]{  17,  57 }, new sbyte[]{  -5,  86 },
	        new sbyte[]{ -13,  94 }, new sbyte[]{ -12,  91 }, new sbyte[]{  -2,  77 }, new sbyte[]{   0,  71 },
	        new sbyte[]{  -1,  73 }, new sbyte[]{   4,  64 }, new sbyte[]{  -7,  81 }, new sbyte[]{   5,  64 },
	        new sbyte[]{  15,  57 }, new sbyte[]{   1,  67 }, new sbyte[]{   0,  68 }, new sbyte[]{ -10,  67 },
	        new sbyte[]{   1,  68 }, new sbyte[]{   0,  77 }, new sbyte[]{   2,  64 }, new sbyte[]{   0,  68 },
	        new sbyte[]{  -5,  78 }, new sbyte[]{   7,  55 }, new sbyte[]{   5,  59 }, new sbyte[]{   2,  65 },
	        new sbyte[]{  14,  54 }, new sbyte[]{  15,  44 }, new sbyte[]{   5,  60 }, new sbyte[]{   2,  70 },
	        new sbyte[]{  -2,  76 }, new sbyte[]{ -18,  86 }, new sbyte[]{  12,  70 }, new sbyte[]{   5,  64 },
	        new sbyte[]{ -12,  70 }, new sbyte[]{  11,  55 }, new sbyte[]{   5,  56 }, new sbyte[]{   0,  69 },
	        new sbyte[]{   2,  65 }, new sbyte[]{  -6,  74 }, new sbyte[]{   5,  54 }, new sbyte[]{   7,  54 },
	        new sbyte[]{  -6,  76 }, new sbyte[]{ -11,  82 }, new sbyte[]{  -2,  77 }, new sbyte[]{  -2,  77 },
	        new sbyte[]{  25,  42 }, 
	        new sbyte[]{  17, -13 }, new sbyte[]{  16,  -9 }, new sbyte[]{  17, -12 }, new sbyte[]{  27, -21 }, // 338 - 398
	        new sbyte[]{  37, -30 }, new sbyte[]{  41, -40 }, new sbyte[]{  42, -41 }, new sbyte[]{  48, -47 },
	        new sbyte[]{  39, -32 }, new sbyte[]{  46, -40 }, new sbyte[]{  52, -51 }, new sbyte[]{  46, -41 },
	        new sbyte[]{  52, -39 }, new sbyte[]{  43, -19 }, new sbyte[]{  32,  11 }, new sbyte[]{  61, -55 },
	        new sbyte[]{  56, -46 }, new sbyte[]{  62, -50 }, new sbyte[]{  81, -67 }, new sbyte[]{  45, -20 },
	        new sbyte[]{  35,  -2 }, new sbyte[]{  28,  15 }, new sbyte[]{  34,   1 }, new sbyte[]{  39,   1 },
	        new sbyte[]{  30,  17 }, new sbyte[]{  20,  38 }, new sbyte[]{  18,  45 }, new sbyte[]{  15,  54 },
	        new sbyte[]{   0,  79 }, new sbyte[]{  36, -16 }, new sbyte[]{  37, -14 }, new sbyte[]{  37, -17 },
	        new sbyte[]{  32,   1 }, new sbyte[]{  34,  15 }, new sbyte[]{  29,  15 }, new sbyte[]{  24,  25 },
	        new sbyte[]{  34,  22 }, new sbyte[]{  31,  16 }, new sbyte[]{  35,  18 }, new sbyte[]{  31,  28 },
	        new sbyte[]{  33,  41 }, new sbyte[]{  36,  28 }, new sbyte[]{  27,  47 }, new sbyte[]{  21,  62 },
	        new sbyte[]{  18,  31 }, new sbyte[]{  19,  26 }, new sbyte[]{  36,  24 }, new sbyte[]{  24,  23 },
	        new sbyte[]{  27,  16 }, new sbyte[]{  24,  30 }, new sbyte[]{  31,  29 }, new sbyte[]{  22,  41 },
	        new sbyte[]{  22,  42 }, new sbyte[]{  16,  60 }, new sbyte[]{  15,  52 }, new sbyte[]{  14,  60 },
	        new sbyte[]{   3,  78 }, new sbyte[]{ -16, 123 }, new sbyte[]{  21,  53 }, new sbyte[]{  22,  56 },
	        new sbyte[]{  25,  61 }, 
	        new sbyte[]{  21,  33 }, new sbyte[]{  19,  50 }, new sbyte[]{  17,  61 },                          // 399 - 435
	        new sbyte[]{  -3,  78 }, new sbyte[]{  -8,  74 }, new sbyte[]{  -9,  72 }, new sbyte[]{ -10,  72 },
	        new sbyte[]{ -18,  75 }, new sbyte[]{ -12,  71 }, new sbyte[]{ -11,  63 }, new sbyte[]{  -5,  70 },
	        new sbyte[]{ -17,  75 }, new sbyte[]{ -14,  72 }, new sbyte[]{ -16,  67 }, new sbyte[]{  -8,  53 },
	        new sbyte[]{ -14,  59 }, new sbyte[]{  -9,  52 }, new sbyte[]{ -11,  68 }, new sbyte[]{   9,  -2 },
	        new sbyte[]{  30, -10 }, new sbyte[]{  31,  -4 }, new sbyte[]{  33,  -1 }, new sbyte[]{  33,   7 },
	        new sbyte[]{  31,  12 }, new sbyte[]{  37,  23 }, new sbyte[]{  31,  38 }, new sbyte[]{  20,  64 },
	        new sbyte[]{  -9,  71 }, new sbyte[]{  -7,  37 }, new sbyte[]{  -8,  44 }, new sbyte[]{ -11,  49 },
	        new sbyte[]{ -10,  56 }, new sbyte[]{ -12,  59 }, new sbyte[]{  -8,  63 }, new sbyte[]{  -9,  67 },
	        new sbyte[]{  -6,  68 }, new sbyte[]{ -10,  79 },
	        new sbyte[]{  -3,  78 }, new sbyte[]{  -8,  74 }, new sbyte[]{  -9,  72 }, new sbyte[]{ -10,  72 }, // 436 - 459
	        new sbyte[]{ -18,  75 }, new sbyte[]{ -12,  71 }, new sbyte[]{ -11,  63 }, new sbyte[]{  -5,  70 },
	        new sbyte[]{ -17,  75 }, new sbyte[]{ -14,  72 }, new sbyte[]{ -16,  67 }, new sbyte[]{  -8,  53 },
	        new sbyte[]{ -14,  59 }, new sbyte[]{  -9,  52 }, new sbyte[]{ -11,  68 }, new sbyte[]{   9,  -2 },
	        new sbyte[]{  30, -10 }, new sbyte[]{  31,  -4 }, new sbyte[]{  33,  -1 }, new sbyte[]{  33,   7 },
	        new sbyte[]{  31,  12 }, new sbyte[]{  37,  23 }, new sbyte[]{  31,  38 }, new sbyte[]{  20,  64 },
	    }
	};

		/**
		*
		* @param buf_size size of buf in bits
		*/
		public void ff_init_cabac_decoder(int[] buf, int buf_offset, int buf_size)
		{
			bytestream_start = buf_offset;
			bytestream_current = buf_offset;
			bytestream = buf;
			bytestream_end = buf_offset + buf_size;

			// DebugTool.printDebugString("buf = {"+buf[bytestream_current]+","+buf[bytestream_current+1]+","+buf[bytestream_current+2]+","+buf[bytestream_current+3]+")\n");

			low = bytestream[bytestream_current++] << 18;
			// DebugTool.printDebugString("init_cabac_decoder(1): low="+low+", range="+range+"\n");
			low += bytestream[bytestream_current++] << 10;
			// DebugTool.printDebugString("init_cabac_decoder(2): low="+low+", range="+range+"\n");
			low += (bytestream[bytestream_current++] << 2) + 2;
			// DebugTool.printDebugString("init_cabac_decoder(3): low="+low+", range="+range+"\n");
			range = 0x000001FE;

			// DebugTool.printDebugString("init_cabac_decoder(4): low="+low+", range="+range+"\n");
		}

		public void ff_init_cabac_states()
		{
			int i, j;

			for (i = 0; i < 64; i++)
			{
				for (j = 0; j < 4; j++)
				{ //FIXME check if this is worth the 1 shift we save
					ff_h264_lps_range[j * 2 * 64 + 2 * i + 0] =
					ff_h264_lps_range[j * 2 * 64 + 2 * i + 1] = lps_range[i][j];
				}

				ff_h264_mlps_state[128 + 2 * i + 0] =
				ff_h264_mps_state[2 * i + 0] = (short)(2 * mps_state[i] + 0);
				ff_h264_mlps_state[128 + 2 * i + 1] =
				ff_h264_mps_state[2 * i + 1] = (short)(2 * mps_state[i] + 1);

				if (i != 0)
				{
					ff_h264_mlps_state[128 - 2 * i - 1] = (short)(2 * lps_state[i] + 0);
					ff_h264_mlps_state[128 - 2 * i - 2] = (short)(2 * lps_state[i] + 1);
				}
				else
				{
					ff_h264_mlps_state[128 - 2 * i - 1] = 1;
					ff_h264_mlps_state[128 - 2 * i - 2] = 0;
				}
			}
		}

		///////////////
		// Decoder function

		public void refill()
		{
			// DebugTool.printDebugString("refill(1): low="+low+", range="+range+"\n");

			low += (bytestream[bytestream_current + 0] << 9) + (bytestream[bytestream_current + 1] << 1);
			low -= CABAC_MASK;
			bytestream_current += CABAC_BITS / 8;

			// DebugTool.printDebugString("refill(2): low="+low+", range="+range+"\n");
		}

		public void refill2()
		{
			int i, x;
			// DebugTool.printDebugString("refill2(1): low="+low+", range="+range+"\n");

			x = low ^ (low - 1);
			i = 7 - ff_h264_norm_shift[x >> (CABAC_BITS - 1)];
			x = -CABAC_MASK;
			x += (bytestream[bytestream_current + 0] << 9) + (bytestream[bytestream_current + 1] << 1);
			low += x << i;
			bytestream_current += CABAC_BITS / 8;

			// DebugTool.printDebugString("refill2(2): low="+low+", range="+range+"\n");
		}

		public void renorm_cabac_decoder()
		{
			// DebugTool.printDebugString("renorm_cabac_decoder(1): low="+low+", range="+range+"\n");

			while (range < 0x00000100)
			{
				range += range;
				low += low;
				if ((low & CABAC_MASK) == 0)
					refill();
			}
			// DebugTool.printDebugString("renorm_cabac_decoder(2): low="+low+", range="+range+"\n");

		}

		public void renorm_cabac_decoder_once(){
        // DebugTool.printDebugString("renorm_cabac_decoder_once(1): low="+low+", range="+range+"\n");

		int shift= (int)(((uint)(range - 0x00000100))>>31);
	    range<<= shift;
	    low  <<= shift;

	    // DebugTool.printDebugString("renorm_cabac_decoder_once: shift="+shift+"\n");

	    if((low & CABAC_MASK) == 0)
	        refill();

	    // DebugTool.printDebugString("renorm_cabac_decoder_once(2): low="+low+", range="+range+"\n");
	}

		public int get_cabac_inline(int[] state, int state_offset)
		{

			int s = state[state_offset];
			int RangeLPS = ff_h264_lps_range[2 * (range & 0x000000C0) + s];
			int bit, lps_mask;

			// DebugTool.printDebugString(" * get_cabac_inline(1): s="+s+", range="+range+", low="+low+", RangeLPS="+RangeLPS+"\n");

			range -= RangeLPS;
			lps_mask = ((range << (CABAC_BITS + 1)) - low) >> 31;
			low -= (range << (CABAC_BITS + 1)) & lps_mask;
			range += (RangeLPS - range) & lps_mask;

			s ^= lps_mask;
			state[state_offset] = ff_h264_mlps_state[128 + s];

			// DebugTool.printDebugString(" * get_cabac_inline: state=>"+ff_h264_mlps_state[128 +s]+"\n");

			bit = s & 1;

			// DebugTool.printDebugString(" * get_cabac_inline(2): state="+s+", range="+range+", low="+low+", lps_mask="+lps_mask+"\n");

			lps_mask = ff_h264_norm_shift[range];
			range <<= lps_mask;
			low <<= lps_mask;

			// DebugTool.printDebugString(" * get_cabac_inline(3): range="+range+", low="+low+", lps_mask="+lps_mask+"\n");

			if ((low & CABAC_MASK) == 0)
			{
				refill2();
				// DebugTool.printDebugString(" * get_cabac_inline(4): low="+low+"\n");
			} // if
			return bit;

		}

		public int get_cabac_noinline(int[] state, int state_offset)
		{
			return get_cabac_inline(state, state_offset);
		}

		public int get_cabac(int[] state, int state_offset)
		{
			return get_cabac_inline(state, state_offset);
		}

		public int get_cabac_bypass()
		{
			int _range;

			// DebugTool.printDebugString("get_cabac_bypass(1): low="+low+", range="+range+"\n");

			low += low;

			if ((low & CABAC_MASK) == 0)
				refill();

			_range = range << (CABAC_BITS + 1);

			// DebugTool.printDebugString("get_cabac_bypass(2): low="+low+", range="+range+"\n");

			if (low < _range)
			{
				return 0;
			}
			else
			{
				low -= _range;
				return 1;
			}
		}

		public int get_cabac_bypass_sign(int val)
		{
			int _range, _mask;
			// DebugTool.printDebugString("get_cabac_bypass_sign(1): low="+low+", range="+range+"\n");

			low += low;

			if ((low & CABAC_MASK) == 0)
				refill();

			_range = range << (CABAC_BITS + 1);
			low -= _range;
			_mask = low >> 31;
			_range &= _mask;
			low += _range;

			// DebugTool.printDebugString("get_cabac_bypass_sign(2): low="+low+", range="+range+"\n");

			return (val ^ _mask) - _mask;
		}

		/**
		 *
		 * @return the number of bytes read or 0 if no end
		 */
		public int get_cabac_terminate()
		{
			range -= 2;
			if (low < (range << (CABAC_BITS + 1)))
			{
				renorm_cabac_decoder_once();
				return 0;
			}
			else
			{
				return bytestream_current - bytestream_start;
			}
		}


		//////////////////////////////////////////////////////
		// H264 Specific CABAC Decoding Functions (2nd level)

		public int decode_cabac_field_decoding_flag(H264Context h)
		{

			int mbb_xy = (int)(h.mb_xy - 2L * h.s.mb_stride);
			int ctx = 0;

			ctx += (h.mb_field_decoding_flag & (/*!!*/h.s.mb_x)); //for FMO:(s.current_picture.mb_type[mba_xy]>>7)&(this.slice_table_base[this.slice_table_offset + mba_xy] == this.slice_num);
			ctx += (int)((h.s.current_picture.mb_type_base[h.s.current_picture.mb_type_offset + mbb_xy] >> 7) & (h.slice_table_base[h.slice_table_offset + mbb_xy] == h.slice_num ? 1 : 0));

			return this.get_cabac_noinline(h.cabac_state, 70 + ctx);// &(this.cabac_state+70)[ctx] );
		}

		public int decode_cabac_intra_mb_type(H264Context h, int ctx_base, int intra_slice)
		{
			int state_offset = ctx_base;
			int mb_type;

			if (intra_slice != 0)
			{
				int ctx = 0;
				if ((h.left_type[0] & (H264Context.MB_TYPE_INTRA16x16 | H264Context.MB_TYPE_INTRA_PCM)) != 0)
					ctx++;
				if ((h.top_type & (H264Context.MB_TYPE_INTRA16x16 | H264Context.MB_TYPE_INTRA_PCM)) != 0)
					ctx++;
				if (this.get_cabac_noinline(h.cabac_state, state_offset + ctx) == 0)
					return 0;   /* I4x4 */
				state_offset += 2;
			}
			else
			{
				if (this.get_cabac_noinline(h.cabac_state, state_offset) == 0)
					return 0;   /* I4x4 */
			}

			if (this.get_cabac_terminate() != 0)
				return 25;  /* PCM */

			mb_type = 1; /* I16x16 */
			mb_type += 12 * this.get_cabac_noinline(h.cabac_state, state_offset + 1); /* cbp_luma != 0 */
			if (this.get_cabac_noinline(h.cabac_state, state_offset + 2) != 0) /* cbp_chroma */
				mb_type += 4 + 4 * this.get_cabac_noinline(h.cabac_state, state_offset + 2 + intra_slice);
			mb_type += 2 * this.get_cabac_noinline(h.cabac_state, state_offset + 3 + intra_slice);
			mb_type += 1 * this.get_cabac_noinline(h.cabac_state, state_offset + 3 + 2 * intra_slice);
			return mb_type;
		}

		public int decode_cabac_mb_skip(H264Context h, int mb_x, int mb_y)
		{
			int mba_xy, mbb_xy;
			int ctx = 0;

			if (h.mb_aff_frame != 0)
			{ //FIXME merge with the stuff in fill_caches?
				int mb_xy = mb_x + (mb_y & ~1) * h.s.mb_stride;
				mba_xy = mb_xy - 1;
				if ((mb_y & 1) != 0
					&& h.slice_table_base[h.slice_table_offset + mba_xy] == h.slice_num
					&& h.mb_field_decoding_flag == ((h.s.current_picture.mb_type_base[h.s.current_picture.mb_type_offset + mba_xy] & H264Context.MB_TYPE_INTERLACED) != 0 ? 1 : 0))
					mba_xy += h.s.mb_stride;
				if (h.mb_field_decoding_flag != 0)
				{
					mbb_xy = mb_xy - h.s.mb_stride;
					if (0 == (mb_y & 1)
						&& h.slice_table_base[h.slice_table_offset + mbb_xy] == h.slice_num
						&& (h.s.current_picture.mb_type_base[h.s.current_picture.mb_type_offset + mbb_xy] & H264Context.MB_TYPE_INTERLACED) != 0)
						mbb_xy -= h.s.mb_stride;
				}
				else
					mbb_xy = mb_x + (mb_y - 1) * h.s.mb_stride;
			}
			else
			{
				int mb_xy = h.mb_xy;
				mba_xy = mb_xy - 1;
				mbb_xy = mb_xy - (h.s.mb_stride << 0);
			}

			if (h.slice_table_base[h.slice_table_offset + mba_xy] == h.slice_num && ((h.s.current_picture.mb_type_base[h.s.current_picture.mb_type_offset + mba_xy] & H264Context.MB_TYPE_SKIP) == 0))
				ctx++;
			if (h.slice_table_base[h.slice_table_offset + mbb_xy] == h.slice_num && ((h.s.current_picture.mb_type_base[h.s.current_picture.mb_type_offset + mbb_xy] & H264Context.MB_TYPE_SKIP) == 0))
				ctx++;

			if (h.slice_type_nos == H264Context.FF_B_TYPE)
				ctx += 13;
			int ret = this.get_cabac_noinline(h.cabac_state, 11 + ctx);

			// DebugTool.printDebugString(" * decode_cabac_mb_skip return "+ret+"\n");
			return ret;
		}

		public int decode_cabac_mb_intra4x4_pred_mode(H264Context h, int pred_mode)
		{
			int mode = 0;

			if (this.get_cabac(h.cabac_state, 68) != 0)
				return pred_mode;

			mode += 1 * this.get_cabac(h.cabac_state, 69);
			mode += 2 * this.get_cabac(h.cabac_state, 69);
			mode += 4 * this.get_cabac(h.cabac_state, 69);

			return mode + ((mode >= pred_mode) ? 1 : 0);
		}

		public int decode_cabac_mb_chroma_pre_mode(H264Context h)
		{
			int mba_xy = h.left_mb_xy[0];
			int mbb_xy = h.top_mb_xy;

			int ctx = 0;

			/* No need to test for IS_INTRA4x4 and IS_INTRA16x16, as we set chroma_pred_mode_table to 0 */
			if (h.left_type[0] != 0 && h.chroma_pred_mode_table[mba_xy] != 0)
				ctx++;

			if (h.top_type != 0 && h.chroma_pred_mode_table[mbb_xy] != 0)
				ctx++;

			if (this.get_cabac_noinline(h.cabac_state, 64 + ctx) == 0)
				return 0;

			if (this.get_cabac_noinline(h.cabac_state, 64 + 3) == 0)
				return 1;
			if (this.get_cabac_noinline(h.cabac_state, 64 + 3) == 0)
				return 2;
			else
				return 3;
		}

		public int decode_cabac_mb_cbp_luma(H264Context h)
		{
			int cbp_b, cbp_a, ctx, cbp = 0;

			cbp_a = h.left_cbp;
			cbp_b = h.top_cbp;

			ctx = ((cbp_a & 0x02) == 0 ? 1 : 0) + 2 * ((cbp_b & 0x04) == 0 ? 1 : 0);
			cbp += this.get_cabac_noinline(h.cabac_state, 73 + ctx);
			ctx = ((cbp & 0x01) == 0 ? 1 : 0) + 2 * ((cbp_b & 0x08) == 0 ? 1 : 0);
			cbp += this.get_cabac_noinline(h.cabac_state, 73 + ctx) << 1;
			ctx = ((cbp_a & 0x08) == 0 ? 1 : 0) + 2 * ((cbp & 0x01) == 0 ? 1 : 0);
			cbp += this.get_cabac_noinline(h.cabac_state, 73 + ctx) << 2;
			ctx = ((cbp & 0x04) == 0 ? 1 : 0) + 2 * ((cbp & 0x02) == 0 ? 1 : 0);
			cbp += this.get_cabac_noinline(h.cabac_state, 73 + ctx) << 3;
			return cbp;
		}

		public int decode_cabac_mb_cbp_chroma(H264Context h)
		{
			int ctx;
			int cbp_a, cbp_b;

			cbp_a = (h.left_cbp >> 4) & 0x03;
			cbp_b = (h.top_cbp >> 4) & 0x03;

			ctx = 0;
			if (cbp_a > 0) ctx++;
			if (cbp_b > 0) ctx += 2;
			if (this.get_cabac_noinline(h.cabac_state, 77 + ctx) == 0)
				return 0;

			ctx = 4;
			if (cbp_a == 2) ctx++;
			if (cbp_b == 2) ctx += 2;
			return 1 + this.get_cabac_noinline(h.cabac_state, 77 + ctx);
		}

		public int decode_cabac_p_mb_sub_type(H264Context h)
		{
			if (this.get_cabac(h.cabac_state, 21) != 0)
				return 0;   /* 8x8 */
			if (this.get_cabac(h.cabac_state, 22) == 0)
				return 1;   /* 8x4 */
			if (this.get_cabac(h.cabac_state, 23) != 0)
				return 2;   /* 4x8 */
			return 3;       /* 4x4 */
		}

		public int decode_cabac_b_mb_sub_type(H264Context h)
		{
			int type;
			if (0 == this.get_cabac(h.cabac_state, 36))
				return 0;   /* B_Direct_8x8 */
			if (0 == this.get_cabac(h.cabac_state, 37))
				return 1 + this.get_cabac(h.cabac_state, 39); /* B_L0_8x8, B_L1_8x8 */
			type = 3;
			if (0 != this.get_cabac(h.cabac_state, 38))
			{
				if (0 != this.get_cabac(h.cabac_state, 39))
					return 11 + this.get_cabac(h.cabac_state, 39); /* B_L1_4x4, B_Bi_4x4 */
				type += 4;
			}
			type += 2 * this.get_cabac(h.cabac_state, 39);
			type += this.get_cabac(h.cabac_state, 39);
			return type;
		}

		public int decode_cabac_mb_ref(H264Context h, int list, int n) {
			int refa = h.ref_cache[list, H264Context.scan8[n] - 1];
			int refb = h.ref_cache[list, H264Context.scan8[n] - 8];
	    int @ref  = 0;
	    int ctx  = 0;

	    if( h.slice_type_nos == H264Context.FF_B_TYPE) {
			if (refa > 0 && 0 == (h.direct_cache[H264Context.scan8[n] - 1] & (H264Context.MB_TYPE_DIRECT2 >> 1)))
	            ctx++;
			if (refb > 0 && 0 == (h.direct_cache[H264Context.scan8[n] - 8] & (H264Context.MB_TYPE_DIRECT2 >> 1)))
	            ctx += 2;
	    } else {
	        if( refa > 0 )
	            ctx++;
	        if( refb > 0 )
	            ctx += 2;
	    }

	    while( this.get_cabac( h.cabac_state, 54+ctx ) != 0) {
	        @ref++;
	        ctx = (ctx>>2)+4;
	        if(@ref >= 32 /*this.ref_list[list]*/){
	            return -1;
	        }
	    }
		return @ref;
	}

		public const int INT_BIT = 32;
		public int decode_cabac_mb_mvd(H264Context h, int ctxbase, int amvd, int[] mvda)
		{
			int mvd;

			// DebugTool.printDebugString(" - decode_cabac_mb_mvd: ctxbase="+ctxbase+", amvd="+amvd+", state_offset="+(ctxbase+((amvd-3)>>(INT_BIT-1))+((amvd-33)>>(INT_BIT-1))+2)+"\n");

			// DebugTool.printDebugString(" - INT_BIT="+INT_BIT+", ((amvd-3)>>(INT_BIT-1))="+((amvd-3)>>(INT_BIT-1))+", ((amvd-33)>>(INT_BIT-1))="+((amvd-33)>>(INT_BIT-1))+"\n");

			if (0 == this.get_cabac(h.cabac_state, ctxbase + ((amvd - 3) >> (INT_BIT - 1)) + ((amvd - 33) >> (INT_BIT - 1)) + 2))
			{
				mvda[0] = 0;
				return 0;
			}

			mvd = 1;
			ctxbase += 3;
			while (mvd < 9 && this.get_cabac(h.cabac_state, ctxbase) != 0)
			{
				if (mvd < 4)
					ctxbase++;
				mvd++;
			}

			if (mvd >= 9)
			{
				int k = 3;
				while (this.get_cabac_bypass() != 0)
				{
					mvd += 1 << k;
					k++;
					if (k > 24)
					{
						//System.out.println("overflow in decode_cabac_mb_mvd");
						return int.MinValue;
					}
				}
				while (k-- != 0)
				{
					mvd += this.get_cabac_bypass() << k;
				}
				mvda[0] = mvd < 70 ? mvd : 70;
			}
			else
				mvda[0] = mvd;
			return this.get_cabac_bypass_sign(-mvd);
		}

		// CABAC Decoder Functions
		public void ff_h264_init_cabac_states(H264Context h)
		{
			sbyte[][] tab =
				(h.slice_type_nos == H264Context.FF_I_TYPE)
				? cabac_context_init_I
				: cabac_context_init_PB[h.cabac_init_idc]
			;

			/* calculate pre-state */
			for (int i = 0; i < 460; i++)
			{
				int pre = 2 * (((tab[i][0] * h.s.qscale) >> 4) + tab[i][1]) - 127;
				pre ^= pre >> 31;
				if (pre > 124)
					pre = 124 + (pre & 1);
				h.cabac_state[i] = pre;
			}
		}

	}
}