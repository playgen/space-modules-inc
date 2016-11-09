using System.IO;

namespace GameWork.Core.IO.Interfaces
{
	public interface IFileLoader
	{
		Stream Load(string path);
	}
}