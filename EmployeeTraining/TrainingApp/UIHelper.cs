using EmployeeTraining.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining.TrainingApp
{
    static class UIHelper
    {
        public static readonly Material FONT_MATERIAL = Utils.FindResourceByName<Material>("UptownBoy Atlas Material");
        public static readonly TMP_FontAsset FONT_ASSET = Utils.FindResourceByName<TMP_FontAsset>("UptownBoy SDF");

        public static readonly ColorBlock COLOR_BLOCK = new ColorBlock
        {
            colorMultiplier = 1,
            disabledColor = new Color(0.7843f, 0.7843f, 0.7843f, 0.5f),
            fadeDuration = 0.1f,
            highlightedColor = new Color(0.9608f, 0.9608f, 0.9608f, 1),
            normalColor = new Color(1, 1, 1, 1),
            pressedColor = new Color(0.7843f, 0.7843f, 0.7843f, 1),
            selectedColor = new Color(0.9608f, 0.9608f, 0.9608f, 1)
        };

        public static void SetupObject(this GameObject obj, Vector3 pos, Vector2? size = null, Vector2? pivot = null)
        {
            // Plugin.LogDebug($"Setting up object {obj.name}");
            obj.transform.eulerAngles = new Vector3(0, 0, 0);
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);

            var rect = obj.GetComponent<RectTransform>();
            if (rect != null)
            {
                if (size != null)
                {
                    rect.pivot = pivot ?? new Vector2(0, 1);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.Value.x);
                    rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.Value.y);
                    rect.anchoredPosition = new Vector2(0, 0);
                }
            }
            obj.transform.localPosition = pos;
        }

        public static void SetupText(this GameObject obj, Vector3 pos, Vector2 size,
            float fontsize, HorizontalAlignmentOptions align = HorizontalAlignmentOptions.Left,
            Color? color = null, Vector2? pivot = null, string key = null, string[] args = null)
        {
            // Plugin.LogDebug($"Setting up text field on {obj.name}");
            obj.SetupObject(pos, size, pivot);

            var tmpText = obj.GetComponent<TextMeshProUGUI>();
            tmpText.fontSize = fontsize;
            tmpText.fontSizeMax = fontsize;
            tmpText.fontSizeMin = 0;
            tmpText.color = color ?? new Color(1, 1, 1);
            tmpText.horizontalAlignment = align;
            tmpText.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmpText.fontMaterial = FONT_MATERIAL;
            tmpText.font = FONT_ASSET;
            tmpText.autoSizeTextContainer = false;
            tmpText.enableAutoSizing = true;
            tmpText.enableWordWrapping = false;

            if (key != null)
            {
                var trans = obj.GetComponent<StringLocalizeTranslator>();
                trans.Key = key;
                if (args != null) trans.Translate(args);
            }
        }

        public static void SetRawText(this GameObject baseObj, string name, string text)
        {
            var obj = baseObj.transform.Find(name).gameObject;
            obj.GetComponent<TextMeshProUGUI>().text = text;
        }

    }
}