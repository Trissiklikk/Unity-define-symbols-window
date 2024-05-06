using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public sealed class EditorPrefsSaveHandle : BaseSaveHandle
    {
        /// <summary>
        /// Save define symbols to EditorPrefs.
        /// </summary>
        /// <param name="defineSymbolsData">List of string you need to save</param>
        public override void Save(List<DefineSymbolsData> defineSymbolsData)
        {
            string saveKey = GetSaveKey();
            string defineSymbolsString = ConvertDataToString(defineSymbolsData);

            EditorPrefs.SetString(saveKey, defineSymbolsString);
        }

        /// <summary>
        /// Load define symbols from EditorPrefs.
        /// </summary>
        /// <returns></returns>
        public override List<DefineSymbolsData> Load()
        {
            string saveKey = GetSaveKey();
            string defineSymbolsString = EditorPrefs.GetString(saveKey, string.Empty);
            List<DefineSymbolsData> result = new List<DefineSymbolsData>();

            if (!string.IsNullOrEmpty(defineSymbolsString))
            {
                string[] defineSymbols = defineSymbolsString.Split(';');

                for (int i = 0; i < defineSymbols.Length; i++)
                {
                    DefineSymbolsData data = JsonConvert.DeserializeObject<DefineSymbolsData>(defineSymbols[i]);
                    result.Add(data);
                }
            }

            return result;
        }
    }
}