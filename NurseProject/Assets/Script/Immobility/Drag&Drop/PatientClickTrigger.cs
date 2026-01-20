using UnityEngine;

public class PatientClickTrigger : MonoBehaviour
{
    void OnMouseDown()
    {
        // เรียกฟังก์ชันใน Manager
        FindObjectOfType<ImmobilityStageManager>().OnPatientClicked();
    }
}