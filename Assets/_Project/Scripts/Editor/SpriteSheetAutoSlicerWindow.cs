using System.IO;
using UnityEditor;
using UnityEngine;

namespace Quantum2D.EditorTools
{
    /// <summary>
    /// Unity Editor Window hỗ trợ cấu hình và slicing Sprite Sheets
    /// Tự động thiết lập TextureType = Multiple, FilterMode = Point/Bilinear, Sprite Editor Pivot
    /// Cho 2 nhân vật: Kiếm Sĩ (Swordmaster) và Nữ Pháp Sư (Mage)
    /// </summary>
    public class SpriteSheetAutoSlicerWindow : EditorWindow
    {
        private Texture2D targetTexture;
        private int rows = 6;
        private int columns = 8;
        private Vector2 pivot = new Vector2(0.5f, 0.1f); // Bottom-Center chuẩn cho Top-Down 2D

        [MenuItem("Quantum2D / Sprite Sheet Slicer Helper")]
        public static void ShowWindow()
        {
            GetWindow<SpriteSheetAutoSlicerWindow>("Sprite Sheet Slicer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Cấu hình Sprite Sheet Nhân Vật 2D", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            targetTexture = (Texture2D)EditorGUILayout.ObjectField("Texture Sprite Sheet", targetTexture, typeof(Texture2D), false);
            rows = EditorGUILayout.IntField("Số hàng (Rows)", rows);
            columns = EditorGUILayout.IntField("Số cột nhân vật (Cols)", columns);
            pivot = EditorGUILayout.Vector2Field("Pivot (Gốc toạ độ chân)", pivot);

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox(
                "Sprite Sheet hiện tại của bạn gồm 6 hàng chính:\n" +
                "1. IDLE (Đứng chuẩn bị)\n" +
                "2. RUN (Chạy 8 hướng)\n" +
                "3. ATTACK (Combo chém/bắn phép + VFX)\n" +
                "4. HURT (Giật lùi khi trúng đòn)\n" +
                "5. DASH (Lướt né chiêu)\n" +
                "6. DIE (Ngã gục / Chờ hồi sinh co-op)\n" +
                "Kèm theo panel bên phải là các hiệu ứng Attack, Dash, Hit VFX và Palette màu.",
                MessageType.Info);

            EditorGUILayout.Space(10);

            if (GUILayout.Button("1. Chuẩn Hoá Texture Settings (Sprite Multiple)", GUILayout.Height(35)))
            {
                ApplySpriteTextureSettings();
            }

            if (GUILayout.Button("2. Mở Sprite Editor để tinh chỉnh lát cắt", GUILayout.Height(35)))
            {
                if (targetTexture != null)
                {
                    Selection.activeObject = targetTexture;
                    EditorApplication.ExecuteMenuItem("Window/2D/Sprite Editor");
                }
                else
                {
                    EditorUtility.DisplayDialog("Lỗi", "Vui lòng kéo Sprite Sheet vào trước!", "OK");
                }
            }
        }

        private void ApplySpriteTextureSettings()
        {
            if (targetTexture == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn Texture!", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(targetTexture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.filterMode = FilterMode.Bilinear;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true;

                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                EditorUtility.DisplayDialog("Thành công", $"Đã chuẩn hoá Texture {targetTexture.name} sang Sprite Multiple chất lượng cao!", "OK");
            }
        }
    }
}
