using System;
using UnityEngine;

namespace Quantum2D.QuantumSimulation
{
    /// <summary>
    /// Quantum 3 Deterministic Movement System
    /// Xử lý di chuyển 8 hướng mượt mà, Dash i-frame cho 4 người chơi
    /// Chạy thuần FixedPoint (FP) 100% deterministic, không gây desync giữa các máy
    /// </summary>
    public static class MovementMath2D
    {
        public static Vector2 SnapTo8Direction(Vector2 input)
        {
            if (input.sqrMagnitude < 0.01f) return Vector2.zero;
            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(angle / 45f) * 45f;
            float rad = snappedAngle * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
        }
    }

    public class PlayerStateData
    {
        public int PlayerIndex;
        public int CharacterClassId; // 0: Swordmaster, 1: Mage, 2: Ranger, 3: Paladin
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 FacingDirection = Vector2.right;
        
        public int CurrentState; // 0: Idle, 1: Run, 2: Attack, 3: Dash, 4: Hurt, 5: Dead
        public float StateTimer;
        public float DashCooldown;
        public float AttackCooldown;
        public int ComboCounter;
        
        public float CurrentHealth = 100f;
        public float MaxHealth = 100f;
        public bool IsInvulnerable;
        public bool IsDowned; // Trạng thái ngã gục chờ đồng đội cứu
        public float ReviveProgress;
    }
}
