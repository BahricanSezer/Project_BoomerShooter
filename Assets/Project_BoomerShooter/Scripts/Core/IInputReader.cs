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
        bool IsFirePressed { get; }
        bool IsFireHeld { get; }
        bool IsSpinPressed { get; }
        float SwitchWeaponInput { get; }
    }
}