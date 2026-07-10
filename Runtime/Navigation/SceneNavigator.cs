using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace GameKit.Navigation
{
    public static class SceneNavigator
    {
        private static readonly Stack<string> history = new Stack<string>();

        public static void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return;

            string currentScene = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(currentScene) && currentScene != sceneName)
            {
                history.Push(currentScene);
            }

            SceneManager.LoadScene(sceneName);
        }

        public static void ReplaceScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName)) return;
            SceneManager.LoadScene(sceneName);
        }

        public static void Back(string fallbackScene)
        {
            string currentScene = SceneManager.GetActiveScene().name;

            while (history.Count > 0)
            {
                string previousScene = history.Pop();
                if (!string.IsNullOrWhiteSpace(previousScene) && previousScene != currentScene)
                {
                    SceneManager.LoadScene(previousScene);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(fallbackScene) && fallbackScene != currentScene)
            {
                SceneManager.LoadScene(fallbackScene);
            }
        }

        public static void ClearHistory()
        {
            history.Clear();
        }
    }
}
