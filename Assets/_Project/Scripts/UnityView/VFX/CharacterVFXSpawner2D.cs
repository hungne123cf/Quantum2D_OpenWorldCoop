using System.Collections;
using UnityEngine;

namespace Quantum2D.UnityView
{
    /// <summary>
    /// VFX Controller cho đòn chém kiếm khí, ma trận phép thuật, vệt dash lướt, hiệu ứng trúng đòn
    /// Hoàn toàn tách biệt để game chạy mượt mà không làm chậm deterministic simulation
    /// </summary>
    public class CharacterVFXSpawner2D : MonoBehaviour
    {
        [Header("Slash / Magic Attack VFX Prefabs")]
        [SerializeField] private GameObject[] attackVfxPrefabs; // Kiếm khí Cyan / Ma pháp Magenta

        [Header("Dash Afterimage / Trails")]
        [SerializeField] private GameObject dashTrailVfxPrefab;

        [Header("Hit Sparks / Stars")]
        [SerializeField] private GameObject hitImpactPrefab;

        public void SpawnAttackVfx(int comboIndex, Vector3 spawnPosition, Vector2 direction, bool flipX)
        {
            if (attackVfxPrefabs == null || attackVfxPrefabs.Length == 0) return;

            int index = Mathf.Clamp(comboIndex, 0, attackVfxPrefabs.Length - 1);
            GameObject prefab = attackVfxPrefabs[index];
            if (prefab == null) return;

            GameObject vfx = Instantiate(prefab, spawnPosition, Quaternion.identity);
            
            // Xoay vệt chém theo hướng tấn công
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            vfx.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            SpriteRenderer sr = vfx.GetComponentInChildren<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipY = flipX; // Lật vệt chém nếu đang quay trái
            }

            Destroy(vfx, 0.6f);
        }

        public void SpawnDashTrail(Vector3 position, Vector2 direction)
        {
            if (dashTrailVfxPrefab == null) return;
            GameObject trail = Instantiate(dashTrailVfxPrefab, position, Quaternion.identity);
            Destroy(trail, 0.4f);
        }

        public void SpawnHitImpact(Vector3 position)
        {
            if (hitImpactPrefab == null) return;
            GameObject hit = Instantiate(hitImpactPrefab, position, Quaternion.identity);
            Destroy(hit, 0.35f);
        }
    }
}
