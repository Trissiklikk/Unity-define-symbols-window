using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Trissiklikk.EditorTools.DefineSymbolsEditor
{
    public sealed class LocalFileSaveHandle : BaseSaveHandle
    {
        private const string FOLDER_NAME = "DefineSymbols";

        public override void Save(List<DefineSymbolsData> defineSymbolsData)
        {
            if (!CheckFolderExist())
            {
                CreateFile();
            }

            SetFileReadPermission(GetSavePath(), false);

            StreamWriter writer = new StreamWriter(GetSavePath());

            for (int i = 0; i < defineSymbolsData.Count; i++)
            {
                string json = JsonConvert.SerializeObject(defineSymbolsData[i]);
                writer.WriteLine(json);
            }

            writer.Close();

            SetFileReadPermission(GetSavePath(), true);
        }

        public override List<DefineSymbolsData> Load()
        {
            List<DefineSymbolsData> result = new List<DefineSymbolsData>();

            if (!CheckFolderExist())
            {
                CreateFile();
                return result;
            }

            StreamReader reader = new StreamReader(GetSavePath());
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                DefineSymbolsData data = JsonConvert.DeserializeObject<DefineSymbolsData>(line);
                result.Add(data);
            }

            reader.Close();

            return result;
        }

        private void CreateFile()
        {
            string folderPath = string.Format("{0}/{1}", Application.dataPath, FOLDER_NAME);
            Directory.CreateDirectory(folderPath);

            string savePath = GetSavePath();
            File.Create(savePath).Close();
        }

        private bool CheckFolderExist()
        {
            string folderPath = string.Format("{0}/{1}", Application.dataPath, FOLDER_NAME);

            return Directory.Exists(folderPath);
        }

        private string GetSavePath()
        {
            string savePath = string.Format("{0}/{1}/{2}.txt", Application.dataPath, FOLDER_NAME, GetSaveKey());

            return savePath;
        }

        private void SetFileReadPermission(string path, bool isReadOnly)
        {
            FileInfo fileInfo = new FileInfo(path);
            fileInfo.IsReadOnly = isReadOnly;
        }
    }
}