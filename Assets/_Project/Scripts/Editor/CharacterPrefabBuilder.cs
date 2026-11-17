using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Quantum;
using Quantum2D.UnityView;

namespace Quantum2D.EditorTools
{
    public static class CharacterPrefabBuilder
    {
        [MenuItem("Quantum2D/Build Character Prefabs and Scene Setup")]
        public static void BuildAll()
        {
            string prefabDir = "Assets/_Project/Prefabs";
            if (!AssetDatabase.IsValidFolder(prefabDir))
            {
                AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");
            }

            BuildCharacterPrefab("Swordmaster_PlayerView", "Assets/_Project/Art/Sprites/Characters/Swordmaster_SpriteSheet.png", prefabDir);
            BuildCharacterPrefab("Mage_PlayerView", "Assets/_Project/Art/Sprites/Characters/Mage_SpriteSheet.jpg", prefabDir);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> Đã tạo xong Prefab nhân vật Kiếm Sĩ & Pháp Sư!");
        }

        private static void BuildCharacterPrefab(string prefabName, string spriteSheetPath, string outputDir)
        {
            string prefabPath = $"{outputDir}/{prefabName}.prefab";

            // Tạo GameObject gốc
            GameObject go = new GameObject(prefabName);

            // Gắn QuantumEntityView
            var entityView = go.AddComponent<QuantumCharacterEntityView>();

            // Gắn SpriteRenderer
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;

            // Load sprite đầu tiên làm mặc định
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath);
            Sprite defaultSprite = null;
            List<Sprite> idleSprites = new List<Sprite>();
            List<Sprite> runSprites = new List<Sprite>();
            List<Sprite> attackSprites = new List<Sprite>();

            foreach (var asset in subAssets)
            {
                if (asset is Sprite sp)
                {
                    if (defaultSprite == null && sp.name.StartsWith("Idle")) defaultSprite = sp;
                    if (sp.name.StartsWith("Idle")) idleSprites.Add(sp);
                    else if (sp.name.StartsWith("Run")) runSprites.Add(sp);
                    else if (sp.name.StartsWith("Attack")) attackSprites.Add(sp);
                }
            }

            if (defaultSprite != null) sr.sprite = defaultSprite;

            // Gắn CharacterAnimationController2D
            var anim = go.AddComponent<CharacterAnimationController2D>();

            // Gắn CharacterVFXSpawner2D
            go.AddComponent<CharacterVFXSpawner2D>();

            // Lưu thành Prefab
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            GameObject.DestroyImmediate(go);
        }
    }
}
