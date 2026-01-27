using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))] // บังคับใส่ Rigidbody2D
public class Draggable2D : MonoBehaviour
{
    [SerializeField] private Camera worldCamera;
    [SerializeField] private bool lockZ = true;
    
    // สถานะ
    public bool isLocked = true; // เริ่มต้นล็อคไว้ (Manager จะมาปลด)
    private bool dragging = false;
    private Vector3 offsetWorld;
    
    // เก็บตำแหน่งเริ่มต้นเผื่อเด้งกลับ
    private Vector3 startPos;

    private void Awake()
    {
        if (!worldCamera) worldCamera = Camera.main;
        startPos = transform.position;
        
        // Setup Physics เบื้องต้นกันลืม ไม่รุ้เขียนไว้ก่อน
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic; 
        GetComponent<Collider2D>().isTrigger = false; // ตัวหมอนต้องไม่เป็น Trigger (ให้ DropZone เป็น Trigger)
    }

    private void Update()
    {
        if (isLocked) return; // ถ้าล็อคอยู่ ห้ามทำอะไรเลย

        if (Input.GetMouseButtonDown(0))
        {
            CheckStartDrag(Input.mousePosition);
        }
        else if (dragging && Input.GetMouseButton(0))
        {
            MoveObject(Input.mousePosition);
        }
        else if (dragging && Input.GetMouseButtonUp(0))
        {
            dragging = false;
            // ถ้าปล่อยมือแล้วยังไม่เข้า DropZone (ยังไม่โดนล็อคกลับ) ให้เด้งกลับที่เดิม
            if (!isLocked) 
            {
                transform.position = startPos;
            }
        }
    }

    private void CheckStartDrag(Vector2 screenPos)
    {
        Vector3 w = worldCamera.ScreenToWorldPoint(screenPos);
        w.z = transform.position.z;
        
        Collider2D hit = Physics2D.OverlapPoint(w);
        if (hit != null && hit.gameObject == gameObject)
        {
            dragging = true;
            offsetWorld = transform.position - w;
        }
    }

    private void MoveObject(Vector2 screenPos)
    {
        Vector3 w = worldCamera.ScreenToWorldPoint(screenPos);
        Vector3 target = w + offsetWorld;
        if (lockZ) target.z = transform.position.z;
        transform.position = target;
    }

    public void DisableDragging()
    {
        dragging = false; // หยุดลากทันที
        isLocked = true;  // ล็อคถาวร (เพราะวางเสร็จแล้ว)
        // จัดตำแหน่ง Z หรือ Layer ตรงนี้เพิ่มได้ถ้าต้องการให้หมอนอยู่ข้างหลัง/ข้างหน้า
    }
}