using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EmployeeTraining.Employee;
using HarmonyLib;
using MyBox;
using UnityEngine;

namespace EmployeeTraining
{
    public static class ETSaveManager
    {
        private static readonly string SAVE_DIR = $"{Application.persistentDataPath}/EmployeeTraining";
        private const string TRAINING_DATA_KEY = "TrainingData-Rev2";

        private static readonly string SAVE_DIR_REV1 = $"{Application.persistentDataPath}/CashierTraining";
        private const string TRAINING_DATA_KEY_REV1 = "CashierSkills";

        public static bool IsReadyToSave = false;

        public static Action SaveDataLoadedEvent;

        private static string GetSaveFilePath(string gameFilePath)
        {
            var gameFileName = Path.GetFileName(gameFilePath);
            return $"{ETSaveManager.SAVE_DIR}/EmployeeTraining-{gameFileName}";
        }

        private static string GetSaveFilePathRev1(string gameFilePath)
        {
            var gameFileName = Path.GetFileName(gameFilePath);
            return $"{ETSaveManager.SAVE_DIR_REV1}/CashierTraining-{gameFileName}";
        }

        public static void Load(string gameFilePath, ES3Settings es3Settings)
        {
            if (gameFilePath == null || gameFilePath.Length == 0) return;

            try
            {
                var filePath = GetSaveFilePath(gameFilePath);
                Plugin.LogInfo($"Loading training data from {filePath}");
                if (File.Exists(filePath))
                {
                    ES3.CacheFile(filePath);
                    if (ES3.KeyExists(TRAINING_DATA_KEY, filePath, es3Settings))
                    {
                        Plugin.LogDebug($"Training data found");
                        ES3.LoadInto(TRAINING_DATA_KEY, filePath, Data, es3Settings);
                    }
                    if (Singleton<IDManager>.Instance)
                    {
                        SaveDataLoadedEvent.Invoke();
                    }
                }
                else
                {
                    // MIGRATION from CashierTrainingProgram
                    Plugin.LogInfo($"Training data NOT FOUND");
                    var filePathRev1 = GetSaveFilePathRev1(gameFilePath);
                    Plugin.LogInfo($"Migrating cashier skill data from {filePathRev1}");
                    if (File.Exists(filePathRev1))
                    {
                        // Create a temporary file...
                        string tempPath = $"{SAVE_DIR_REV1}/.migration";
                        string temp = File.ReadAllText(filePathRev1, Encoding.UTF8);
                        temp = temp.Replace(",CashierTrainingProgram", ",EmployeeTrainingProgram");
                        File.WriteAllText(tempPath, temp);
                        // And load it
                        ES3.CacheFile(tempPath);
                        if (ES3.KeyExists(TRAINING_DATA_KEY_REV1, tempPath, es3Settings))
                        {
                            var Rev1Data = new CashierTraining.CashiersData();
                            ES3.LoadInto(TRAINING_DATA_KEY_REV1, tempPath, Rev1Data, es3Settings);
                            Data = Rev1Data.Migrate();
                        }
                        if (Singleton<IDManager>.Instance)
                        {
                            Data.CashierSkills.ForEach(v => v.Skill.Setup());
                            Data.RestockerSkills.ForEach(v => v.Skill.Setup());
                        }
                        File.Delete(tempPath);
                        Plugin.LogInfo($"Cashier skill data has been successfully migrated");
                    }
                    else
                    {
                        Plugin.LogInfo($"Training data NOT FOUND");
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.LogError(e);
                Plugin.LogError($"Failed to load training data!");
            }
            finally
            {
                IsReadyToSave = true;
            }
        }

        public static void Save(string gameFilePath, ES3Settings es3Settings)
        {
            if (!IsReadyToSave) return;
            try
            {
                // BetterSaveSystem calls without filepath when the settings is saved in the title screen
                if (gameFilePath.Length > 0)
                {
                    Directory.CreateDirectory(ETSaveManager.SAVE_DIR);
                    var filePath = ETSaveManager.GetSaveFilePath(gameFilePath);
                    Plugin.LogInfo($"Saving training data to {filePath}");
                    ES3.Save(TRAINING_DATA_KEY, Data, filePath, es3Settings);
                    ES3.StoreCachedFile(filePath);
                    DeleteOldestSaveWhenMaxed();
                }
            }
            catch (Exception e)
            {
                Plugin.LogError("Failed to save training data!");
                Plugin.LogError(e);
            }
        }

        public static void Clear()
        {
            Plugin.LogDebug($"Clearing training data");
            if (ES3.KeyExists(TRAINING_DATA_KEY))
            {
                ES3.DeleteKey(TRAINING_DATA_KEY);
            }
            Data = new TrainingData();
        }

        private static void DeleteOldestSaveWhenMaxed()
        {
            int maxBackupCount = Traverse.Create(Singleton<SaveManager>.Instance)
                    .Field("m_MaxBackupCount").GetValue<int>();
            string[] array = Directory.GetFiles(ETSaveManager.SAVE_DIR, "*.es3");
            if (array.Length <= maxBackupCount)
            {
                return;
            }
            array = (from f in array
                     orderby new FileInfo(f).LastWriteTime
                     descending
                     select f).ToArray();
            string filePath = array.Last();
            if (ES3.FileExists(filePath))
            {
                ES3.DeleteFile(filePath);
            }
        }

        public static TrainingData Data { get; private set; } = new TrainingData();
    }

    [Serializable]
    public class TrainingData
    {
        public List<CashierSkillData> CashierSkills = new List<CashierSkillData>();

        public List<RestockerSkillData> RestockerSkills = new List<RestockerSkillData>();

        public List<CsHelperSkillData> CsHelperSkills = new List<CsHelperSkillData>();
    }
}
