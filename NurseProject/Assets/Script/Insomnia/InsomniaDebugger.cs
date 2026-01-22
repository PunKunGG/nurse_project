using UnityEngine;

public class InsomniaDebugger : MonoBehaviour
{
    public InsomniaQuizManager manager; // อ้างอิง Script ใหม่
    private GUIStyle style;

    void Start()
    {
        if (!manager) manager = FindObjectOfType<InsomniaQuizManager>();

        style = new GUIStyle();
        style.fontSize = 22;
        style.normal.textColor = Color.cyan; // เปลี่ยนสีหน่อยจะได้แยกออก
    }

    void OnGUI()
    {
        if (manager == null) return;

        // พื้นหลัง Box
        GUI.Box(new Rect(10, 10, 350, 320), "Insomnia Stage Debugger");

        GUILayout.BeginArea(new Rect(20, 40, 330, 300));

        // --- Monitor ---
        GUILayout.Label($"Question: {manager.GetCurrentQuestionNum()} / 4", style);
        GUILayout.Label($"Score: {manager.GetScore()}", style);
        
        GUILayout.Space(10);
        GUILayout.Label("--- CHEATS ---", style);

        // --- Buttons ---
        if (GUILayout.Button("Auto Answer: CORRECT"))
        {
            manager.ForceAnswer(true);
        }

        if (GUILayout.Button("Auto Answer: WRONG"))
        {
            manager.ForceAnswer(false);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("INSTANT WIN (Pass)"))
        {
            manager.ForceEndGame(true);
        }

        if (GUILayout.Button("INSTANT FAIL (Retry)"))
        {
            manager.ForceEndGame(false);
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Reload Scene"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        GUILayout.EndArea();
    }
}