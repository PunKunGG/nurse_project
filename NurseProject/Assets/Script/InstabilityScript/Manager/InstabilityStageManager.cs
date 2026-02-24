// InstabilityManager.cs
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InstabilityStageManager : MonoBehaviour
{
    [Header("Progress")]
    [SerializeField] private int totalHazards = 6;
    [SerializeField] private TMP_Text progressText;
    private int fixedCount = 0;

    [Header("Visual Novel Intro")]
    [SerializeField] private VisualNovelIntro introNovel;
    [TextArea(3, 10)]
    [SerializeField] private string[] introDialogue = new string[] {
        "สวัสดีค่ะ วันนี้เราจะมาดูแลคุณตาที่บ้านกันนะคะ สภาพแวดล้อมที่ไม่เหมาะสมอาจทำให้เกิดอุบัติเหตุหกล้มได้ ลองสำรวจจุดเสี่ยงต่างๆ แล้วแก้ไขให้ถูกต้องดูค่ะ!"
    };

    public bool IsUIBlockingInput { get; private set; }

    [Header("Knowledge Popup (Animated)")]
    [SerializeField] private KnowledgePopupUI knowledgePopup;

    [Header("Result UI")]
    public UniversalResultUI resultUI; // ลาก UniversalResultUI มาใส่ตรงนี้

    private bool pendingWin = false;

    private void Awake()
    {
        UpdateProgressUI();

        IsUIBlockingInput = true;

        if (introNovel != null)
        {
            introNovel.OnIntroFinished.AddListener(OnIntroFinished);
            introNovel.StartIntro(introDialogue);
        }
        else
        {
            IsUIBlockingInput = false;
        }
    }

    private void OnIntroFinished()
    {
        if (introNovel != null)
            introNovel.OnIntroFinished.RemoveListener(OnIntroFinished);
            
        IsUIBlockingInput = false;
        Debug.Log("Instability intro finished, game unblocked.");
    }

    /// <summary>
    /// Called by each obstacle when fixed.
    /// </summary>
    public void OnObstacleFixed(string knowledgeMessage)
    {
        fixedCount = Mathf.Clamp(fixedCount + 1, 0, totalHazards);
        UpdateProgressUI();

        if (knowledgePopup != null && !string.IsNullOrWhiteSpace(knowledgeMessage))
            knowledgePopup.Show(knowledgeMessage);

        if (fixedCount >= totalHazards)
        {
            if (knowledgePopup != null)
            {
                pendingWin = true;
                knowledgePopup.OnClosed += HandleKnowledgeClosedForWin;
            }
            else
            {
                Win();
            }
        }

    }

    private void HandleKnowledgeClosedForWin()
    {
        knowledgePopup.OnClosed -= HandleKnowledgeClosedForWin;

        if (!pendingWin) return;
        pendingWin = false;

        Win();
    }


    private void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"{fixedCount}/{totalHazards}";
    }

    private void Win()
    {
        if (resultUI)
        {
            resultUI.ShowResult(
                true,
                "<color=green>ยอดเยี่ยม!</color>",
                "คุณได้ช่วยลดความเสี่ยงในการพลัดตกหกล้มของผู้สูงอายุ\nโดยการจัดสภาพแวดล้อมตามหลักการพยาบาลได้อย่างถูกต้อง",
                "ไปด่านต่อไป >>"
            );
        }
        else
        {
             Debug.LogWarning("UniversalResultUI not assigned!");
        }
    }

    private static void SetCanvasGroupVisible(CanvasGroup cg, bool visible)
    {
        if (cg == null) return;
        cg.alpha = visible ? 1f : 0f;
        cg.interactable = visible;
        cg.blocksRaycasts = visible;
    }
}

