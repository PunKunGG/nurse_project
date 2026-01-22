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
    public void ShowResult(bool isSuccess)
    {
        panelRoot.SetActive(true);

        if (isSuccess)
        {
            // --- กรณีผ่าน ---
            statusIcon.sprite = passSprite;
            titleText.text = "<color=green>ภารกิจสำเร็จ!</color>";
            msgText.text = "ยินดีด้วย คุณมีความรู้ความเข้าใจที่ถูกต้อง";
            buttonText.text = "ไปด่านต่อไป >>";
            
            // ตั้งค่าปุ่มให้ไปฉากต่อไป
            onButtonClickAction = () => {
                Debug.Log("Loading Next Scene: " + nextSceneName);
                if(!string.IsNullOrEmpty(nextSceneName))
                    SceneManager.LoadScene(nextSceneName);
            };
        }
        else
        {
            // --- กรณีไม่ผ่าน ---
            statusIcon.sprite = failSprite;
            titleText.text = "<color=red>ภารกิจล้มเหลว</color>";
            msgText.text = "ไม่ต้องเสียใจ ลองทบทวนเนื้อหาแล้วทำใหม่นะ";
            buttonText.text = "ลองอีกครั้ง";

            // ตั้งค่าปุ่มให้ Reload ฉากเดิม
            onButtonClickAction = () => {
                Debug.Log("Reloading Current Scene...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            };
        }
    }

    void OnBtnClicked()
    {
        // ทำคำสั่งที่ฝากไว้ (โหลดฉากใหม่ หรือ รีเซ็ต)
        onButtonClickAction?.Invoke();
    }
}