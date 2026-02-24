using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] heartIcons; // Assign 3 heart Image GameObjects here

    public void UpdateHearts(int currentLives)
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (heartIcons[i] != null)
            {
                // Show heart if index < currentLives
                // e.g. 3 lives: indices 0,1,2 all active
                // 2 lives: indices 0,1 active, 2 inactive
                heartIcons[i].SetActive(i < currentLives);
            }
        }
    }
}
