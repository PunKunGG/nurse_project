using UnityEngine;

public class ExamTrigger : MonoBehaviour
{
    [SerializeField] private ImmobilityStageManager stageManager;

    private void Reset()
    {
        // Helpful auto-wiring in editor if manager exists
        stageManager = FindFirstObjectByType<ImmobilityStageManager>();
    }

    private void OnMouseDown()
    {
        if (!stageManager) return;

        // If UI is already blocking input, ignore
        if (stageManager.IsUIBlockingInput) return;

        stageManager.StartGradeQuestion();
    }
}