using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle2D : MonoBehaviour
{
    [Header("State")]
    [SerializeField] private bool isFixed = false;

    [Header("References")]
    [SerializeField] private InstabilityManager manager;

    [Header("Knowledge message (short for prototype)")]
    [TextArea(2, 6)]
    [SerializeField] private string knowledgeMessage;

    [Header("Visual after fix")]
    [SerializeField] private GameObject fixedVisual; // optional: enable after fix
    [SerializeField] private GameObject ObstacleVisual; // optional: disable after fix

    private void Reset()
    {
        // Ensure collider is a trigger? Not necessary for click.
        // Keep as normal collider for raycast.
        GetComponent<Collider2D>().isTrigger = false;
    }

    private void Awake()
    {
        ApplyVisual();
    }

    private void OnMouseDown()
    {
        Debug.Log($"[Obstacle2D] OnMouseDown fired on: {name}", this);
        TryFix();
    }

    public void TryFix()
    {
        Debug.Log($"[Obstacle2D] TryFix called on: {name} | isFixed={isFixed}", this);

        if (isFixed) return;
        isFixed = true;

        ApplyVisual();

        if (manager != null)
        {
            Debug.Log("[Obstacle2D] Manager exists, calling OnObstacleFixed()", this);
            manager.OnObstacleFixed(knowledgeMessage);
        }
        else
        {
            Debug.LogWarning("[Obstacle2D] Manager is NULL (visual should still toggle).", this);
        }
    }

    private void ApplyVisual()
    {
        if (ObstacleVisual != null) ObstacleVisual.SetActive(!isFixed);
        if (fixedVisual != null) fixedVisual.SetActive(isFixed);
    }
}
