using Quantum;
using UnityEngine;

namespace Quantum2D.UnityView
{
    /// <summary>
    /// Đồng bộ trực tiếp dữ liệu từ Quantum Simulation sang Sprite và Animation của Unity View
    /// </summary>
    public unsafe class QuantumCharacterEntityView : QuantumEntityView
    {
        [Header("Animation Controller")]
        [SerializeField] private CharacterAnimationController2D animController;
        [SerializeField] private CharacterVFXSpawner2D vfxSpawner;

        public override void OnInitialize()
        {
            base.OnInitialize();
            if (animController == null) animController = GetComponentInChildren<CharacterAnimationController2D>();
            if (vfxSpawner == null) vfxSpawner = GetComponentInChildren<CharacterVFXSpawner2D>();

            // Lắng nghe các sự kiện Attack, Dash từ Quantum Simulation
            QuantumEvent.Subscribe(listener: this, handler: (EventEventPlayerAttack e) => OnAttackEvent(e));
            QuantumEvent.Subscribe(listener: this, handler: (EventEventPlayerDash e) => OnDashEvent(e));
        }

        public override void OnUpdateView()
        {
            var frame = PredictedFrame;
            if (frame == null || !frame.Exists(EntityRef)) return;

            if (frame.Unsafe.TryGetPointer<PlayerCharacter>(EntityRef, out var playerChar))
            {
                // Cập nhật hướng quay mặt
                Vector2 dir = playerChar->FacingDirection.ToVector2();
                if (animController != null)
                {
                    animController.SetFacingDirection(dir);

                    // Đồng bộ trạng thái Animation từ ECS
                    switch (playerChar->CurrentState)
                    {
                        case ActionState.Idle:
                            animController.SetState(CharacterAnimState.Idle);
                            break;
                        case ActionState.Run:
                            animController.SetState(CharacterAnimState.Run);
                            break;
                        case ActionState.AttackCombo1:
                        case ActionState.AttackCombo2:
                        case ActionState.AttackCombo3:
                            animController.SetState(CharacterAnimState.Attack);
                            break;
                        case ActionState.Dash:
                            animController.SetState(CharacterAnimState.Dash);
                            break;
                        case ActionState.Hurt:
                            animController.SetState(CharacterAnimState.Hurt);
                            break;
                        case ActionState.Downed:
                            animController.SetState(CharacterAnimState.Die);
                            break;
                    }
                }
            }
        }

        private void OnAttackEvent(EventEventPlayerAttack e)
        {
            if (e.Player == EntityRef && vfxSpawner != null)
            {
                vfxSpawner.SpawnAttackVfx(e.ComboStep, transform.position, e.Direction.ToVector2(), e.Direction.X < 0);
            }
        }

        private void OnDashEvent(EventEventPlayerDash e)
        {
            if (e.Player == EntityRef && vfxSpawner != null)
            {
                vfxSpawner.SpawnDashTrail(transform.position, (e.EndPos - e.StartPos).ToVector2());
            }
        }
    }
}
