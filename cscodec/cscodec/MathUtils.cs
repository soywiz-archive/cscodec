namespace cscodec
{
	public static class MathUtils
	{
		static public int Clamp(int Value, int Min, int Max)
		{
			if (Value < Min) return Min;
			if (Value > Max) return Max;
			return Value;
		}
	}
}
