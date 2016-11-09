using GameWork.Core.Logging.Interfaces;
using System;
using System.Collections.Generic;

namespace GameWork.Core.Localization
{
    public class LocalizationManager
    {
        private readonly ILogger _logger;
        private LocalizationModel _model;
        private Dictionary<string, string> _currentLocalization;

        public event Action SetLocaleEvent;

        public LocalizationManager(ILogger logger)
        {
            _logger = logger;
        }

        public void SetModel(LocalizationModel model)
        {
            _model = model;
            SetLocale(_model.Default);
        }

        public void SetLocale(string localeId)
        {
            _currentLocalization = _model.Localizations[localeId];

            if (SetLocaleEvent != null)
            {
                SetLocaleEvent();
            }
        }

        public string GetLocalization(string id)
        {
            if (!_currentLocalization.ContainsKey(id))
            {
                _logger.Error("Localization doesn't contain key for: \"" + id + "\"");
                return string.Empty;
            }

            return _currentLocalization[id];
        }

        public bool HasLocale(string locale)
        {
            return _model.Localizations.ContainsKey(locale);
        }

        public void Reset()
        {
            SetLocale(_model.Default);
        }
    }
}
