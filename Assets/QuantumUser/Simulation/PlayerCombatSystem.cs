using Photon.Deterministic;

namespace Quantum
{
    /// <summary>
    /// Quantum 3 Deterministic Combat & Co-op Revive System
    /// Xử lý chuỗi Combo 3-Hit chém/phép và cơ chế hồi sinh đồng đội khi gục ngã
    /// </summary>
    public unsafe class PlayerCombatSystem : SystemMainThreadFilter<PlayerCombatSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform2D* Transform;
            public PlayerCharacter* Character;
        }

        public override void Update(Frame frame, ref Filter filter)
        {
            if (filter.Character->CurrentState == ActionState.Downed)
            {
                return;
            }

            var input = frame.GetPlayerInput(filter.Character->Player);
            if (input == null) return;

            // Xử lý Attack Combo
            if (input->Attack.WasPressed && filter.Character->AttackCooldown <= FP._0 && filter.Character->CurrentState != ActionState.Dash)
            {
                filter.Character->ComboIndex = (filter.Character->ComboIndex % 3) + 1;
                filter.Character->AttackCooldown = FP._0_33; // 0.33s giữa các hit
                filter.Character->StateTimer = FP._0_25 + FP._0_05; // 0.30s

                switch (filter.Character->ComboIndex)
                {
                    case 1: filter.Character->CurrentState = ActionState.AttackCombo1; break;
                    case 2: filter.Character->CurrentState = ActionState.AttackCombo2; break;
                    case 3: filter.Character->CurrentState = ActionState.AttackCombo3; break;
                }

                // Bắn Event để View sinh ra đòn chém Cyan (Kiếm Sĩ) hoặc Cầu phép Magenta (Pháp Sư)
                frame.Events.EventPlayerAttack(filter.Entity, filter.Character->ClassType, filter.Character->ComboIndex, filter.Transform->Position, filter.Character->FacingDirection);
                return;
            }

            // Đang trong lúc vung kiếm/bắn phép
            if (filter.Character->CurrentState == ActionState.AttackCombo1 ||
                filter.Character->CurrentState == ActionState.AttackCombo2 ||
                filter.Character->CurrentState == ActionState.AttackCombo3)
            {
                filter.Character->StateTimer -= frame.DeltaTime;
                if (filter.Character->StateTimer <= FP._0)
                {
                    filter.Character->CurrentState = ActionState.Idle;
                }
            }

            // Xử lý nút Tương tác (Interact) để hồi sinh đồng đội ở gần
            if (input->Interact.IsDown)
            {
                CheckAndReviveTeammate(frame, ref filter);
            }
            else
            {
                filter.Character->ReviveProgress = FP._0;
            }
        }

        private void CheckAndReviveTeammate(Frame frame, ref Filter filter)
        {
            var players = frame.Filter<Transform2D, PlayerCharacter>();
            while (players.Next(out var targetEntity, out var targetTrans, out var targetChar))
            {
                if (targetEntity == filter.Entity) continue;

                // Nếu đồng đội đang gục ngã (Downed) và nằm trong bán kính 2 mét
                if (targetChar.CurrentState == ActionState.Downed)
                {
                    FP distSq = FPVector2.DistanceSquared(filter.Transform->Position, targetTrans.Position);
                    if (distSq <= FP._4) // Bán kính 2 mét (2^2 = 4)
                    {
                        filter.Character->ReviveProgress += frame.DeltaTime;

                        // Giữ đủ 2.5 giây thì đồng đội sống lại với 50% HP
                        if (filter.Character->ReviveProgress >= FP._2 + FP._0_50)
                        {
                            targetChar.CurrentState = ActionState.Idle;
                            targetChar.Health = targetChar.MaxHealth / FP._2;
                            filter.Character->ReviveProgress = FP._0;

                            frame.Events.EventPlayerRevived(targetEntity, filter.Entity);
                        }
                        return;
                    }
                }
            }
        }
    }
}
