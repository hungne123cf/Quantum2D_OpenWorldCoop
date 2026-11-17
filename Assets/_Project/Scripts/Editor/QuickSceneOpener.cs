using UnityEditor;
using UnityEditor.SceneManagement;

namespace Quantum2D.EditorTools
{
    public static class QuickSceneOpener
    {
        [MenuItem("Quantum2D/🎮 MO SCENE GAMEPLAY NGAY")]
        public static void OpenGameScene()
        {
            EditorSceneManager.OpenScene("Assets/QuantumUser/Scenes/QuantumGameScene.unity");
        }

        [MenuItem("Quantum2D/📱 MO SCENE MENU ONLINE")]
        public static void OpenMenuScene()
        {
            EditorSceneManager.OpenScene("Assets/Photon/QuantumMenu/QuantumSampleMenu.unity");
        }
    }
}
