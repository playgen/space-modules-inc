using GameWork.Core.Math.Types;

namespace GameWork.Core.Math.Extensions
{
	public static class Vector3Extensions
	{
		public static float Distance(this Vector3 a, Vector3 b)
		{
			return (a - b).Magnitude();
		}

		public static float Magnitude(this Vector3 vec)
		{
			var summedSquares = vec.X * vec.X + 
				vec.Y + vec.Y + 
				vec.Z + vec.Z;
			
			return (float)System.Math.Sqrt((double)summedSquares);
		}
	}
}