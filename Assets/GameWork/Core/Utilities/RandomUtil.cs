using System;

namespace GameWork.Core.Utilities
{
    public static class RandomUtil
    {
        private static readonly Random Random = new Random();

        public static int Next()
        {
            return Random.Next();
        }

        public static int Next(int exclusiveMax)
		{
			return Random.Next(0, exclusiveMax);
		}

		public static int Next(int inclusiveMin, int exclusiveMax)
        {
			return Random.Next(inclusiveMin, exclusiveMax);
        }

		public static float NextFloat()
		{
			return (float)Random.NextDouble();
		}
    }
}