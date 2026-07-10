using UnityEngine;

namespace GameKit.Interaction
{
    public interface IRoom
    {
        int Id { get; }
        Vector3 Center { get; }
        Bounds Bounds { get; }
    }

    public interface IStair
    {
        int Id { get; }
        IRoom RoomA { get; }
        IRoom RoomB { get; }
        float Weight { get; }
    }
}
