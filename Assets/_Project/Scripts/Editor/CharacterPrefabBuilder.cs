using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Quantum;
using Quantum2D.UnityView;

namespace Quantum2D.EditorTools
{
    public static class CharacterPrefabBuilder
    {
        [MenuItem("Quantum2D/⚡ 2. DONG BO PREFAB VA ANIMATION NHAN VAT")]
        public static void BuildAll()
        {
            string prefabDir = "Assets/_Project/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabDir))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            }

            BuildSwordmasterPrefab(prefabDir);

            // Copy sang Resources để Runtime spawner load ngay lập tức
            string resDir = "Assets/QuantumUser/Resources";
            if (!AssetDatabase.IsValidFolder(resDir))
            {
                AssetDatabase.CreateFolder("Assets/QuantumUser", "Resources");
            }
            AssetDatabase.CopyAsset($"{prefabDir}/Swordmaster_PlayerView.prefab", $"{resDir}/Swordmaster_PlayerView.prefab");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ TẠO VÀ GẮN ĐẦY ĐỦ ANIMATION CHO PREFAB KIẾM SĨ (SWORDMASTER)!");
        }

        private static void BuildSwordmasterPrefab(string outputDir)
        {
            string prefabPath = $"{outputDir}/Swordmaster_PlayerView.prefab";

            GameObject go = new GameObject("Swordmaster_PlayerView");

            var entityView = go.AddComponent<QuantumCharacterEntityView>();
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10;

            // Load sub-sprites từ các dải ảnh đã cắt
            List<Sprite> idleSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_idle.png");
            List<Sprite> runSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_Run.png");
            List<Sprite> attackSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_attack.png");
            List<Sprite> dashSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_dash.png");
            List<Sprite> hurtSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_hurt.png");
            List<Sprite> dieSprites = LoadSpritesFromAsset("Assets/_Project/Art/Sprites/Characters/Swordmaster_die.png");

            if (idleSprites.Count > 0)
            {
                sr.sprite = idleSprites[0];
            }

            var animController = go.AddComponent<CharacterAnimationController2D>();
            animController.SetAnimationClips(idleSprites, runSprites, attackSprites, hurtSprites, dashSprites, dieSprites);

            var vfxSpawner = go.AddComponent<CharacterVFXSpawner2D>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
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
            // Sắp xếp theo tên frame (ví dụ Swordmaster_idle_0, _1, _2...)
            result.Sort((a, b) => string.Compare(a.name, b.name, System.StringComparison.Ordinal));
            return result;
        }
    }
}
