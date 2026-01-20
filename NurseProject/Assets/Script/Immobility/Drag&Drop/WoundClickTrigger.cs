using UnityEngine;

public class WoundClickTrigger : MonoBehaviour
{
    void OnMouseDown()
    {
        // เรียกฟังก์ชันใน Manager
        FindObjectOfType<ImmobilityStageManager>().OnWoundClicked();
    }
}