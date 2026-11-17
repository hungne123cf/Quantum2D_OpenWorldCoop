using Quantum;
using UnityEngine;

namespace Quantum2D.UnityView
{
    /// <summary>
    /// Đồng bộ trực tiếp dữ liệu từ Quantum Simulation sang Sprite và Animation của Unity View
    /// </summary>
    public class QuantumCharacterEntityView : QuantumEntityView
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
            if (Game == null || Game.Frames.Predicted == null) return;
            var frame = Game.Frames.Predicted;
            if (!frame.Exists(EntityRef)) return;

            // Đọc component PlayerCharacter từ ECS Frame
            if (frame.TryGet<PlayerCharacter>(EntityRef, out var playerChar))
            {
                // Cập nhật hướng quay mặt
                Vector2 dir = playerChar.FacingDirection.ToUnityVector2();
                if (animController != null)
                {
                    animController.SetFacingDirection(dir);

                    // Đồng bộ trạng thái Animation từ ECS
                    switch (playerChar.CurrentState)
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
                Vector2 dir = e.Direction.ToUnityVector2();
                vfxSpawner.SpawnAttackVfx(e.ComboStep, transform.position, dir, dir.x < 0);
            }
        }

        private void OnDashEvent(EventEventPlayerDash e)
        {
            if (e.Player == EntityRef && vfxSpawner != null)
            {
                Vector2 delta = (e.EndPos - e.StartPos).ToUnityVector2();
                vfxSpawner.SpawnDashTrail(transform.position, delta);
            }
        }
    }
}
