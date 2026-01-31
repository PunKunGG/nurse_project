using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InanitionTarget : MonoBehaviour
{
    [Header("Wiring")]
    public InanitionStageManager manager;
    public GameObject redCircleObj; // รูปวงกลมสีแดง (ซ่อนไว้ก่อน)

    private Button btn;

    void Start()
    {
        btn = GetComponent<Button>();
        
        // ซ่อนวงกลมแดงตอนเริ่ม
        if (redCircleObj) redCircleObj.SetActive(false);
        
        // ผูกปุ่ม
        btn.onClick.AddListener(OnClicked);
        
        // หา Manager อัตโนมัติถ้าลืมลาก
        if (!manager) manager = FindObjectOfType<InanitionStageManager>();
    }

    void OnClicked()
    {
        // 1. โชว์วงกลมแดง
        if (redCircleObj) redCircleObj.SetActive(true);

        // 2. แจ้ง Manager ว่าเจอแล้ว
        if (manager) manager.OnTargetFound();

        // 3. ปิดปุ่มตัวเอง (จะได้กดซ้ำไม่ได้)
        btn.interactable = false; 
    }
}