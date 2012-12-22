using System;
namespace cscodec.h243.util
{
	public class Arrays
	{
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