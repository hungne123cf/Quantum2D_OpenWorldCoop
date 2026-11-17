using Photon.Deterministic;

namespace Quantum
{
    /// <summary>
    /// Tự động sinh ra nhân vật tương ứng cho tối đa 4 người chơi khi kết nối vào phòng online
    /// Phân bổ vị trí xuất hiện (Spawn Points) và chỉ số ban đầu
    /// Gắn sẵn Quantum View Component để Unity tự động sinh ra hình ảnh Kiếm Sĩ / Nữ Pháp Sư
    /// </summary>
    public unsafe class PlayerSpawningSystem : SystemSignalsOnly, ISignalOnPlayerAdded, ISignalOnPlayerRemoved
    {
        // GUID Asset của 2 Prefab nhân vật đã tạo trong Assets/_Project/Prefabs
        private static readonly AssetGuid SwordmasterGuid = new AssetGuid(0xcb0605d0f82e81a4, 0x4a759553b9eec16a);
        private static readonly AssetGuid MageGuid = new AssetGuid(0x4cad065e8bdb8824, 0xca7c92d3fab47a9a);

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
                Mana = (FP._100),
                MaxMana = (FP._100),
                FacingDirection = FPVector2.Right,
                ComboIndex = 0,
                IsInvulnerable = false
            };
            frame.Set(entity, playerChar);

            // GẮN VIEW COMPONENT: Báo cho Unity EntityViewUpdater tự sinh ra hình ảnh nhân vật
            AssetGuid selectedGuid = (chosenClass == CharacterClass.Swordmaster) ? SwordmasterGuid : MageGuid;
            var view = new View
            {
                Current = selectedGuid
            };
            frame.Set(entity, view);
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
