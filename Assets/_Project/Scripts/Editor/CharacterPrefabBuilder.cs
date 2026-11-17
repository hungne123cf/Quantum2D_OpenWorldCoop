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

            // Copy sang Resources để Spawner luôn load được khi chạy runtime
            string resDir = "Assets/QuantumUser/Resources";
            if (!AssetDatabase.IsValidFolder(resDir))
            {
                AssetDatabase.CreateFolder("Assets/QuantumUser", "Resources");
            }
            AssetDatabase.CopyAsset($"{prefabDir}/Swordmaster_PlayerView.prefab", $"{resDir}/Swordmaster_PlayerView.prefab");
            AssetDatabase.CopyAsset($"{prefabDir}/Mage_PlayerView.prefab", $"{resDir}/Mage_PlayerView.prefab");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("<color=green>[Quantum2D]</color> ĐÃ TẠO VÀ GẮN ĐẦY ĐỦ ANIMATION CHO PREFAB KIẾM SĨ & PHÁP SƯ!");
        }

        private static void BuildCharacterPrefab(string prefabName, string spriteSheetPath, string outputDir)
        {
            string prefabPath = $"{outputDir}/{prefabName}.prefab";

            GameObject go = new GameObject(prefabName);

            var entityView = go.AddComponent<QuantumCharacterEntityView>();

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 10; // Đặt sorting order cao để luôn hiển thị trên mặt đất

            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(spriteSheetPath);
            Sprite defaultSprite = null;
            List<Sprite> idleSprites = new List<Sprite>();
            List<Sprite> runSprites = new List<Sprite>();
            List<Sprite> attackSprites = new List<Sprite>();
            List<Sprite> hurtSprites = new List<Sprite>();
            List<Sprite> dashSprites = new List<Sprite>();
            List<Sprite> dieSprites = new List<Sprite>();

            foreach (var asset in subAssets)
            {
                if (asset is Sprite sp)
                {
                    if (defaultSprite == null && sp.name.StartsWith("Idle")) defaultSprite = sp;

                    if (sp.name.StartsWith("Idle")) idleSprites.Add(sp);
                    else if (sp.name.StartsWith("Run")) runSprites.Add(sp);
                    else if (sp.name.StartsWith("Attack")) attackSprites.Add(sp);
                    else if (sp.name.StartsWith("Hurt")) hurtSprites.Add(sp);
                    else if (sp.name.StartsWith("Dash")) dashSprites.Add(sp);
                    else if (sp.name.StartsWith("Die")) dieSprites.Add(sp);
                }
            }

            if (defaultSprite == null && subAssets.Length > 0 && subAssets[0] is Sprite firstSp)
            {
                defaultSprite = firstSp;
            }

            sr.sprite = defaultSprite;

            // Gắn và điền danh sách AnimationClips vào CharacterAnimationController2D
            var anim = go.AddComponent<CharacterAnimationController2D>();
            
            // Dùng SerializedObject để gán dữ liệu vào private fields của Animation Controller
            var serializedAnim = new SerializedObject(anim);
            var clipsProp = serializedAnim.FindProperty("animationClips");
            var srProp = serializedAnim.FindProperty("spriteRenderer");
            srProp.objectReferenceValue = sr;

            clipsProp.ClearArray();

            AddClip(clipsProp, CharacterAnimState.Idle, idleSprites, 8f, true);
            AddClip(clipsProp, CharacterAnimState.Run, runSprites, 10f, true);
            AddClip(clipsProp, CharacterAnimState.Attack, attackSprites, 12f, false);
            AddClip(clipsProp, CharacterAnimState.Hurt, hurtSprites, 8f, false);
            AddClip(clipsProp, CharacterAnimState.Dash, dashSprites, 14f, false);
            AddClip(clipsProp, CharacterAnimState.Die, dieSprites, 6f, false);

            serializedAnim.ApplyModifiedPropertiesWithoutUndo();

            go.AddComponent<CharacterVFXSpawner2D>();

            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            GameObject.DestroyImmediate(go);
        }

        private static void AddClip(SerializedProperty clipsProp, CharacterAnimState state, List<Sprite> sprites, float fps, bool loop)
        {
            if (sprites.Count == 0) return;

            int index = clipsProp.arraySize;
            clipsProp.InsertArrayElementAtIndex(index);
            var element = clipsProp.GetArrayElementAtIndex(index);

            element.FindPropertyRelative("state").enumValueIndex = (int)state;
            element.FindPropertyRelative("frameRate").floatValue = fps;
            element.FindPropertyRelative("loop").boolValue = loop;

            var framesProp = element.FindPropertyRelative("frames");
            framesProp.ClearArray();
            for (int i = 0; i < sprites.Count; i++)
            {
                framesProp.InsertArrayElementAtIndex(i);
                framesProp.GetArrayElementAtIndex(i).objectReferenceValue = sprites[i];
            }
        }
    }
}
