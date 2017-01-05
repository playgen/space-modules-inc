namespace GameWork.Core.Math
{
	public static class MathF
	{
		public static float Lerp(float start, float end, float weight)
		{
            weight = weight.Clamp(0f, 1f);

            var range = (end - start);
            var progress = range * weight;

            return start + progress;
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