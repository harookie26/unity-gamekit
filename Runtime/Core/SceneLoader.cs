using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameKit.Core
{
    public sealed class SceneLoader : MonoBehaviour
    {
        public void LoadSceneByName(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Scene name cannot be empty.", this);
                return;
            }

            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneByIndex(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
            {
                Debug.LogError($"Invalid scene index: {sceneIndex}.", this);
                return;
            }

            SceneManager.LoadScene(sceneIndex);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogError("Scene name cannot be empty.", this);
                return;
            }

            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
