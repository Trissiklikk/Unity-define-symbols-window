using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public class DefineSymbolsData
    {
        private string m_symbolName;

        public string SymbolName
        {
            get => m_symbolName;
            set => m_symbolName = value;
        }

        private bool m_isEnabled;

        public bool IsEnabled
        {
            get => m_isEnabled;
            set => m_isEnabled = value;
        }

        public DefineSymbolsData(string symbolName, bool isEnabled)
        {
            m_symbolName = symbolName;
            m_isEnabled = isEnabled;
        }
    }
}