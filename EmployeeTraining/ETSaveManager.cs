using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using EmployeeTraining.EmployeeCashier;
using EmployeeTraining.EmployeeCsHelper;
using EmployeeTraining.EmployeeJanitor;
using EmployeeTraining.EmployeeRestocker;
using EmployeeTraining.EmployeeSecurity;
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

        private static readonly ES3Settings es3Settings = new ES3Settings(new Enum[]{ES3.Location.Cache});

        public static bool IsReadyToSave = false;

        public static Action SaveDataLoadedEvent;

        public static void CreateSaveDirectory()
        {
            if (!Directory.Exists(SAVE_DIR))
            {
                Plugin.LogInfo($"Creating savedata directory: {SAVE_DIR}");
                Directory.CreateDirectory(SAVE_DIR);
            }
        }

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

        public static void Load(string gameFilePath, int tries = -1)
        {
            if (string.IsNullOrEmpty(gameFilePath)) return;

            bool loaded = false;
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
                        ETSaveManager.SaveDataLoadedEvent?.Invoke();
                    }
                    loaded = true;
                }
                else
                {
                    // MIGRATION from CashierTrainingProgram
                    Plugin.LogInfo($"Training data NOT FOUND");
                    var filePathRev1 = GetSaveFilePathRev1(filePath);
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
                        loaded = true;
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
                if (!loaded)
                {
                    if (tries == -1) Plugin.LogWarn("Trying to load from backups");
                    tries += 1;
                    if (GetNextFileToLoad(tries, out string nextFile))
                    {
                        Load(nextFile, tries);
                    }
                }
                IsReadyToSave = true;
            }
        }

        private static bool GetNextFileToLoad(int failed, out string nextFile)
        {
            string[] files = Directory.GetFiles(ETSaveManager.SAVE_DIR, "*.es3");
            files = files.OrderByDescending(f => new FileInfo(f).LastWriteTime).ToArray();
            if (failed < files.Length)
            {
                nextFile = Regex.Replace(files[failed], @"\\(?:Employee|Cashier)Training-(.+\.es3)$", @"\\$1");
                return true;
            }
            else
            {
                nextFile = null;
                return false;
            }
        }

        public static void Save(string gameFilePath)
        {
            if (!IsReadyToSave) return;
            try
            {
                // BetterSaveSystem calls without filepath when the settings is saved in the title screen
                if (gameFilePath.Length > 0)
                {
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

        public List<JanitorSkillData> JanitorSkills = new List<JanitorSkillData>();

        public List<SecuritySkillData> SecuritySkills = new List<SecuritySkillData>();
    }
}
