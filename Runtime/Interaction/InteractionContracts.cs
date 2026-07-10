using UnityEngine;

namespace GameKit.Interaction
{
    public interface IInteractable
    {
        void Interact();
    }

    public interface ICollectible
    {
        string Id { get; }
        void Collect();
    }

    public interface IChannelable
    {
        void StartChannel();
        void StopChannel();
    }

    public interface ILinkable
    {
        string LinkId { get; }
        string UniqueId { get; }
    }

    public interface IHidable
    {
        bool IsAvailable { get; }
        bool IsPlayerHiding { get; }
        float CooldownRemaining { get; }
        void EnterHiding(GameObject player);
        void ExitHiding();
    }

    public interface ISaveable
    {
        string SaveKey { get; }
        object CaptureState();
        void RestoreState(object state);
    }
}
