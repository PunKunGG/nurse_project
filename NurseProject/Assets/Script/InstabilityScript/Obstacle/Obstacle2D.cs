using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle2D : MonoBehaviour
{
    private enum State { Unfixed, Fixed }

    [Header("State (Debug)")]
    [SerializeField] private State state = State.Unfixed;

    [Header("References")]
    [SerializeField] private InstabilityStageManager manager;

    [Header("Knowledge Message")]
    [SerializeField] private string itemName;
    [TextArea(2, 6)]
    [SerializeField] private string knowledgeMessage;

    [Header("Visuals (Multiple)")]
    [SerializeField] private GameObject[] hazardVisuals;   // light ON, switch idle, etc.
    [SerializeField] private GameObject[] fixedVisuals;    // light OFF, switch flipped, etc.
    [SerializeField] private GameObject[] outlineVisuals;  // hover outline (switch + light)

    private void Awake() => ApplyVisuals();

    private void OnMouseDown() => TryFix();

    // if switch/light have their own collider, call this from a proxy
    public void OnProxyClick() => TryFix();

    private void TryFix()
    {
        if (state == State.Fixed) return;
        if (manager != null && manager.IsUIBlockingInput) return;

        state = State.Fixed;
        ApplyVisuals();

        if (manager != null)
        {
            string msg = string.IsNullOrWhiteSpace(knowledgeMessage)
                ? "[Knowledge message not set]"
                : knowledgeMessage;

            manager.OnObstacleFixed(itemName, msg);
        }
        else
        {
            Debug.LogWarning($"[Obstacle2D] Manager not assigned on {name}", this);
        }
    }

    private void ApplyVisuals()
    {
        bool isFixed = (state == State.Fixed);

        SetActiveAll(hazardVisuals, !isFixed);
        SetActiveAll(fixedVisuals, isFixed);

        // outline and floating removed per user request
        SetActiveAll(outlineVisuals, false);
    }

    private static void SetActiveAll(GameObject[] gos, bool active)
    {
        if (gos == null) return;
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i] != null) gos[i].SetActive(active);
        }
    }
}
