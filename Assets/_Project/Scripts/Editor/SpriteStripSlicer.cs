using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Quantum2D.EditorTools
{
    public static class SpriteStripSlicer
    {
        [MenuItem("Quantum2D/⚡ CAT TAT CA DAI FRAME THANH TUNG CON")]
        public static void SliceAllStrips()
        {
            string folder = "Assets/_Project/Art/Sprites/Characters";
            string[] files = Directory.GetFiles(folder, "*.png");

            foreach (var file in files)
            {
                string assetPath = file.Replace("\\", "/");
                string fileName = Path.GetFileNameWithoutExtension(assetPath).ToLower();

                // Xác định số frame dựa theo tên động tác
                int colCount = 8;
                if (fileName.Contains("hurt")) colCount = 4; // Dải hurt có 4 frame

                SliceHorizontalStrip(assetPath, colCount);
            }

            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ CẮT THÀNH CÔNG TỪNG FRAME CHO TẤT CẢ DẢI ẢNH!");
        }

        private static void SliceHorizontalStrip(string assetPath, int colCount)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;

            // Đọc kích thước ảnh thật
            Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
            int width = tex != null ? tex.width : 700;
            int height = tex != null ? tex.height : 100;

            float cellW = (float)width / colCount;
            float cellH = (float)height;

            string baseName = Path.GetFileNameWithoutExtension(assetPath);
            List<SpriteMetaData> metas = new List<SpriteMetaData>();

            for (int i = 0; i < colCount; i++)
            {
                float x = i * cellW;
                SpriteMetaData meta = new SpriteMetaData
                {
                    name = $"{baseName}_{i}",
                    rect = new Rect(x, 0, cellW, cellH),
                    alignment = (int)SpriteAlignment.Custom,
                    pivot = new Vector2(0.5f, 0.15f) // Gốc chân nhân vật
                };
                metas.Add(meta);
            }

            importer.spritesheet = metas.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
        }
    }
}
