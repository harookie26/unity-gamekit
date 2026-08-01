using System;
using System.Collections;
using System.Collections.Generic;
using GameKit.Events;
using UnityEngine;

namespace GameKit.Dialogue
{
    public enum DialogueEventValueType
    {
        String,
        Integer,
        Float,
        Boolean
    }

    [Serializable]
    public sealed class DialogueEventValue
    {
        public string key = "";
        public DialogueEventValueType type;
        public string stringValue = "";
        public int integerValue;
        public float floatValue;
        public bool booleanValue;

        internal void AddTo(EventPayload payload)
        {
            if (payload == null || string.IsNullOrWhiteSpace(key))
                return;

            switch (type)
            {
                case DialogueEventValueType.String:
                    payload.Set(key, stringValue);
                    break;
                case DialogueEventValueType.Integer:
                    payload.Set(key, integerValue);
                    break;
                case DialogueEventValueType.Float:
                    payload.Set(key, floatValue);
                    break;
                case DialogueEventValueType.Boolean:
                    payload.Set(key, booleanValue);
                    break;
            }
        }
    }

    [Serializable]
    public sealed class DialogueEvent
    {
        [Tooltip("Name published through GlobalEventBus.")]
        public string eventName = "";
        [Tooltip("Optional typed values included with the event payload.")]
        public List<DialogueEventValue> values = new();
    }

    [CreateAssetMenu(fileName = "DialogueEventOutcome", menuName = "GameKit/Dialogue/Outcomes/Publish Events")]
    public sealed class DialogueEventOutcome : DialogueOutcome
    {
        [Tooltip("All events are published in list order when this outcome runs.")]
        [SerializeField] private List<DialogueEvent> events = new();

        public override IEnumerator Execute(DialogueOutcomeContext context)
        {
            if (events == null)
                yield break;

            foreach (DialogueEvent dialogueEvent in events)
            {
                if (dialogueEvent == null || string.IsNullOrWhiteSpace(dialogueEvent.eventName))
                    continue;

                EventPayload payload = new();
                if (dialogueEvent.values != null)
                {
                    foreach (DialogueEventValue value in dialogueEvent.values)
                        value?.AddTo(payload);
                }

                payload.Set("responseIndex", context != null ? context.ResponseIndex : -1);
                payload.Set("responseText", context?.Response?.text ?? "");
                GlobalEventBus.Instance.Publish(dialogueEvent.eventName, payload);
            }

            yield break;
        }
    }
}
