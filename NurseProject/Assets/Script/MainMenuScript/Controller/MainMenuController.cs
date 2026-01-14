using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Load scene by name (recommended for clarity)
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Optional: load scene by build index
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    // Optional: quit application
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // visible only in Editor
    }
}
