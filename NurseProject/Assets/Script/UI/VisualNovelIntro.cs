using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using System.Collections;

public class VisualNovelIntro : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelRoot;
    public Image characterImage;
    public TextMeshProUGUI dialogueText;
    public Button nextButton;
    public TextMeshProUGUI nextButtonText;

    [Header("Content (Defaults)")]
    public Sprite defaultCharacterSprite;
    [TextArea(3, 10)]
    public string[] dialoguePages; // Supports multiple pages of text

    [Header("Settings")]
    public float typeDelay = 0.05f; // Typing speed

    [Header("Events")]
    public UnityEvent OnIntroFinished;

    private int currentPage = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private void Start()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextClicked);
        }

        // Auto start if active
        // StartIntro(); 
    }

    /// <summary>
    /// Starts the visual novel sequence. Provide custom text if you want, 
    /// or pass null to use the dialoguePages configured in inspector.
    /// </summary>
    public void StartIntro(string[] customPages = null, Sprite customSprite = null)
    {
        if (panelRoot != null) panelRoot.SetActive(true);

        if (customPages != null && customPages.Length > 0)
        {
            dialoguePages = customPages;
        }

        if (customSprite != null && characterImage != null)
        {
            characterImage.sprite = customSprite;
        }
        else if (defaultCharacterSprite != null && characterImage != null)
        {
            characterImage.sprite = defaultCharacterSprite;
        }

        // Add a default page if nothing is set in the inspector so the script doesn't break
        if (dialoguePages == null || dialoguePages.Length == 0)
        {
            dialoguePages = new string[] { "สวัสดีค่ะ วันนี้เรามีเคสผู้ป่วยติดเตียงที่ต้องดูแล สิ่งแรกที่คุณต้องทำคือการพลิกตัวผู้ป่วยเพื่อประเมินความเสี่ยงของการเกิดแผลกดทับนะคะ ลองคลิกที่ตัวผู้ป่วยเพื่อเริ่มกันเลยค่ะ!" };
        }

        currentPage = 0;
        DisplayPage(currentPage);
    }

    private void DisplayPage(int index)
    {
        if (index >= dialoguePages.Length)
        {
            FinishIntro();
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialoguePages[index]));
        
        // Update button text
        if (nextButtonText != null)
        {
            nextButtonText.text = (index == dialoguePages.Length - 1) ? "เริ่มเกม" : "หน้าถัดไป";
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            
            // Skip delay if it's just a space
            if (c != ' ')
                yield return new WaitForSeconds(typeDelay);
        }

        isTyping = false;
    }

    private void OnNextClicked()
    {
        // If it's still typing, force complete the text instantly
        if (isTyping)
        {
            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            dialogueText.text = dialoguePages[currentPage];
            isTyping = false;
        }
        else
        {
            // Move to next page
            currentPage++;
            DisplayPage(currentPage);
        }
    }

    private void FinishIntro()
    {
        if (panelRoot != null) panelRoot.SetActive(false);
        OnIntroFinished?.Invoke();
    }
}
