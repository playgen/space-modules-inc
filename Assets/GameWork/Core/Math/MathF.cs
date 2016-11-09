namespace GameWork.Core.Math
{
	public static class MathF
	{
		public static float Lerp(float start, float end, float weight)
		{
			weight = weight.Clamp(0f, 1f);

			var difference = (end - start);
			var progress = difference * weight; 

			if(progress < 0)
			{
				progress = Abs(difference) + progress;
			}

			return progress;
		}

		public static float Clamp(this float val, float min, float max)
		{
			if(val < min) return min;
			else if(max < val) return max;
			else return val;
		}

		public static float Abs(float val)
		{
			if(val < 0)
			{
				val *= -1f;
			}

			return val;
		}
	}
}