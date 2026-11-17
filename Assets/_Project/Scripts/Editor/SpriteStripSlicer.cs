using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Quantum2D.EditorTools
{
    public static class SpriteStripSlicer
    {
        [MenuItem("Quantum2D/⚡ 1. CAT TAT CA DAI FRAME THANH TUNG CON")]
        public static void SliceAllStrips()
        {
            var stripConfigs = new Dictionary<string, (int width, int height, int cols)>
            {
                { "Swordmaster_idle", (703, 121, 8) },
                { "Swordmaster_Run", (722, 110, 8) },
                { "Swordmaster_attack", (705, 100, 8) },
                { "Swordmaster_dash", (714, 88, 8) },
                { "Swordmaster_hurt", (408, 100, 4) },
                { "Swordmaster_die", (699, 112, 8) }
            };

            foreach (var kvp in stripConfigs)
            {
                string assetPath = $"Assets/_Project/Art/Sprites/Characters/{kvp.Key}.png";
                if (!File.Exists(assetPath)) continue;

                TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (importer == null) continue;

                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;

                int width = kvp.Value.width;
                int height = kvp.Value.height;
                int cols = kvp.Value.cols;
                float cellW = (float)width / cols;
                float cellH = (float)height;

                List<SpriteMetaData> metas = new List<SpriteMetaData>();
                for (int i = 0; i < cols; i++)
                {
                    float x = i * cellW;
                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"{kvp.Key}_{i}",
                        rect = new Rect(x, 0, cellW, cellH),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.15f)
                    };
                    metas.Add(meta);
                }

                importer.spritesheet = metas.ToArray();
                EditorUtility.SetDirty(importer);
                importer.SaveAndReimport();
            }

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẮT THÀNH CÔNG TỪNG FRAME CHO TẤT CẢ DẢI ẢNH!");
        }
    }
}
