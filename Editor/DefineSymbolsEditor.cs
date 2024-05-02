using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools
{
    public sealed class DefineSymbolsEditor : EditorWindow
    {
        private static BuildTargetGroup m_currentBuildTargetGroup = BuildTargetGroup.Standalone;

        private List<string> m_defineSymbols = new List<string>();
        private string m_addDefineSymbol = string.Empty;
        private Vector2 m_scrollPosition;

        [MenuItem("Window/Trissiklikk Editor Tools/Define Symbols Window %#F2")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DefineSymbolsEditor));
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Target Device");
                m_currentBuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(m_currentBuildTargetGroup);
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPosition, new GUIStyle() { margin = new RectOffset(10, 10, 20, 20) }))
            {
                m_scrollPosition = scrollView.scrollPosition;

                for (int i = 0; i < m_defineSymbols.Count; i++)
                {
                    string defineSymbol = m_defineSymbols[i];
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(defineSymbol, new GUIStyle()
                        {
                            fontStyle = FontStyle.Bold,
                            fontSize = 12,
                            normal = new GUIStyleState()
                            {
                                textColor = Color.white
                            }
                        });

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Remove"))
                        {
                            Remove(defineSymbol);
                        }
                    }

                    GUILayout.Space(3);
                }
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                m_addDefineSymbol = EditorGUILayout.TextField(m_addDefineSymbol);

                if (GUILayout.Button("Clear"))
                {
                    ClearAddDefineSymbol();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Add"))
                {
                    Refresh();
                    Add();
                }

                if (GUILayout.Button("Refresh"))
                {
                    Refresh();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply"))
                {
                    Save(m_defineSymbols);
                }
            }

                GUILayout.Space(10);
        }

        /// <summary>
        /// Add define symbol to list.
        /// </summary>
        private void Add()
        {
            if (string.IsNullOrEmpty(m_addDefineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol cannot be empty", "OK");
                return;
            }

            if (m_defineSymbols.Contains(m_addDefineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol already exists", "OK");
                return;
            }

            m_defineSymbols.Add(m_addDefineSymbol);

            ClearAddDefineSymbol();
        }

        /// <summary>
        /// Refresh current define symbols list.
        /// </summary>
        private void Refresh()
        {
            string defineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(m_currentBuildTargetGroup);

            m_defineSymbols.Clear();
            m_defineSymbols.AddRange(defineSymbols.Split(';'));
        }

        /// <summary>
        /// Remove define symbol from list.
        /// </summary>
        /// <param name="defineSymbol">String value of symbol you need to remove.</param>
        private bool Remove(string defineSymbol)
        {
            EditorUtility.SetDirty(this);

            if (string.IsNullOrEmpty(defineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol cannot be empty", "OK");
                return false;
            }

            if (!m_defineSymbols.Contains(defineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol does not exist", "OK");
                return false;
            }

            bool isRemoved = m_defineSymbols.Remove(defineSymbol);

            if (!isRemoved)
            {
                EditorUtility.DisplayDialog("Error", "Define symbol does not exist", "OK");
                return false;
            }


            return true;
        }

        /// <summary>
        /// Clear add define symbol field.
        /// </summary>
        private void ClearAddDefineSymbol()
        {
            m_addDefineSymbol = string.Empty;

            GUI.FocusControl(null);
        }

        /// <summary>
        /// Save define symbols to PlayerSettings.
        /// </summary>
        /// <param name="defineSymbols">List of string you need to save.</param>
        private void Save(List<string> defineSymbols)
        {
            string defineSymbolsString = string.Join(";", defineSymbols);
            Save(defineSymbolsString);
        }

        /// <summary>
        /// Save define symbols to PlayerSettings.
        /// </summary>
        /// <param name="defineSymbols">Sting value rquest join string with ";" </param>
        private void Save(string defineSymbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(m_currentBuildTargetGroup, defineSymbols);
        }
    }
}