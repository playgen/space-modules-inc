using System.IO;
using System.Collections.Generic;
using GameWork.Core.IO;

namespace GameWork.Core.Localization
{
    public class LocalizationLoader
    {
        public LocalizationModel Load(Stream stream)
        {
            var model = new LocalizationModel
            {
                Localizations = new Dictionary<string, Dictionary<string, string>>()
            };

            var reader = new CSVReader(stream);
            var headers = new string[0];
            var row = 0;

            while (reader.Peek() >= 0)
            {
                var rowValues = reader.ReadRow();

                if (row == 0)
                {
                    headers = rowValues;
                    SetHeaders(model.Localizations, headers);
                }
                else
                {
                    AddKeys(model.Localizations, headers, rowValues);
                }

                row++;
            }

            return model;
        }

        private static void SetHeaders(Dictionary<string, Dictionary<string, string>> localizationDictionary , string[] headers)
        {
            foreach (var header in headers)
            {
                localizationDictionary[header] = new Dictionary<string, string>();
            }
        }

        private static void AddKeys(Dictionary<string, Dictionary<string, string>> localizationDictionary, string[] headers, string[] rowValues)
        {
            var key = rowValues[0];

            for (var col = 1; col < rowValues.Length; col++)
            {
                var header = headers[col];
                var value = rowValues[col];

                localizationDictionary[header].Add(key, value);
            }
        }
    }
}
