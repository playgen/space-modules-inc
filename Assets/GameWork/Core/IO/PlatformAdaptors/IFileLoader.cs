using System.IO;

namespace GameWork.Core.IO.EngineAdaptors
{
	public interface IFileLoader
	{
		Stream Load(string path);
	}
}