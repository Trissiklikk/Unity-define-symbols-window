using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public sealed class DefineSymbolsEditor : EditorWindow
    {
        private static BuildTargetGroup m_currentBuildTargetGroup = BuildTargetGroup.Standalone;
        private static SaveType m_targeSaveType = SaveType.LocalFile;

        private BaseSaveHandle m_defineSymbolsSaveHandle;
        private List<DefineSymbolsData> m_editorDefineSymbols = new List<DefineSymbolsData>();
        private string m_addDefineSymbol = string.Empty;
        private Vector2 m_scrollPosition;
        private SaveType m_currentSaveType;

        [MenuItem("Window/Trissiklikk Editor Tools/Define Symbols Window %#F2")]
        public static void ShowWindow()
        {
            GetWindow(typeof(DefineSymbolsEditor));
        }

        private void OnEnable()
        {
            Refresh();
        }

        private void OnInspectorUpdate()
        {
            if (m_currentSaveType != m_targeSaveType)
            {
                Refresh();
            }
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Target Device");
                m_currentBuildTargetGroup = (BuildTargetGroup)EditorGUILayout.EnumPopup(m_currentBuildTargetGroup);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Save Type");
                m_targeSaveType = (SaveType)EditorGUILayout.EnumPopup(m_targeSaveType);
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(m_scrollPosition, new GUIStyle() { margin = new RectOffset(10, 10, 20, 20) }))
            {
                m_scrollPosition = scrollView.scrollPosition;

                for (int i = 0; i < m_editorDefineSymbols.Count; i++)
                {
                    DefineSymbolsData defineSymbol = m_editorDefineSymbols[i];

                    if (defineSymbol == null)
                    {
                        continue;
                    }

                    Color textColor = defineSymbol.IsEnabled ? Color.green : Color.gray;

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(defineSymbol.SymbolName, new GUIStyle()
                        {
                            fontStyle = FontStyle.Bold,
                            fontSize = 12,
                            normal = new GUIStyleState()
                            {
                                textColor = textColor
                            }
                        });

                        defineSymbol.IsEnabled = EditorGUILayout.Toggle(m_editorDefineSymbols[i].IsEnabled);

                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("x"))
                        {
                            Remove(defineSymbol.SymbolName);
                        }
                    }

                    GUILayout.Space(3);
                }
            }

            GUILayout.FlexibleSpace();

            using (new EditorGUILayout.HorizontalScope())
            {
                m_addDefineSymbol = EditorGUILayout.TextField(m_addDefineSymbol);

                if (GUILayout.Button("Add"))
                {
                    Add();
                }

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Clear"))
                {
                    ClearAddDefineSymbol();
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
                    Apply();
                }
            }

            GUILayout.Space(10);
        }

        /// <summary>
        /// Apply the data to save process.
        /// </summary>
        private void Apply()
        {
            VerifiedSaveType();
            m_defineSymbolsSaveHandle.Save(m_editorDefineSymbols);
            m_defineSymbolsSaveHandle.ApplyDataToProject(m_currentBuildTargetGroup, GetEnableData());
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

            if (m_editorDefineSymbols.Exists(x => x.SymbolName == m_addDefineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol already exists", "OK");
                return;
            }

            DefineSymbolsData defineSymbolsData = new DefineSymbolsData()
            {
                SymbolName = m_addDefineSymbol,
                IsEnabled = true
            };

            m_editorDefineSymbols.Add(defineSymbolsData);

            ClearAddDefineSymbol();
        }

        /// <summary>
        /// Remove define symbol from list.
        /// </summary>
        /// <param name="defineSymbol">String value of symbol you need to remove.</param>
        private bool Remove(string defineSymbol)
        {
            if (string.IsNullOrEmpty(defineSymbol))
            {
                EditorUtility.DisplayDialog("Error", "Define symbol cannot be empty", "OK");
                return false;
            }

            bool isAllowed = EditorUtility.DisplayDialog("Warning", "Are you sure you want to remove this define symbol? You need to hit 'Apply' button again when you remove this form list. ", "Yes", "No");

            if (!isAllowed)
            {
                return false;
            }

            DefineSymbolsData defineSymbolsData = m_editorDefineSymbols.Find(x => x.SymbolName == defineSymbol);

            bool isRemoved = m_editorDefineSymbols.Remove(defineSymbolsData);

            if (!isRemoved)
            {
                EditorUtility.DisplayDialog("Error", "Define symbol does not exist", "OK");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Refresh current define symbols list.
        /// </summary>
        private void Refresh()
        {
            m_currentSaveType = m_targeSaveType;

            VerifiedSaveType();

            m_editorDefineSymbols.Clear();
            m_editorDefineSymbols = m_defineSymbolsSaveHandle.Load();
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
        /// This method returns enabled define symbols.
        /// </summary>
        /// <returns></returns>
        private List<DefineSymbolsData> GetEnableData()
        {
            List<DefineSymbolsData> enableData = new List<DefineSymbolsData>();

            for (int i = 0; i < m_editorDefineSymbols.Count; i++)
            {
                if (m_editorDefineSymbols[i].IsEnabled)
                {
                    enableData.Add(m_editorDefineSymbols[i]);
                }
            }

            return enableData;
        }

        private void VerifiedSaveType()
        {
            switch (m_targeSaveType)
            {
                case SaveType.LocalFile:

                    if (m_defineSymbolsSaveHandle is LocalFileSaveHandle)
                    {
                        return;
                    }

                    m_defineSymbolsSaveHandle = new LocalFileSaveHandle();
                    break;

                case SaveType.EditorPrefs:

                    if (m_defineSymbolsSaveHandle is EditorPrefsSaveHandle)
                    {
                        return;
                    }

                    m_defineSymbolsSaveHandle = new EditorPrefsSaveHandle();
                    break;

                default:

                    throw new System.Exception("Save type not found.");
            }
        }
    }
}