namespace GameEngine.Extensions
{
	public static class TypeConversionExtensions
	{
		public static UnityEngine.Vector3 ToEngineType(this GameWork.Core.Math.Types.Vector3 gameWorkType)
		{
			return new UnityEngine.Vector3(gameWorkType.X, gameWorkType.Y, gameWorkType.Z);
		}

		public static GameWork.Core.Math.Types.Vector3 ToGameWorkType(this UnityEngine.Vector3 engineType)
		{
			return new GameWork.Core.Math.Types.Vector3(engineType.x, engineType.y, engineType.z);
		}
	}
}