using GameWork.Core.Models.Interfaces;
using System.Collections.Generic;

namespace GameWork.Core.Localization
{
    public struct LocalizationModel : IModel
    {
        public string Default { get; set; }

        public Dictionary<string, Dictionary<string, string>> Localizations { get; set; }
    }
}