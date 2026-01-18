using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DropZone2D : MonoBehaviour
{
    [SerializeField] private ImmobilityStageManager stageManager;
    [SerializeField] private Transform snapPoint;
    [SerializeField] private Draggable2D pillowDrag;
    [SerializeField] private Collider2D pillowCollider;

    private bool completed;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (completed) return;
        if (!pillowCollider) return;

        if (other == pillowCollider)
        {
            completed = true;
            Debug.Log("Pillow entered drop zone");

            // Snap
            if (snapPoint) other.transform.position = snapPoint.position;

            // Lock pillow
            if (pillowDrag) pillowDrag.DisableDragging();

            // Notify
            if (stageManager) stageManager.OnPillowPlacedCorrect();

        }
    }
}
