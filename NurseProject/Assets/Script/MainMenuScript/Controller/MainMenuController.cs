using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Use this specifically for the Start Game button to ensure all prefs are reset
    public void StartNewGame(string sceneName)
    {
        // Reset the stars
        ProgressManager.ResetProgress();
        
        // Optional: If you strictly want to delete ALL PlayerPrefs (like any other saved stats)
        // PlayerPrefs.DeleteAll(); 
        
        SceneManager.LoadScene(sceneName);
    }

    // Load scene by name (recommended for clarity)
    public void LoadScene(string sceneName)
    {
        // Automatically reset progress if loading the first stage of the game
        if (ProgressManager.SceneSequence != null && ProgressManager.SceneSequence.Length > 0)
        {
            if (sceneName == ProgressManager.SceneSequence[0])
            {
                ProgressManager.ResetProgress();
            }
        }

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
