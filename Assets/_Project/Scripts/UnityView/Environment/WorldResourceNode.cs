using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Quantum2D.UnityView.Environment
{
    public enum WorldResourceType
    {
        TreeWood = 0,
        IronMine = 1,
        ManaCrystal = 2,
        HerbalHerb = 3
    }

    /// <summary>
    /// Hệ thống Node tài nguyên thế giới mở (Cây, mỏ đá, quặng ma thuật)
    /// Hỗ trợ cả 4 người chơi cùng đi farm, chia sẻ hoặc tranh nhặt tài nguyên
    /// </summary>
    public class WorldResourceNode : MonoBehaviour
    {
        [Header("Resource Config")]
        [SerializeField] private WorldResourceType resourceType;
        [SerializeField] private int maxHealth = 3;
        [SerializeField] private GameObject dropItemPrefab;
        [SerializeField] private int dropCount = 2;
        [SerializeField] private float respawnTime = 30f;

        [Header("Visual Effects")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite damagedSprite;
        [SerializeField] private GameObject harvestHitVfx;

        private int _currentHealth;
        private bool _isHarvested = false;

        private void Awake()
        {
            _currentHealth = maxHealth;
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void TakeHarvestHit(int damage, Vector3 attackerPos)
        {
            if (_isHarvested) return;

            _currentHealth -= damage;
            StartCoroutine(ShakeEffect());

            if (harvestHitVfx != null)
            {
                GameObject vfx = Instantiate(harvestHitVfx, transform.position, Quaternion.identity);
                Destroy(vfx, 0.5f);
            }

            if (_currentHealth <= 0)
            {
                DepleteResource();
            }
            else if (damagedSprite != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = damagedSprite;
            }
        }

        private void DepleteResource()
        {
            _isHarvested = true;

            // Rơi tài nguyên cho 4 player cùng nhặt
            for (int i = 0; i < dropCount; i++)
            {
                if (dropItemPrefab != null)
                {
                    Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                    Instantiate(dropItemPrefab, transform.position + randomOffset, Quaternion.identity);
                }
            }

            if (spriteRenderer != null) spriteRenderer.enabled = false;
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            StartCoroutine(RespawnRoutine());
        }

        private IEnumerator RespawnRoutine()
        {
            yield return new WaitForSeconds(respawnTime);
            _isHarvested = false;
            _currentHealth = maxHealth;
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = normalSprite;
                spriteRenderer.enabled = true;
            }
            Collider2D col = GetComponent<Collider2D>();
            if (col != null) col.enabled = true;
        }

        private IEnumerator ShakeEffect()
        {
            Vector3 originalPos = transform.position;
            for (int i = 0; i < 4; i++)
            {
                transform.position = originalPos + (Vector3)(Random.insideUnitCircle * 0.08f);
                yield return new WaitForSeconds(0.03f);
            }
            transform.position = originalPos;
        }
    }
}
