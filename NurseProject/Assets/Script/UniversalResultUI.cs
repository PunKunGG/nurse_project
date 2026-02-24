using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // จำเป็นสำหรับการเปลี่ยนฉาก

public class UniversalResultUI : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelRoot;      // ตัว Panel ใหญ่สุด
    public Image statusIcon;          // รูปไอคอน (ถ้วย/กากบาท)
    public TextMeshProUGUI titleText; // หัวข้อ
    public TextMeshProUGUI msgText;   // ข้อความบรรยาย
    public Button actionButton;       // ปุ่มกด
    public TextMeshProUGUI buttonText;// ข้อความในปุ่ม

    [Header("Settings - PASS")]
    public Sprite passSprite;         // รูปตอนผ่าน (เช่น หน้าหมอยิ้ม/เครื่องหมายถูก)
    public string nextSceneName;      // **ชื่อ Scene ต่อไปที่จะให้โหลด**

    [Header("Settings - FAIL")]
    public Sprite failSprite;         // รูปตอนไม่ผ่าน (เช่น หน้าเศร้า/กากบาท)

    // ตัวแปรเก็บ Action ที่จะทำเมื่อกดปุ่ม
    private System.Action onButtonClickAction;

    void Start()
    {
        // เริ่มมาซ่อนก่อนเสมอ
        if(panelRoot) panelRoot.SetActive(false);
        
        // ผูกปุ่มเข้ากับฟังก์ชัน
        if(actionButton) 
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnBtnClicked);
        }
    }

    // ฟังก์ชันพระเอก: สั่งเปิดหน้าต่าง (Manager อื่นๆ จะเรียกใช้ตัวนี้)
    // ฟังก์ชันพระเอก: สั่งเปิดหน้าต่าง (แบบ Universal ของจริง)
    public void ShowResult(bool isSuccess, string title, string message, string btnTxt, System.Action onNextAction = null)
    {
        Debug.Log($"[UniversalResultUI] ShowResult called — {(isSuccess ? "WIN ✔" : "LOSE ✘")} | Title: {title}");

        panelRoot.SetActive(true);

        // Ensure button is wired up and interactable (ป้องกัน Start() ไม่ทัน)
        if (actionButton)
        {
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnBtnClicked);
            actionButton.interactable = true;
        }

        // 1. Setup Image & Color
        if (isSuccess)
        {
            statusIcon.sprite = passSprite;
            // titleText.color = Color.green; // Optional: Force color
        }
        else
        {
            statusIcon.sprite = failSprite;
            // titleText.color = Color.red; // Optional: Force color
        }

        // 2. Setup Text
        titleText.text = title;
        msgText.text = message;
        buttonText.text = btnTxt;

        // 3. Setup Action
        if (onNextAction != null)
        {
            // ถ้าส่ง Action มาให้ใช้ Action นั้น
            onButtonClickAction = onNextAction;
        }
        else
        {
            // ถ้าไม่ส่งมา ให้ใช้ Default Logic (Load Next / Reload)
            if (isSuccess)
            {
                onButtonClickAction = () => {
                    // Report completion to ProgressManager
                    ProgressManager.CompleteCurrentStage(SceneManager.GetActiveScene().name);
                    
                    // Instead of loading the specific next scene directly, go to the Progress Scene
                    Debug.Log("Loading Progress Scene...");
                    SceneManager.LoadScene("ProgressScene");
                };
            }
            else
            {
                onButtonClickAction = () => {
                    Debug.Log("Reloading Current Scene...");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                };
            }
        }
    }

    void OnBtnClicked()
    {
        // ทำคำสั่งที่ฝากไว้ (โหลดฉากใหม่ หรือ รีเซ็ต)
        onButtonClickAction?.Invoke();
    }
}