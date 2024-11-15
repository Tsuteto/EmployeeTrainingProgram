using System.Linq;
using TMPro;
using UnityEngine;

namespace EmployeeTraining.Localization
{
    delegate object TranslateArgHandler();

    public class StringLocalizeTranslator : MonoBehaviour
    {
        public string Key;
        private object[] args = new object[]{};
        private TMP_Text tmpText;

        private void Awake()
        {
            // Plugin.LogDebug($"StringLocalizeTranslator.Awake: {this.name}");
            Plugin.Instance.LocaleChangedEvent += OnLocaleChanged;
            this.tmpText = this.GetComponent<TMP_Text>();
        }

        private void Start()
        {
            this.UpdateText();
        }

        public void Translate(params object[] args)
        {
            // Plugin.LogDebug($"Translate: {name} with args={args.ToJson()}");
            this.args = args;
            this.UpdateText();
        }

        public void UpdateText()
        {
            if (this.tmpText != null)
            {
                var args = this.args.Select(v => v.GetType() == typeof(TranslateArgHandler) ? ((TranslateArgHandler)v).Invoke() : v).ToArray();
                this.tmpText.text = Plugin.Localizer.Get(this.Key).Translate(args);
            }
        }

        private void OnLocaleChanged(string locale)
        {
            // Plugin.LogDebug("StringLocalizeTranslator.OnLocaleChanged");
            this.UpdateText();
        }

        private void OnDestroy()
        {
            Plugin.Instance.LocaleChangedEvent -= OnLocaleChanged;
        }
    }
}