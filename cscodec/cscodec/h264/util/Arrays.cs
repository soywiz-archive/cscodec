using System;
namespace cscodec.h243.util
{
	public class Arrays
	{
		public static T[] Create1D<T>(int Rank1)
		{
			return new T[Rank1];
		}

		public static T[][] Create2D<T>(int Rank1, int Rank2)
		{
			var Ret = new T[Rank1][];
			for (int n = 0; n < Rank1; n++) Ret[n] = Create1D<T>(Rank2);
			return Ret;
		}

		public static T[][][] Create3D<T>(int Rank1, int Rank2, int Rank3)
		{
			var Ret = new T[Rank1][][];
			for (int n = 0; n < Rank1; n++) Ret[n] = Create2D<T>(Rank2, Rank3);
			return Ret;
		}

		public static T[][][][] Create4D<T>(int Rank1, int Rank2, int Rank3, int Rank4)
		{
			var Ret = new T[Rank1][][][];
			for (int n = 0; n < Rank1; n++) Ret[n] = Create3D<T>(Rank2, Rank3, Rank4);
			return Ret;
		}

		public static bool equals(Array array1,
				Array array2)
		{
			if (array1 == null || array2 == null) return false;
			int[] a1 = (int[])array1;
			int[] a2 = (int[])array2;
			if (a1.Length != a2.Length) return false;
			for (int i = 0; i < a1.Length; i++)
			{
				if (a1 != a2)
					return false;
			} // for
			return true;
		}

		public static void fill(int[] arr, int startIdxIncl, int endIdxExcl, int val)
		{
			for (int i = startIdxIncl; i < endIdxExcl; i++)
				arr[i] = val;
		}

		public static void fill(int[] arr, int val)
		{
			for (int i = 0; i < arr.Length; i++)
				arr[i] = val;
		}

		public static void fill(short[] arr, int startIdxIncl, int endIdxExcl, short val)
		{
			for (int i = startIdxIncl; i < endIdxExcl; i++)
				arr[i] = val;
		}

		public static void fill(short[] arr, short val)
		{
			for (int i = 0; i < arr.Length; i++)
				arr[i] = val;
		}

	}
}