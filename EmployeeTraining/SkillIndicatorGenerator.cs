using EmployeeTraining.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EmployeeTraining
{
    public static class SkillIndicatorGenerator
    {
        public static GameObject SkillIndicatorTmpl { get; private set; }

        public static void Generate()
        {
            // Plugin.LogDebug("Composing Skill Indicator");
            var tmplObj = new GameObject("Skill Indicator", typeof(SkillIndicator), typeof(Canvas));

            var sliderObj = new GameObject("Exp Slider", typeof(Slider));
            sliderObj.transform.SetParent(tmplObj.transform);
            sliderObj.transform.localPosition = new Vector3(0, 0, 0);

            var bgObj = new GameObject("Background", typeof(CanvasRenderer), typeof(Image));
            bgObj.transform.SetParent(sliderObj.transform);

            var fillAreaObj = new GameObject("Fill Area", typeof(RectTransform));
            fillAreaObj.transform.SetParent(sliderObj.transform);

            var fillObj = new GameObject("Fill", typeof(CanvasRenderer), typeof(Image));
            fillObj.transform.SetParent(fillAreaObj.transform);

            var lvlTextObj = new GameObject("Lvl Text", typeof(TextMeshProUGUI), typeof(StringLocalizeTranslator)).gameObject;
            lvlTextObj.transform.SetParent(tmplObj.transform);
            lvlTextObj.transform.localPosition = new Vector3(0, 0, 0);
            lvlTextObj.transform.localScale = new Vector3(1, 1, 1);

            var expTextObj = new GameObject("Exp Text", typeof(TextMeshProUGUI)).gameObject;
            expTextObj.transform.SetParent(tmplObj.transform);
            expTextObj.transform.localPosition = new Vector3(0, 0, 0);
            expTextObj.transform.localScale = new Vector3(1, 1, 1);

            Vector2 sliderPivot = new Vector2(0.35f, 0.5f);
            Vector2 sliderSize = new Vector2(0.4f, 0.1f);

            var canvas = tmplObj.GetComponent<Canvas>();
            canvas.additionalShaderChannels = AdditionalCanvasShaderChannels.TexCoord1 | AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.Tangent;

            var indRect = tmplObj.GetComponent<RectTransform>();
            indRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1.0f);
            indRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.15f);

            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderSize.x);
            bgRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sliderSize.y);
            bgRect.pivot = sliderPivot;

            var fillAreaRect = fillAreaObj.GetComponent<RectTransform>();
            fillAreaRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sliderSize.x);
            fillAreaRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, sliderSize.y);
            fillAreaRect.pivot = sliderPivot;

            var fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, -0.02f);
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, -0.02f);

            var bgImg = bgObj.GetComponent<Image>();
            bgImg.color = new Color(0.8915f, 0.9955f, 1f, 0.25f);

            var fillImg = fillObj.GetComponent<Image>();
            fillImg.color = new Color(0.87f, 0.9f, 0.9f, 1f);

            var slider = sliderObj.GetComponent<Slider>();
            slider.fillRect = fillObj.GetComponent<RectTransform>();
            slider.interactable = false;
            slider.transition = Selectable.Transition.None;

            var sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            sliderRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);

            var lvlText = lvlTextObj.GetComponent<TextMeshProUGUI>();
            lvlText.fontSize = 0.1f;
            lvlText.fontSizeMax = 0.1f;
            lvlText.fontSizeMin = 0;
            lvlText.color = new Color(1, 1, 1);
            lvlText.horizontalAlignment = HorizontalAlignmentOptions.Right;
            lvlText.verticalAlignment = VerticalAlignmentOptions.Middle;
            lvlText.fontMaterial = UIHelper.FONT_MATERIAL;
            lvlText.font = UIHelper.FONT_ASSET;
            // NOTE: autoSizeTextContainer will automatically fix the size=(0, 0)
            lvlText.autoSizeTextContainer = true;
            lvlText.enableAutoSizing = false;
            lvlText.enableWordWrapping = false;
            var lvlTrans = lvlTextObj.GetComponent<StringLocalizeTranslator>();
            lvlTrans.Key = "Lvl";
            lvlTrans.Translate("<LVL>");

            var lvlTextRect = lvlTextObj.GetComponent<RectTransform>();
            lvlTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            lvlTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            lvlTextRect.pivot = new Vector2(0.5f, 0.5f);
            lvlTextRect.anchoredPosition = new Vector2(-0.18f, 0);

            TextMeshProUGUI expText = expTextObj.GetComponent<TextMeshProUGUI>();
            expText.fontMaterial = UIHelper.FONT_MATERIAL;
            expText.font = UIHelper.FONT_ASSET;
            expText.fontSize = 0.07f;
            expText.fontSizeMax = 0.07f;
            expText.fontSizeMin = 0;
            expText.color = new Color(0.7f, 0.72f, 0.72f);
            expText.outlineColor = new Color32(255, 255, 255, 192);
            expText.outlineWidth = 0.25f;
            expText.horizontalAlignment = HorizontalAlignmentOptions.Left;
            expText.verticalAlignment = VerticalAlignmentOptions.Middle;
            expText.autoSizeTextContainer = true;
            expText.enableAutoSizing = false;
            expText.enableWordWrapping = false;
            expText.materialForRendering.SetFloat("_FaceDilate", 0.25f);
            expText.UpdateFontAsset();

            var expTextRect = expTextObj.GetComponent<RectTransform>();
            expTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0f);
            expTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0f);
            expTextRect.pivot = new Vector2(0.5f, 0.5f);
            expTextRect.anchoredPosition = new Vector2(-0.125f, -0.01f);

            tmplObj.SetActive(false);
            SkillIndicatorTmpl = tmplObj;

            // Plugin.LogDebug("Finished composing Skill Indicator");
        }

        public static void Dispose()
        {
            SkillIndicatorTmpl = null;
        }
    }
}