using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ProgressSceneController : MonoBehaviour
{
    [Header("UI Elements")]
    public Image[] starImages; // Array of 6 star images
    public Sprite starEmptySprite;
    public Sprite starFullSprite;
    public Button continueButton;
    public float animationDelay = 0.5f;

    [Header("Completion Settings")]
    public Image iIconImage;
    public Sprite iIconSuccessSprite;
    [Tooltip("GameObject to show when all stars are collected (e.g. Congratulation Text)")]
    public GameObject congratulationMessageObj;
    [Tooltip("Text component inside the continue button")]
    public TMP_Text continueButtonText;
    public string endGameButtonString = "จบเกม";

    private int targetStars;

    private void Start()
    {
        if (congratulationMessageObj != null)
        {
            congratulationMessageObj.SetActive(false);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(LoadNextScene);
            continueButton.gameObject.SetActive(false); // Hide button initially
        }

        targetStars = ProgressManager.CurrentStars;

        // Ensure array is size 6
        if (starImages.Length != ProgressManager.SceneSequence.Length)
        {
            Debug.LogWarning("ProgressSceneController: starImages array length doesn't match SceneSequence length!");
        }

        // Initialize UI before animation
        for (int i = 0; i < starImages.Length; i++)
        {
            // If they just got a new star (targetStars > 0), the previous amount of stars was (targetStars - 1).
            // So any star index LESS than (targetStars - 1) should be full.
            if (targetStars > 0 && i < targetStars - 1)
            {
                // Previously unlocked stars are already full
                starImages[i].sprite = starFullSprite;
                starImages[i].color = Color.white; 
            }
            else
            {
                // New star to be unlocked, and future locked stars are empty
                starImages[i].sprite = starEmptySprite;
                starImages[i].color = new Color(1, 1, 1, 0.5f); // Example: half transparent for empty ones
            }
        }

        StartCoroutine(AnimateProgress());
    }

    private IEnumerator AnimateProgress()
    {
        // Wait a small moment before animating
        yield return new WaitForSeconds(animationDelay);

        // If targetStars > 0, we animate the last earned star
        if (targetStars > 0 && targetStars <= starImages.Length)
        {
            int justUnlockedIndex = targetStars - 1;
            
            // Simple pop animation loop (could use DOTween if you have it)
            Image newStar = starImages[justUnlockedIndex];
            newStar.sprite = starFullSprite;
            newStar.color = Color.white;
            
            // Optional: Basic scale animation using Coroutine
            float time = 0;
            float duration = 0.5f;
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = Mathf.Clamp01(time / duration);
                
                // Add a little bounce effect (overshoot) while avoiding negative values for Pow
                float remain = Mathf.Max(0f, 1f - t);
                float ease = Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(remain, 2.2f) + t;
                
                newStar.transform.localScale = Vector3.LerpUnclamped(startScale, endScale, ease);
                yield return null;
            }
            newStar.transform.localScale = endScale;
        }

        yield return new WaitForSeconds(0.5f);

        // If all stars are collected, show the success icon
        if (targetStars == starImages.Length)
        {
            if (congratulationMessageObj != null)
            {
                congratulationMessageObj.SetActive(true);
            }

            if (continueButtonText != null)
            {
                continueButtonText.text = endGameButtonString;
            }

            if (iIconImage != null && iIconSuccessSprite != null)
            {
                iIconImage.sprite = iIconSuccessSprite;

                // Optional pop animation for the I icon
                float iTime = 0;
                float iDuration = 0.5f;
                Vector3 iStartScale = Vector3.zero; // or from its current scale if you prefer it not disappearing fully, but zero makes a good pop
                Vector3 iEndScale = Vector3.one * 2f; // target 2x scale
                
                iIconImage.transform.localScale = iStartScale;
                
                while (iTime < iDuration)
                {
                    iTime += Time.deltaTime;
                    float t = Mathf.Clamp01(iTime / iDuration);
                    float remain = Mathf.Max(0f, 1f - t);
                    float ease = Mathf.Sin(t * Mathf.PI * (0.2f + 2.5f * t * t * t)) * Mathf.Pow(remain, 2.2f) + t;
                    
                    iIconImage.transform.localScale = Vector3.LerpUnclamped(iStartScale, iEndScale, ease);
                    yield return null;
                }
                iIconImage.transform.localScale = iEndScale;
            }
            
            yield return new WaitForSeconds(0.5f);
        }

        // Show continue button
        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
        }
    }

    private void LoadNextScene()
    {
        string nextScene = ProgressManager.GetNextSceneName();
        Debug.Log($"[ProgressSceneController] Loading next scene: {nextScene}");
        SceneManager.LoadScene(nextScene);
    }

    // Optional: Dev testing function you can map to a UI button
    public void DebugResetProgress()
    {
        ProgressManager.ResetProgress();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
