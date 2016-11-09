using GameWork.Core.Models.Interfaces;
using System.IO;

namespace GameWork.Core.IO.Interfaces
{
	public interface IJsonReader
	{
		TModel ConstructModel<TModel>(Stream stream) where TModel : IModel;
	}
}