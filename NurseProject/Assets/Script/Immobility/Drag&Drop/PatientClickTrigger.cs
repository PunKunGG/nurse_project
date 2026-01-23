using UnityEngine;

public class PatientClickTrigger : MonoBehaviour
{
    [SerializeField] private ImmobilityStageManager stageManager;

    private void Reset()
    {
        stageManager = FindFirstObjectByType<ImmobilityStageManager>();
    }

    private void OnMouseDown()
    {
        if (!stageManager) return;
        stageManager.OnPatientClicked();
    }
}
