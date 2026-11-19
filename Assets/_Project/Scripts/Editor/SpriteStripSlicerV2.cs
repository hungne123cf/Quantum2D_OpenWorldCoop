using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Quantum;
using Quantum2D.UnityView;

namespace Quantum2D.EditorTools
{
    public static class SpriteStripSlicerV2
    {
        [MenuItem("Quantum2D/🌟 1. CAT DAI ANH V2 CHUAN (6 FRAMES)")]
        public static void SliceV2Strips()
        {
            string[] v2Files = new string[]
            {
                "Swordmaster_v2_idle",
                "Swordmaster_v2_run",
                "Swordmaster_v2_attack",
                "Swordmaster_v2_walk"
            };

            foreach (var baseName in v2Files)
            {
                string assetPath = $"Assets/_Project/Art/Sprites/Characters/{baseName}.png";
                if (!File.Exists(assetPath)) continue;

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;

                // Load Texture để lấy kích thước thật (1024 x 240)
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                int width = tex != null ? tex.width : 1024;
                int height = tex != null ? tex.height : 240;
                int cols = 6;
                float cellW = (float)width / cols; // ~170.66 px
                float cellH = (float)height;       // 240 px

                List<SpriteMetaData> metas = new List<SpriteMetaData>();
                for (int i = 0; i < cols; i++)
                {
                    float x = i * cellW;
                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"{baseName}_{i}",
                        rect = new Rect(x, 0, cellW, cellH),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.08f) // Đặt gốc chân nhân vật sát đáy
                    };
                    metas.Add(meta);
                }

                importer.spritesheet = metas.ToArray();
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẮT GỌN GÀNG VÀ CHUẨN XÁC 6 FRAMES V2 CHO KIẾM SĨ MỚI!");
        }

        [MenuItem("Quantum2D/🌟 2. RÁP PREFAB VOI ANH V2 SIEU MUOT")]
        public static void BuildPrefabV2()
        {
            string prefabDir = "Assets/_Project/Prefabs";
            string prefabPath = $"{prefabDir}/Swordmaster_PlayerView.prefab";

            GameObject go = new GameObject("Swordmaster_PlayerView");

            var entityView = go.AddComponent<QuantumCharacterEntityView>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;

            List<Sprite> idleSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_idle.png");
            List<Sprite> runSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_run.png");
            List<Sprite> attackSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_attack.png");
            List<Sprite> walkSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_walk.png");

            if (idleSprites.Count > 0)
            {
                sr.sprite = idleSprites[0];
            }

            var animController = go.AddComponent<CharacterAnimationController2D>();
            animController.SetAnimationClips(idleSprites, runSprites, attackSprites, walkSprites, runSprites, idleSprites);

            var vfxSpawner = go.AddComponent<CharacterVFXSpawner2D>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

            // Copy sang Resources
            string resDir = "Assets/QuantumUser/Resources";
            AssetDatabase.CopyAsset(prefabPath, $"{resDir}/Swordmaster_PlayerView.prefab");

            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẬP NHẬT PREFAB SWORDMASTER GỌN GÀNG, KHÔNG THỪA KHOẢNG TRỐNG!");
        }

        private static List<Sprite> LoadSpritesFromAsset(string path)
        {
            List<Sprite> result = new List<Sprite>();
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            foreach (var a in assets)
            {
                if (a is Sprite s)
                {
                    result.Add(s);
                }
            }
            result.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            return result;
        }
    }
}
