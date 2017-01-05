using System.IO;
using GameWork.Core.Models.Interfaces;

namespace GameWork.Core.IO.EngineAdaptors
{
	public interface IJsonReader
	{
		TModel ConstructModel<TModel>(Stream stream) where TModel : IModel;
	}
}