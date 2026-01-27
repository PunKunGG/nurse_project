using UnityEngine;

public class EnvironmentDebugger : MonoBehaviour
{
    public EnvironmentStageManager manager;
    private GUIStyle style;

    void Start()
    {
        if (!manager) manager = FindFirstObjectByType<EnvironmentStageManager>();

        // ตั้งค่าตัวหนังสือให้ใหญ่อ่านง่าย
        style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.yellow;
    }

    void OnGUI()
    {
        if (manager == null) return;

        // สร้างกล่องพื้นหลัง
        GUI.Box(new Rect(10, 10, 450, 350), "Environment Stage Debugger");

        GUILayout.BeginArea(new Rect(20, 40, 430, 330));

        // --- SECTION 1: แสดงสถานะ (Monitor) ---
        string currentState = GetCurrentState();
        GUILayout.Label($"Current Phase: <color=white>{currentState}</color>", style);
        GUILayout.Space(5);
        
        // แสดงจำนวนของที่แก้ไปแล้ว
        // (ต้องแก้ตัวแปร fixedHazards ใน Manager เป็น public ก่อนนะ ถึงจะเห็นบรรทัดนี้)
        GUILayout.Label($"Hazards Fixed: <color=cyan>{manager.fixedHazards} / {manager.totalHazards}</color>", style);
        
        GUILayout.Space(15);
        GUILayout.Label("--- CHEAT CODES ---", style);

        // --- SECTION 2: ปุ่มกดข้าม (Cheats) ---

        // 1. ปุ่มข้าม Cutscene
        if (manager.cutscenePanel.activeSelf)
        {
            if (GUILayout.Button("Skip Cutscene >>"))
            {
                manager.cutscenePanel.SetActive(false);
                if(manager.quizPanel) manager.quizPanel.SetActive(true);
            }
        }

        // 2. ปุ่มข้าม Quiz
        if (manager.quizPanel.activeSelf)
        {
            if (GUILayout.Button("Auto-Win Quiz >>"))
            {
                manager.OnQuizCompleted();
            }
        }

        // 3. ปุ่มช่วยหาของ (Gameplay)
        // จะโชว์เฉพาะตอนเข้าโหมดหาของแล้ว
        if (!manager.cutscenePanel.activeSelf && !manager.quizPanel.activeSelf)
        {
            if (GUILayout.Button("Fix 1 Random Hazard"))
            {
                FixRandomHazard();
            }

            if (GUILayout.Button("FIX ALL (Instant Win)"))
            {
                FixAllHazards();
            }
        }

        // 4. ปุ่ม Restart
        GUILayout.Space(10);
        if (GUILayout.Button("Reload Scene"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        GUILayout.EndArea();
    }

    // ฟังก์ชันช่วยเช็คสถานะเพื่อเอามาโชว์
    string GetCurrentState()
    {
        if (manager.cutscenePanel.activeSelf) return "Watching Cutscene";
        if (manager.quizPanel.activeSelf) return "Answering Quiz";
        if (manager.fixedHazards >= manager.totalHazards && manager.totalHazards > 0) return "Level Complete!";
        return "Gameplay (Finding Hazards)";
    }

    // ฟังก์ชันโกง: สุ่มแก้ของ 1 ชิ้น
    void FixRandomHazard()
    {
        // หาของทั้งหมดในฉากที่มี Script HazardObject
        HazardObject[] allHazards = FindObjectsByType<HazardObject>(FindObjectsSortMode.None);
        
        foreach (var hazard in allHazards)
        {
            // ถ้าชิ้นนี้ยังไม่ถูกแก้ (เช็คโดยดูว่า Sprite เปลี่ยนหรือยัง หรือดูตัวแปรภายใน)
            // เพื่อความง่าย เราจะสั่ง Fix เลย ถ้ามัน Fix แล้ว Script HazardObject มันจะกันเอง
            hazard.ForceFix(); // ต้องไปเพิ่มฟังก์ชันนี้ใน HazardObject
            break; // แก้ทีละอันพอ
        }
    }

    // ฟังก์ชันโกง: แก้หมดเลย
    void FixAllHazards()
    {
        HazardObject[] allHazards = FindObjectsByType<HazardObject>(FindObjectsSortMode.None);
        foreach (var hazard in allHazards)
        {
            hazard.ForceFix();
        }
    }
}