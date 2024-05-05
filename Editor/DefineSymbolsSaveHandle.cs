using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class DefineSymbolsSaveHandle
    {
        private const string SAVE_KEY = "{0}_DefineSymbols";

        /// <summary>
        /// Save define symbols to PlayerSettings.
        /// </summary>
        /// <param name="buildTargetGroup">The type of build target you want to save.</param>
        /// <param name="defineSymbols">List of string you need to Apply.</param>
        public void ApplyDataToProject(BuildTargetGroup buildTargetGroup, List<DefineSymbolsData> defineSymbolsData)
        {
            string defineSymbolsString = ConvertDataToStringOnlyName(defineSymbolsData);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbolsString);
        }

        /// <summary>
        /// Save define symbols to EditorPrefs.
        /// </summary>
        /// <param name="defineSymbolsData">List of string you need to save</param>
        public void Save(List<DefineSymbolsData> defineSymbolsData)
        {
            string saveKey = GetSaveKey();
            string defineSymbolsString = ConvertDataToString(defineSymbolsData);

            EditorPrefs.SetString(saveKey, defineSymbolsString);
        }

        /// <summary>
        /// Load define symbols from EditorPrefs.
        /// </summary>
        /// <returns></returns>
        public List<DefineSymbolsData> Load()
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

        /// <summary>
        /// Get current project define symbols from PlayerSettings.
        /// </summary>
        /// <param name="buildTargetGroup">Build target</param>
        /// <returns></returns>
        public List<string> GetDefineSymbols(BuildTargetGroup buildTargetGroup)
        {
            string defineSymbolsString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(defineSymbolsString))
            {
                string[] defineSymbols = defineSymbolsString.Split(';');

                result.AddRange(defineSymbols);
            }

            return result;
        }

        /// <summary>
        /// Convert DefineSymbolsData list to string.
        /// </summary>
        /// <param name="defineSymbolsData">List of Define Symbols Data you want to convert</param>
        /// <returns></returns>
        private string ConvertDataToString(List<DefineSymbolsData> defineSymbolsData)
        {
            string result = string.Empty;
            List<string> defineSymbols = new List<string>();

            if(defineSymbolsData.Count > 0)
            {
                for (int i = 0; i < defineSymbolsData.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(defineSymbolsData[i]);
                    defineSymbols.Add(json);
                }

                result = string.Join(";", defineSymbols);
            }

            return result;
        }

        /// <summary>
        /// Convert DefineSymbolsData list to string only name.
        /// </summary>
        /// <param name="defineSymbolsData">List of Define Symbols Data you want to convert</param>
        /// <returns></returns>
        private string ConvertDataToStringOnlyName(List<DefineSymbolsData> defineSymbolsDatas)
        {
            string result = string.Empty;
            List<string> defineSymbolsName = new List<string>();

            if (defineSymbolsDatas.Count > 0)
            {
                for (int i = 0; i < defineSymbolsDatas.Count; i++)
                {
                    defineSymbolsName.Add(defineSymbolsDatas[i].SymbolName);
                }

                result = string.Join(";", defineSymbolsName);
            }

            return result;
        }

        /// <summary>
        /// This method returns save key.
        /// </summary>
        /// <returns></returns>
        private string GetSaveKey()
        {
            return string.Format(SAVE_KEY, Application.productName);
        }
    }
}