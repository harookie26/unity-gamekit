using UnityEngine;
using UnityEngine.UI;

namespace GameKit.UI
{
    public sealed class RuntimeUiFactory
    {
        private readonly Font font;
        private readonly Color outlineColor;
        private readonly Color highlightAccent;
        private readonly Color pressedAccent;

        public RuntimeUiFactory(Font font)
            : this(font, new Color(.08f, .12f, .10f, .18f), new Color(.91f, .72f, .29f), new Color(.16f, .38f, .29f))
        {
        }

        public RuntimeUiFactory(Font font, Color outlineColor, Color highlightAccent, Color pressedAccent)
        {
            this.font = font;
            this.outlineColor = outlineColor;
            this.highlightAccent = highlightAccent;
            this.pressedAccent = pressedAccent;
        }

        public Image Panel(Transform parent, string name, Rect rect, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            SetRect(go.GetComponent<RectTransform>(), rect);

            Image image = go.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;

            if (color.a > 0.01f)
            {
                Outline outline = go.AddComponent<Outline>();
                outline.effectColor = outlineColor;
                outline.effectDistance = new Vector2(1f, -1f);
            }

            return image;
        }

        public Button Button(RectTransform parent, string name, string label, Rect rect, Color color, Color textColor, int textSize)
        {
            Image image = Panel(parent, name, rect, color);
            image.raycastTarget = true;

            Button button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            ColorBlock colors = button.colors;
            colors.highlightedColor = Color.Lerp(color, highlightAccent, .38f);
            colors.pressedColor = Color.Lerp(color, pressedAccent, .42f);
            colors.selectedColor = colors.highlightedColor;
            colors.fadeDuration = .08f;
            button.colors = colors;

            if (!string.IsNullOrEmpty(label))
            {
                Text(button.transform, "Label", label, R(0, 0, rect.width, rect.height), textSize, textColor, TextAnchor.MiddleCenter, FontStyle.Bold);
            }

            return button;
        }

        public Text Text(Transform parent, string name, string value, Rect rect, int size, Color color, TextAnchor anchor, FontStyle style = FontStyle.Normal)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            go.transform.SetParent(parent, false);
            SetRect(go.GetComponent<RectTransform>(), rect);

            Text text = go.GetComponent<Text>();
            text.text = value;
            text.font = font;
            text.fontSize = size;
            text.color = color;
            text.alignment = anchor;
            text.fontStyle = style;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return text;
        }

        public static Rect R(float x, float y, float width, float height) => new Rect(x, y, width, height);

        private static void SetRect(RectTransform rectTransform, Rect rect)
        {
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(rect.x, -rect.y);
            rectTransform.sizeDelta = new Vector2(rect.width, rect.height);
        }
    }
}
