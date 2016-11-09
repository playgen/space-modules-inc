using System.IO;

namespace GameWork.Core.Models.Interfaces
{
	public interface IModelReader
	{
		TModel ConstructModel<TModel>(Stream stream) where TModel : IModel;
	}
}