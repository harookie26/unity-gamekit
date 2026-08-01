using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameKit.Dialogue
{
    public class DialogueChoicePresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform choiceContainer;
        [SerializeField] private Button choiceButtonPrefab;
        [SerializeField] private bool selectFirstChoice = true;

        private readonly List<Button> buttons = new();

        public virtual int PresentedChoiceCount => buttons.Count;

        protected virtual void Awake()
        {
            if (choiceButtonPrefab != null && choiceButtonPrefab.gameObject.scene.IsValid())
                choiceButtonPrefab.gameObject.SetActive(false);

            Hide();
        }

        public virtual bool CanPresent => choiceContainer != null && choiceButtonPrefab != null;

        public virtual void Show(IReadOnlyList<DialogueResponse> responses, Action<int> onSelected)
        {
            ClearButtons();

            if (!CanPresent || responses == null)
                return;

            gameObject.SetActive(true);
            choiceContainer.gameObject.SetActive(true);

            for (int i = 0; i < responses.Count; i++)
            {
                DialogueResponse response = responses[i];
                if (response == null)
                    continue;

                int responseIndex = i;
                Button button = Instantiate(choiceButtonPrefab, choiceContainer);
                button.gameObject.SetActive(true);
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onSelected?.Invoke(responseIndex));

                TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
                if (label != null)
                    label.text = response.text ?? "";

                buttons.Add(button);
            }

            if (selectFirstChoice && buttons.Count > 0 && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
        }

        public virtual void Hide()
        {
            ClearButtons();
            if (choiceContainer != null)
                choiceContainer.gameObject.SetActive(false);
        }

        private void ClearButtons()
        {
            foreach (Button button in buttons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }

            buttons.Clear();
        }
    }
}
