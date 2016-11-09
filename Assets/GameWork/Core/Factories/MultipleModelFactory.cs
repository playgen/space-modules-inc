using System;
using System.Collections.Generic;
using GameWork.Core.Models.Interfaces;

namespace GameWork.Core.Factories
{
    public class MultipleModelFactory<TObject, TModel>
        where TModel : IIdModel
    {
        protected readonly Dictionary<string, TModel> Models = new Dictionary<string, TModel>();

        public void AddModel(TModel model)
        {
            Models.Add(model.Id, model);
        }

        public virtual TObject Create(string modelId)
        {
            var model = Models[modelId];
            return (TObject) Activator.CreateInstance(typeof(TObject), model);
        }
    }
}