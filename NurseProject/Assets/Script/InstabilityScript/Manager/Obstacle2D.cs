using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Obstacle2D : MonoBehaviour
{
    private enum State
    {
        Unselected,
        Selected,
        Fixed
    }

    [Header("State (Debug)")]
    [SerializeField] private State state = State.Unselected;

    [Header("References")]
    [SerializeField] private InstabilityManager manager;

    [Header("Knowledge Message")]
    [TextArea(2, 6)]
    [SerializeField] private string knowledgeMessage;

    [Header("Visuals")]
    [SerializeField] private GameObject obstacleVisual; // hazard visual
    [SerializeField] private GameObject fixedVisual;    // fixed visual
    [SerializeField] private GameObject outlineVisual;  // outline/highlight (optional)

    private void Awake()
    {
        ApplyVisuals();
    }

    private void OnMouseDown()
    {
        HandleClick();
    }

    private void HandleClick()
    {
        if (state == State.Fixed) return;

        if (state == State.Unselected)
        {
            // First click: show outline only
            state = State.Selected;
            ApplyVisuals();
            return;
        }

        if (state == State.Selected)
        {
            // Second click: confirm -> fix
            Fix();
        }
    }

    private void Fix()
    {
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
        bool isSelected = (state == State.Selected);

        if (obstacleVisual != null) obstacleVisual.SetActive(!isFixed);
        if (fixedVisual != null) fixedVisual.SetActive(isFixed);

        // Outline shows only when selected and not fixed
        if (outlineVisual != null) outlineVisual.SetActive(isSelected && !isFixed);
    }

    // Optional: allow manager to clear selection globally (if you want only one selection at a time)
    public void ClearSelection()
    {
        if (state != State.Selected) return;
        state = State.Unselected;
        ApplyVisuals();
    }
}
