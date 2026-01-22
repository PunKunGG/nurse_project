// ExamTrigger.cs
using UnityEngine;

public class ExamTrigger : MonoBehaviour
{
    [SerializeField] private ImmobilityStageManager stageManager;

    private void Reset()
    {
        stageManager = FindFirstObjectByType<ImmobilityStageManager>();
    }
    private void OnMouseDown()
    {
        if (!stageManager) return;
        stageManager.OnExamTriggerClicked(gameObject);
    }
}
