using UnityEngine;

namespace BoomerShooter.Interfaces
{
    public interface IInputReader
    {
        Vector2 MoveInput { get; }
        Vector2 LookInput { get; }
        bool IsJumpPressed { get; }
        bool IsWalking { get; }
        bool IsCrouchPressed { get; }
    }
}