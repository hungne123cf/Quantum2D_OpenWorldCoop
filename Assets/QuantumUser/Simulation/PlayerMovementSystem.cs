using Photon.Deterministic;

namespace Quantum
{
    /// <summary>
    /// Quantum 3 Deterministic Movement System
    /// Xử lý di chuyển 8 hướng mượt mà, Dash i-frame cho 4 người chơi
    /// Chạy thuần FixedPoint (FP) 100% deterministic, không gây desync giữa các máy
    /// </summary>
    public unsafe class PlayerMovementSystem : SystemMainThreadFilter<PlayerMovementSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform2D* Transform;
            public PlayerCharacter* Character;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            // Nếu người chơi đang gục ngã (Downed) thì không thể di chuyển
            if (filter.Character->CurrentState == ActionState.Downed)
            {
                return;
            }

            var input = frame.GetPlayerInput(filter.Character->Player);
            if (input == null) return;

            // Xử lý Cooldown của Dash và Attack
            if (filter.Character->DashCooldown > FP._0)
            {
                filter.Character->DashCooldown -= frame.DeltaTime;
            }

            if (filter.Character->AttackCooldown > FP._0)
            {
                filter.Character->AttackCooldown -= frame.DeltaTime;
            }

            // Xử lý Dash (Né chiêu + Bất tử i-frame trong 0.25 giây)
            if (input->Dash.WasPressed && filter.Character->DashCooldown <= FP._0 && filter.Character->CurrentState != ActionState.Dash)
            {
                filter.Character->CurrentState = ActionState.Dash;
                filter.Character->StateTimer = FP._0_25; // 0.25s lướt
                filter.Character->DashCooldown = FP._1; // 1s cooldown
                filter.Character->IsInvulnerable = true;

                // Bắn Event cho Unity View kích hoạt vệt Dash Afterimage / VFX
                frame.Events.EventPlayerDash(filter.Entity, filter.Character->ClassType, filter.Transform->Position, filter.Transform->Position + filter.Character->FacingDirection * FP._3);
                return;
            }

            // Nếu đang trong trạng thái Dash
            if (filter.Character->CurrentState == ActionState.Dash)
            {
                filter.Character->StateTimer -= frame.DeltaTime;
                filter.Transform->Position += filter.Character->FacingDirection * (FP._10 + FP._2) * frame.DeltaTime;

                if (filter.Character->StateTimer <= FP._0)
                {
                    filter.Character->CurrentState = ActionState.Idle;
                    filter.Character->IsInvulnerable = false;
                }
                return;
            }

            // Di chuyển 8 hướng cơ bản
            FPVector2 moveDir = input->MoveDirection;
            if (moveDir.SqrMagnitude > FP._0_01)
            {
                moveDir = moveDir.Normalized;
                filter.Character->FacingDirection = moveDir;

                FP speed = filter.Character->MoveSpeed > FP._0 ? filter.Character->MoveSpeed : FP._4;
                filter.Transform->Position += moveDir * speed * frame.DeltaTime;

                filter.Character->CurrentState = ActionState.Run;
            }
            else
            {
                if (filter.Character->CurrentState == ActionState.Run)
                {
                    filter.Character->CurrentState = ActionState.Idle;
                }
            }
        }
    }
}
