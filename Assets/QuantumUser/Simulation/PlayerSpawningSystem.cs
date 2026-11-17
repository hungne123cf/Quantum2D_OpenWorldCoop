using Photon.Deterministic;

namespace Quantum
{
    /// <summary>
    /// Tự động sinh ra nhân vật tương ứng cho tối đa 4 người chơi khi kết nối vào phòng online
    /// Phân bổ vị trí xuất hiện (Spawn Points) và chỉ số ban đầu
    /// </summary>
    public unsafe class PlayerSpawningSystem : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime)
        {
            // Kiểm tra xem player này đã được spawn entity chưa
            var players = frame.Filter<PlayerCharacter>();
            while (players.Next(out var existingEntity, out var existingChar))
            {
                if (existingChar.Player == player) return;
            }

            // Tạo mới một Entity đại diện cho người chơi
            var entity = frame.Create();

            // Gán Transform2D (Vị trí xuất hiện phân bổ cho 4 player)
            int playerSlot = (int)player;
            FP spawnX = (playerSlot % 2 == 0 ? FP._2 : -FP._2);
            FP spawnY = (playerSlot > 1 ? -FP._2 : FP._2);

            var transform = new Transform2D
            {
                Position = new FPVector2(spawnX, spawnY),
                Rotation = FP._0
            };
            frame.Set(entity, transform);

            // Gán Component PlayerCharacter
            CharacterClass chosenClass = (playerSlot % 2 == 0) ? CharacterClass.Mage : CharacterClass.Swordmaster;

            var playerChar = new PlayerCharacter
            {
                Player = player,
                ClassType = chosenClass,
                CurrentState = ActionState.Idle,
                MoveSpeed = FP._4,
                Health = FP._100,
                MaxHealth = FP._100,
                Mana = FP._100,
                MaxMana = FP._100,
                FacingDirection = FPVector2.Right,
                ComboIndex = 0,
                IsInvulnerable = false
            };
            frame.Set(entity, playerChar);

            // Gán View Component thông qua AssetGuid.TryParse chuẩn của Quantum 3
            string guidString = (chosenClass == CharacterClass.Swordmaster) ? "cb0605d0f82e81a44a759553b9eec16a" : "4cad065e8bdb8824ca7c92d3fab47a9a";
            if (AssetGuid.TryParse(guidString, out var assetGuid, false))
            {
                var view = new View
                {
                    Current = assetGuid
                };
                frame.Set(entity, view);
            }
        }

        public void OnPlayerRemoved(Frame frame, PlayerRef player)
        {
            var players = frame.Filter<PlayerCharacter>();
            while (players.Next(out var entity, out var playerChar))
            {
                if (playerChar.Player == player)
                {
                    frame.Destroy(entity);
                    break;
                }
            }
        }
    }
}
