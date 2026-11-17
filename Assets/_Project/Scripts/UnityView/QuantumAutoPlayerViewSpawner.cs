using System.Collections.Generic;
using Quantum;
using UnityEngine;

namespace Quantum2D.UnityView
{
    /// <summary>
    /// Đồng bộ tự động sinh ra nhân vật hiển thị trực tiếp cho Quantum 3
    /// Không phụ thuộc vào AssetDB GUID, đảm bảo thấy ngay nhân vật 100% khi bấm Play
    /// </summary>
    public class QuantumAutoPlayerViewSpawner : MonoBehaviour
    {
        [Header("Character Prefabs")]
        [SerializeField] private GameObject swordmasterPrefab;
        [SerializeField] private GameObject magePrefab;

        private readonly Dictionary<EntityRef, GameObject> _spawnedViews = new Dictionary<EntityRef, GameObject>();

        private void OnEnable()
        {
            if (swordmasterPrefab == null)
            {
                swordmasterPrefab = Resources.Load<GameObject>("Swordmaster_PlayerView");
            }
            if (magePrefab == null)
            {
                magePrefab = Resources.Load<GameObject>("Mage_PlayerView");
            }
        }

        private void Update()
        {
            var game = QuantumRunner.Default != null ? QuantumRunner.Default.Game : null;
            if (game == null || game.Frames.Predicted == null) return;

            var frame = game.Frames.Predicted;

            // Duyệt toàn bộ người chơi trong Frame mô phỏng của Quantum
            var players = frame.Filter<Transform2D, PlayerCharacter>();
            var activeEntities = new HashSet<EntityRef>();

            while (players.Next(out var entity, out var transform2d, out var playerChar))
            {
                activeEntities.Add(entity);

                if (!_spawnedViews.TryGetValue(entity, out var viewGo))
                {
                    // Chọn đúng Prefab Kiếm Sĩ hoặc Pháp Sư
                    GameObject prefabToSpawn = (playerChar.ClassType == CharacterClass.Swordmaster) ? swordmasterPrefab : magePrefab;
                    
                    if (prefabToSpawn != null)
                    {
                        viewGo = Instantiate(prefabToSpawn, transform2d.Position.ToUnityVector2(), Quaternion.identity);
                        viewGo.name = $"Player_{playerChar.Player}_{playerChar.ClassType}";
                        
                        var entityView = viewGo.GetComponent<QuantumCharacterEntityView>();
                        if (entityView != null)
                        {
                            entityView.EntityRef = entity;
                        }

                        _spawnedViews[entity] = viewGo;
                        Debug.Log($"<color=cyan>[Quantum2D]</color> Đã sinh nhân vật hiển thị thành công: {viewGo.name}");
                    }
                }
                else
                {
                    // Cập nhật vị trí mượt mà theo toạ độ Quantum ECS
                    viewGo.transform.position = transform2d.Position.ToUnityVector2();
                }
            }

            // Dọn dẹp nhân vật khi người chơi thoát
            var toRemove = new List<EntityRef>();
            foreach (var kvp in _spawnedViews)
            {
                if (!activeEntities.Contains(kvp.Key))
                {
                    if (kvp.Value != null) Destroy(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (var key in toRemove)
            {
                _spawnedViews.Remove(key);
            }
        }
    }
}
