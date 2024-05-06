using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public class DefineSymbolsData
    {
        private string m_symbolName;

        [JsonProperty("SymbolName")]
        public string SymbolName
        {
            get => m_symbolName;
            set => m_symbolName = value;
        }

        private bool m_isEnabled;

        [JsonProperty("IsEnabled")]
        public bool IsEnabled
        {
            get => m_isEnabled;
            set => m_isEnabled = value;
        }
    }
}