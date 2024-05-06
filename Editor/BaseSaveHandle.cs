using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public abstract class BaseSaveHandle
    {
        private const string SAVE_KEY = "{0}_DefineSymbols";

        public abstract void Save(List<DefineSymbolsData> defineSymbolsData);
        public abstract List<DefineSymbolsData> Load();

        /// <summary>
        /// Get current project define symbols from PlayerSettings.
        /// </summary>
        /// <param name="buildTargetGroup">Build target</param>
        /// <returns></returns>
        public virtual List<string> GetDefineSymbols(BuildTargetGroup buildTargetGroup)
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

        public virtual void ApplyDataToProject(BuildTargetGroup buildTargetGroup, List<DefineSymbolsData> defineSymbolsData)
        {
            string defineSymbolsString = ConvertDataToStringOnlyName(defineSymbolsData);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defineSymbolsString);
        }

        /// <summary>
        /// Convert DefineSymbolsData list to string.
        /// </summary>
        /// <param name="defineSymbolsData">List of Define Symbols Data you want to convert</param>
        /// <returns></returns>
        protected string ConvertDataToString(List<DefineSymbolsData> defineSymbolsData)
        {
            string result = string.Empty;
            JArray defineSymbols = new JArray();

            if (defineSymbolsData.Count > 0)
            {
                for (int i = 0; i < defineSymbolsData.Count; i++)
                {
                    string json = JsonConvert.SerializeObject(defineSymbolsData[i]);
                    defineSymbols.Add(json);
                }

                result = JArray.Parse(defineSymbols.ToString()).ToString();
            }

            return result;
        }

        /// <summary>
        /// Convert DefineSymbolsData list to string only name.
        /// </summary>
        /// <param name="defineSymbolsData">List of Define Symbols Data you want to convert</param>
        /// <returns></returns>
        protected string ConvertDataToStringOnlyName(List<DefineSymbolsData> defineSymbolsDatas)
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
        protected string GetSaveKey()
        {
            return string.Format(SAVE_KEY, Application.productName);
        }
    }
}