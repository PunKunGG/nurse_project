using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Draggable2D : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private bool lockZ = true;

    private bool dragging;
    private Vector3 offsetWorld;
    private int activeTouchId = -1;

    private void Awake()
    {
        if (!worldCamera) worldCamera = Camera.main;
    }

    private void Update()
    {
        // Touch drag has priority on mobile
        if (Input.touchCount > 0)
        {
            HandleTouch();
            return;
        }

        // Mouse drag fallback (PC/editor)
        HandleMouse();
    }

    private void HandleTouch()
    {
        Touch t = Input.GetTouch(0);

        if (t.phase == TouchPhase.Began)
        {
            Vector3 w = ScreenToWorld(t.position);
            Collider2D hit = Physics2D.OverlapPoint(w);
            if (hit != null && hit.gameObject == gameObject)
            {
                dragging = true;
                activeTouchId = t.fingerId;

                offsetWorld = transform.position - w;
            }
        }
        else if (dragging && t.fingerId == activeTouchId && (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary))
        {
            Vector3 w = ScreenToWorld(t.position);
            Vector3 target = w + offsetWorld;
            if (lockZ) target.z = transform.position.z;
            transform.position = target;
        }
        else if (dragging && t.fingerId == activeTouchId && (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled))
        {
            dragging = false;
            activeTouchId = -1;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 w = ScreenToWorld(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(w);
            if (hit != null && hit.gameObject == gameObject)
            {
                dragging = true;
                offsetWorld = transform.position - w;
            }
        }
        else if (dragging && Input.GetMouseButton(0))
        {
            Vector3 w = ScreenToWorld(Input.mousePosition);
            Vector3 target = w + offsetWorld;
            if (lockZ) target.z = transform.position.z;
            transform.position = target;
        }
        else if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }
    }

    private Vector3 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 s = new Vector3(screenPos.x, screenPos.y, 0f);
        Vector3 w = worldCamera.ScreenToWorldPoint(s);
        w.z = transform.position.z;
        return w;
    }

    public void DisableDragging()
    {
        dragging = false;
        enabled = false;
    }
}
