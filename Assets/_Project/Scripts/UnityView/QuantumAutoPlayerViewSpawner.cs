using System.Collections.Generic;
using Quantum;
using UnityEngine;

namespace Quantum2D.UnityView
{
    public class QuantumAutoPlayerViewSpawner : MonoBehaviour
    {
        [Header("Character Prefabs")]
        [SerializeField] private GameObject swordmasterPrefab;
        [SerializeField] private GameObject magePrefab;

        private readonly Dictionary<EntityRef, GameObject> _spawnedViews = new Dictionary<EntityRef, GameObject>();

        private void Awake()
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
            // Kiểm tra Runner của Quantum (Offline runner hoặc Online runner)
            var runner = QuantumRunner.Default;
            if (runner == null || runner.Game == null) return;

            var frame = runner.Game.Frames.Predicted;
            if (frame == null) return;

            // Lọc toàn bộ người chơi trong Frame
            var players = frame.Filter<Transform2D, PlayerCharacter>();
            var activeEntities = new HashSet<EntityRef>();

            while (players.Next(out var entity, out var transform2d, out var playerChar))
            {
                activeEntities.Add(entity);

                if (!_spawnedViews.TryGetValue(entity, out var viewGo) || viewGo == null)
                {
                    GameObject prefabToSpawn = (playerChar.ClassType == CharacterClass.Swordmaster) ? swordmasterPrefab : magePrefab;
                    
                    if (prefabToSpawn != null)
                    {
                        Vector3 worldPos = new Vector3((float)transform2d.Position.X, (float)transform2d.Position.Y, 0f);
                        viewGo = Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
                        viewGo.name = $"Player_{playerChar.Player}_{playerChar.ClassType}";
                        viewGo.transform.localScale = Vector3.one * 1.5f; // Tăng nhẹ kích thước dễ nhìn

                        var entityView = viewGo.GetComponent<QuantumCharacterEntityView>();
                        if (entityView != null)
                        {
                            entityView.EntityRef = entity;
                        }

                        _spawnedViews[entity] = viewGo;
                        Debug.Log($"<color=green>[Quantum2D]</color> ĐÃ SINH NHÂN VẬT THÀNH CÔNG: {viewGo.name} tại toạ độ {worldPos}");
                    }
                    else
                    {
                        Debug.LogWarning("[Quantum2D] Không tìm thấy Prefab nhân vật để sinh!");
                    }
                }
                else
                {
                    // Cập nhật vị trí mượt mà
                    Vector3 targetPos = new Vector3((float)transform2d.Position.X, (float)transform2d.Position.Y, 0f);
                    viewGo.transform.position = targetPos;
                }
            }

            // Dọn dẹp
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
