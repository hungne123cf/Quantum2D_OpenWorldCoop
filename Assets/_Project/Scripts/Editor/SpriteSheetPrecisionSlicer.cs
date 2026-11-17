using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quantum2D.EditorTools
{
    public static class SpriteSheetPrecisionSlicer
    {
        [MenuItem("Quantum2D/🎯 CAT CHUAN XAC TUNG KHUNG NHAN VAT")]
        public static void SlicePrecisely()
        {
            SliceCleanTexture("Assets/_Project/Art/Sprites/Characters/Swordmaster_Clean.png");
            SliceCleanTexture("Assets/_Project/Art/Sprites/Characters/Mage_Clean.png");
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẮT CHUẨN XÁC 100% TỪNG KHUNG HÌNH NHÂN VẬT!");
        }

        private static void SliceCleanTexture(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;

            int textureWidth = 608;
            int textureHeight = 683;
            int rows = 6;
            int cols = 8;

            float cellW = (float)textureWidth / cols;   // 76.0 px
            float cellH = (float)textureHeight / rows;  // 113.8 px

            // Hàng từ trên xuống: IDLE, RUN, ATTACK, HURT, DASH, DIE
            // Nhưng toạ độ Unity tính Y=0 từ dưới lên
            string[] rowNames = new string[] { "Die", "Dash", "Hurt", "Attack", "Run", "Idle" };

            List<SpriteMetaData> metas = new List<SpriteMetaData>();

            for (int r = 0; r < rows; r++)
            {
                string stateName = rowNames[r];
                float y = r * cellH;

                for (int c = 0; c < cols; c++)
                {
                    float x = c * cellW;

                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"{stateName}_{c}",
                        rect = new Rect(x, y, cellW, cellH),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.15f) // Gốc chân nhân vật
                    };
                    metas.Add(meta);
                }
            }

            importer.spritesheet = metas.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}
