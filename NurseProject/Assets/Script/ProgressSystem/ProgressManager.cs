using UnityEngine;

public static class ProgressManager
{
    // The strict sequence of scenes in the game
    public static readonly string[] SceneSequence = new string[]
    {
        "Instability",
        "Immobility",
        "Intellectual Impairment (NF)",
        "Insomnia (NF)",
        "Inanition (NF)",
        "Incontinence"
    };

    public static int CurrentStars
    {
        get => PlayerPrefs.GetInt("PlayerStars", 0);
        set => PlayerPrefs.SetInt("PlayerStars", value);
    }

    public static void ResetProgress()
    {
        CurrentStars = 0;
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Called when a level is finished successfully.
    /// Increments stars if the player just completed their current stage.
    /// </summary>
    public static void CompleteCurrentStage(string currentSceneName)
    {
        int index = System.Array.IndexOf(SceneSequence, currentSceneName);
        if (index >= 0)
        {
            // Only unlock the next stage if they are at the stage they need to beat
            // (prevents adding stars if they replay old stages)
            if (index == CurrentStars)
            {
                CurrentStars++;
                PlayerPrefs.Save();
                Debug.Log($"[ProgressManager] Star unlocked! Total Stars: {CurrentStars}");
            }
            else
            {
                Debug.Log($"[ProgressManager] Stage already beaten. Total Stars: {CurrentStars}");
            }
        }
        else
        {
            Debug.LogWarning($"[ProgressManager] Completed scene {currentSceneName} not in sequence array.");
        }
    }

    public static string GetNextSceneName()
    {
        // If there are still more levels
        if (CurrentStars < SceneSequence.Length)
        {
            return SceneSequence[CurrentStars];
        }
        
        // If finished all 6 levels, return MainMenu or the End scene
        return "MainMenu";
    }
}
