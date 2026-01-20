using UnityEngine;

public class ImmobilityDebugger : MonoBehaviour
{
    // ลาก Manager มาใส่ตรงนี้
    public ImmobilityStageManager manager;
    
    // ตั้งค่าขนาดตัวหนังสือ Debug
    private GUIStyle style;

    void Start()
    {
        if(!manager) manager = FindObjectOfType<ImmobilityStageManager>();
        
        style = new GUIStyle();
        style.fontSize = 24;
        style.normal.textColor = Color.yellow; // สีตัวหนังสือ
    }

    void OnGUI()
    {
        if (manager == null) return;

        // วาดกล่องพื้นหลังสีดำจางๆ
        GUI.Box(new Rect(10, 10, 400, 300), "Debug Menu");

        GUILayout.BeginArea(new Rect(20, 40, 380, 280));

        // --- แสดงค่าตัวแปร (Status) ---
        GUILayout.Label($"Current State: <color=white>{manager.state}</color>", style);
        GUILayout.Space(10); // เว้นบรรทัด
        
        // --- ปุ่มกดข้าม (Cheat Buttons) ---
        if (GUILayout.Button("Force: Restart Level"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        GUILayout.Space(5);

        // ปุ่มจะโชว์เฉพาะตอนที่กดได้
        if (manager.state == ImmobilityStageManager.StageState.Idle)
        {
            if (GUILayout.Button("Force: Turn Patient")) manager.OnPatientClicked();
        }

        if (manager.state == ImmobilityStageManager.StageState.PatientTurned)
        {
            if (GUILayout.Button("Force: Click Wound")) manager.OnWoundClicked();
        }

        if (manager.state == ImmobilityStageManager.StageState.GradeQuestion)
        {
            if (GUILayout.Button("Force: Answer Grade Correct")) manager.OnGradeAnsweredCorrect();
        }

        if (manager.state == ImmobilityStageManager.StageState.PillowPlacement)
        {
            // อันนี้ยากหน่อยเพราะต้อง Simulate การวาง แต่สามารถข้ามไป State ต่อไปได้เลย
            if (GUILayout.Button("Force: Skip Pillow Placement")) manager.OnPillowPlacedCorrect();
        }
        
        GUILayout.EndArea();
    }
}