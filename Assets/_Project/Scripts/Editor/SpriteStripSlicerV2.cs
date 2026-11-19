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

                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                int width = tex != null ? tex.width : 1024;
                int height = tex != null ? tex.height : 240;
                int cols = 6;
                float cellW = (float)width / cols;
                float cellH = (float)height;

                List<SpriteMetaData> metas = new List<SpriteMetaData>();
                for (int i = 0; i < cols; i++)
                {
                    float x = i * cellW;
                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"{baseName}_{i}",
                        rect = new Rect(x, 0, cellW, cellH),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.08f)
                    };
                    metas.Add(meta);
                }

                importer.spritesheet = metas.ToArray();
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            // Xử lý riêng biệt file Attack 16 Frames theo Grid chuẩn 128x256 cố định
            Slice16FramesAttack();

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẮT THÀNH CÔNG TẤT CẢ SPRITE V2 (BAO GỒM 16 FRAMES ATTACK CỐ ĐỊNH)!");
        }

        private static void Slice16FramesAttack()
        {
            string assetPath = "Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_attack_16frames.png";
            if (!File.Exists(assetPath)) return;

            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;

            int totalW = 1024;
            int totalH = 512;
            int cols = 8;
            int rows = 2;

            float cellW = (float)totalW / cols; // 128 px
            float cellH = (float)totalH / rows; // 256 px

            List<SpriteMetaData> metas = new List<SpriteMetaData>();

            // Hàng 1 (Top: Y từ 256 đến 512 trong toạ độ Unity tính từ đáy)
            // Hàng 2 (Bottom: Y từ 0 đến 256)
            int index = 0;
            for (int r = 1; r >= 0; r--) // Quét từ hàng trên xuống hàng dưới
            {
                float y = r * cellH;
                for (int c = 0; c < cols; c++)
                {
                    float x = c * cellW;
                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"Swordmaster_v2_attack_{index:D2}",
                        rect = new Rect(x, y, cellW, cellH),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.15f) // Trọng tâm bàn chân cố định
                    };
                    metas.Add(meta);
                    index++;
                }
            }

            importer.spritesheet = metas.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
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
            List<Sprite> walkSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_walk.png");
            
            // Tải 16 frames đòn chém
            List<Sprite> attack16Sprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_v2_attack_16frames.png");

            if (idleSprites.Count > 0)
            {
                sr.sprite = idleSprites[0];
            }

            var animController = go.AddComponent<CharacterAnimationController2D>();
            // Gán 16 frames attack với tốc độ 18 fps cực kỳ mượt mà
            animController.SetAnimationClips(idleSprites, runSprites, attack16Sprites, walkSprites, runSprites, idleSprites);

            var vfxSpawner = go.AddComponent<CharacterVFXSpawner2D>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);

            string resDir = "Assets/QuantumUser/Resources";
            AssetDatabase.CopyAsset(prefabPath, $"{resDir}/Swordmaster_PlayerView.prefab");

            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẬP NHẬT PREFAB VỚI BỘ ATTACK 16 FRAMES SIÊU ĐẲNG CẤP!");
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
