using System;
using BepInEx;
using EmployeeTraining.EmployeeCashier;
using EmployeeTraining.EmployeeCsHelper;
using EmployeeTraining.EmployeeJanitor;
using EmployeeTraining.EmployeeRestocker;
using EmployeeTraining.EmployeeSecurity;
using EmployeeTraining.Localization;
using EmployeeTraining.TrainingApp;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace EmployeeTraining
{
    [BepInPlugin("jp.tsuteto.sms.EmployeeTrainingProgram", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; }
        public static StringLocalizer Localizer { get; private set; }
        // public static bool IsBetterSaveSystemInstalled { get; private set; }

        public Settings Settings { get; private set; }
        
        public Action GameLoadedEvent;
        public Action GameQuitEvent;
        public Action<string> LocaleChangedEvent;

        private Plugin()
        {
            Plugin.Instance = this;
        }

        private void Awake()
        {
            this.Settings = new Settings(base.Config);
            var serializer = new LocalizationSerializer(this);
            Plugin.Localizer = serializer.Load();

            // Plugin.IsBetterSaveSystemInstalled = Type.GetType("BetterSaveSystem.BetterSaveSystemMod,BetterSaveSystem") != null;
            // Logger.LogDebug($"BetterSaveSystem {(IsBetterSaveSystemInstalled ? "installed" : "not installed")}");

            GamePatch.plugin = this;

            Logger.LogInfo("Applying EmployeeTraining patches");
            Harmony harmony = new Harmony("jp.tsuteto.sms.EmployeeTraining");
            harmony.PatchAll();

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += this.OnSceneWasLoaded;

            LocalizationSettings.SelectedLocaleChanged += this.OnLocaleChanged;
            Locale locale = LocalizationSettings.SelectedLocale;
            Plugin.Localizer.Lang = locale.Identifier.Code;

            SkillIndicatorGenerator.Init();
            CsHelperLogic.Init();
        }

        private void Update()
        {
        }

        private void OnLocaleChanged(Locale newLocale)
		{
			// Plugin.LogDebug($"Changed locale: {newLocale.Identifier.Code}");
            Plugin.Localizer.Lang = newLocale.Identifier.Code;
            this.LocaleChangedEvent?.Invoke(newLocale.Identifier.Code);
        }

        public void OnSceneWasLoaded(Scene activeScene, Scene nextScene)
        {
            if (nextScene.name == "Main Menu")
            {
                this.GameQuitEvent?.Invoke();
                ETSaveManager.IsReadyToSave = false;
            }
            if (nextScene.name == "Main Scene")
            {
                var customerList = new GameObject("Shopping Customer List", typeof(ShoppingCustomerList));
                customerList.transform.SetParent(GameObject.Find("---MANAGERS---").transform);

                var appObj = new GameObject("Training App", typeof(PCTrainingApp));
                appObj.transform.SetParent(GameObject.Find("---MANAGERS---").transform);

                this.GameLoadedEvent?.Invoke();

                // Output all Product info
                // var products = Singleton<IDManager>.Instance.Products;
                // products.ForEach(p => {
                //     LogDebug($"{p.ID}: name={p.name}, prodsInBox={p.GridLayoutInBox.productCount}, prodsInDisplay={p.GridLayoutInStorage.productCount}, productName={p.ProductBrand} {p.ProductName}");
                // });

                // products.Where(x => x is WeightedProductSO).ForEach(p => {
                //     LogDebug($"{p.ID}: name={p.name}, weight={(p as WeightedProductSO).Weight}, Box={p.GridLayoutInBox.boxSize}");
                // });
            }
        }

        public static void LogDebug(object data)
        {
            Instance.Logger?.LogDebug(data);
        }

        public static void LogInfo(object data)
        {
            Instance.Logger?.LogInfo(data);
        }

        public static void LogWarn(object data)
        {
            Instance.Logger?.LogWarning(data);
        }

        public static void LogError(object data)
        {
            Instance.Logger?.LogError(data);
        }
    }
}
