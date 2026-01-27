using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle2D : MonoBehaviour
{
    private enum State { Unfixed, Fixed }

    [Header("State (Debug)")]
    [SerializeField] private State state = State.Unfixed;

    [Header("References")]
    [SerializeField] private InstabilityManager manager;

    [Header("Knowledge Message")]
    [TextArea(2, 6)]
    [SerializeField] private string knowledgeMessage;

    [Header("Visuals (Multiple)")]
    [SerializeField] private GameObject[] hazardVisuals;   // light ON, switch idle, etc.
    [SerializeField] private GameObject[] fixedVisuals;    // light OFF, switch flipped, etc.
    [SerializeField] private GameObject[] outlineVisuals;  // hover outline (switch + light)

    private bool isHovered;

    private void Awake() => ApplyVisuals();

    private void OnMouseEnter()
    {
        isHovered = true;
        ApplyVisuals();
    }

    private void OnMouseExit()
    {
        isHovered = false;
        ApplyVisuals();
    }

    private void OnMouseDown() => TryFix();

    // if switch/light have their own collider, call this from a proxy
    public void OnProxyClick() => TryFix();

    private void TryFix()
    {
        if (state == State.Fixed) return;

        state = State.Fixed;
        ApplyVisuals();

        if (manager != null)
        {
            string msg = string.IsNullOrWhiteSpace(knowledgeMessage)
                ? "[Knowledge message not set]"
                : knowledgeMessage;

            manager.OnObstacleFixed(msg);
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

        // outline ONLY when hovered, never when fixed
        SetActiveAll(outlineVisuals, isHovered && !isFixed);
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
