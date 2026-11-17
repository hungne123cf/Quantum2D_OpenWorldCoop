using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Quantum2D.EditorTools
{
    [InitializeOnLoad]
    public static class SpriteSheetAutoSlicerTool
    {
        static SpriteSheetAutoSlicerTool()
        {
            EditorApplication.delayCall += CheckAndSliceOnce;
        }

        private static void CheckAndSliceOnce()
        {
            if (!SessionState.GetBool("Quantum2D_Sliced_Done", false))
            {
                SessionState.SetBool("Quantum2D_Sliced_Done", true);
                SliceAll();
            }
        }

        [MenuItem("Quantum2D/Slice Both SpriteSheets Now")]
        public static void SliceAll()
        {
            SliceSheet("Assets/_Project/Art/Sprites/Characters/Swordmaster_SpriteSheet.png");
            SliceSheet("Assets/_Project/Art/Sprites/Characters/Mage_SpriteSheet.jpg");
            AssetDatabase.Refresh();
        }

        private static void SliceSheet(string assetPath)
        {
            TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.alphaIsTransparency = true;

            int textureHeight = 683;
            int charAreaWidth = 780;
            int rows = 6;
            int cols = 8;

            float cellWidth = (float)charAreaWidth / cols;
            float cellHeight = (float)textureHeight / rows;

            string[] rowNames = new string[] { "Die", "Dash", "Hurt", "Attack", "Run", "Idle" };

            List<SpriteMetaData> metas = new List<SpriteMetaData>();

            for (int r = 0; r < rows; r++)
            {
                string stateName = rowNames[r];
                float y = r * cellHeight;

                for (int c = 0; c < cols; c++)
                {
                    float x = c * cellWidth;

                    SpriteMetaData meta = new SpriteMetaData
                    {
                        name = $"{stateName}_{c}",
                        rect = new Rect(x, y, cellWidth, cellHeight),
                        alignment = (int)SpriteAlignment.Custom,
                        pivot = new Vector2(0.5f, 0.15f)
                    };
                    metas.Add(meta);
                }
            }

            metas.Add(new SpriteMetaData { name = "VFX_Attack_Panel", rect = new Rect(790, 360, 220, 230), alignment = 0, pivot = new Vector2(0.5f, 0.5f) });
            metas.Add(new SpriteMetaData { name = "VFX_Dash_Panel", rect = new Rect(790, 180, 220, 170), alignment = 0, pivot = new Vector2(0.5f, 0.5f) });
            metas.Add(new SpriteMetaData { name = "VFX_Hit_Panel", rect = new Rect(790, 10, 220, 160), alignment = 0, pivot = new Vector2(0.5f, 0.5f) });

            importer.spritesheet = metas.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();

            Debug.Log($"<color=green>[Quantum2D]</color> Đã tự động cắt thành công {metas.Count} sprites cho: {assetPath}");
        }
    }
}
