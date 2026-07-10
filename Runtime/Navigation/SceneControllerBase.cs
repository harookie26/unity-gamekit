using UnityEngine;
using UnityEngine.InputSystem;

namespace GameKit.Navigation
{
    public abstract class SceneControllerBase : MonoBehaviour
    {
        [Header("Scene Navigation")]
        [SerializeField] private string backScene = "MainMenu";

        protected virtual void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) Back();
        }

        public void LoadScene(string sceneName) => SceneNavigator.LoadScene(sceneName);
        public void ReplaceScene(string sceneName) => SceneNavigator.ReplaceScene(sceneName);
        public void Back() => SceneNavigator.Back(backScene);

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
