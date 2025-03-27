using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainMenu
{
    public class MainMenuManager : MonoBehaviour
    {
        public void QuitGame()
        {
            Application.Quit();
        }

        public void PlayGame()
        {
            SceneManager.LoadScene(3);
        }

        public void LoadMultiplayerMenu()
        {
            SceneManager.LoadScene(1);
        }
    }
}
