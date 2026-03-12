using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class InanitionHotspot : MonoBehaviour
{
    [Header("Settings")]
    public bool isCorrect;          // True = Risk Factor, False = Decoy
    public string id;               // Unique ID for this hotspot (optional, helpful for debugging)
    
    [Header("Visuals")]
    public GameObject highlightObj; // The red circle ring (assigned in inspector)
    
    [Header("Data")]
    [TextArea] public string tooltipText; // Optional description
    public string itemName;
    [TextArea(3, 10)]
    public string knowledgeMessage;

    private bool isFound = false;
    private InanitionGameManager manager;

    void Start()
    {
        manager = FindObjectOfType<InanitionGameManager>();
        
        // Ensure highlight is hidden at start
        if (highlightObj) highlightObj.SetActive(false);
    }

    void OnMouseDown()
    {
        // Prevent interaction if already found or game over
        if (isFound) return;
        if (manager && !manager.IsPlaying) return;

        // Visual feedback immediately? 
        // Logic says: Correct -> Show Red Circle. Wrong -> Lose Heart.
        
        if (manager)
        {
            manager.OnHotspotClicked(this);
        }
    }

    public void MarkAsFound()
    {
        isFound = true;
        if (highlightObj)
        {
            highlightObj.SetActive(true);
            
            // Try to animate via Image (UI)
            Image img = highlightObj.GetComponent<Image>();
            
            // If not UI Image but SpriteRenderer, create a runtime UI proxy
            if (img == null)
            {
                SpriteRenderer sr = highlightObj.GetComponent<SpriteRenderer>();
                if (sr)
                {
                    img = CreateRadialImageProxy(sr);
                    // Hide original sprite since proxy takes over
                    sr.enabled = false; 
                }
            }

            if (img)
            {
                StartCoroutine(AnimateCircle(img));
            }
        }
        
        // Disable collider to prevent further clicks
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;
    }

    private Image CreateRadialImageProxy(SpriteRenderer sr)
    {
        // Check if we already made one
        Transform existingProxy = sr.transform.Find("RadialProxy");
        if (existingProxy) return existingProxy.GetComponent<Image>();

        // Create Canvas (for World Space UI) on the object itself or a child?
        // Easiest is to make a child Canvas
        GameObject proxyObj = new GameObject("RadialProxy");
        proxyObj.transform.SetParent(sr.transform, false);
        proxyObj.transform.localPosition = Vector3.zero;
        proxyObj.transform.localScale = Vector3.one;
        proxyObj.layer = sr.gameObject.layer;

        Canvas canvas = proxyObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingLayerID = sr.sortingLayerID;
        canvas.sortingOrder = sr.sortingOrder + 1; // On top of original

        // Add Image
        GameObject verifyObj = new GameObject("Image");
        verifyObj.transform.SetParent(proxyObj.transform, false);
        Image img = verifyObj.AddComponent<Image>();
        img.sprite = sr.sprite;
        img.color = sr.color;
        
        // Match Size
        // Sprite size in world units = sprite.bounds.size (local to sprite pixels)
        // RectTransform sizeDelta should match sprite pixels if scale is 1
        RectTransform rt = img.rectTransform;
        if (sr.sprite)
        {
            float width = sr.sprite.rect.width; // pixels
            float height = sr.sprite.rect.height; // pixels
            float ppu = sr.sprite.pixelsPerUnit;
            
            // Image size = pixels. But world scale needs to be adjusted?
            // Canvas scaler? No, simple world space.
            // World Space Canvas Unit = 1 meter.
            // UI Image size 100x100 = 100x100 units? No, 100x100 pixels.
            // Scale factor needs to be 1 / PPU
            
            rt.sizeDelta = new Vector2(width, height);
            rt.localScale = new Vector3(1f/ppu, 1f/ppu, 1f);
        }

        return img;
    }

    private System.Collections.IEnumerator AnimateCircle(Image img)
    {
        // Setup for "Draw" animation
        img.type = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Radial360;
        img.fillOrigin = (int)Image.Origin360.Top;
        img.fillClockwise = true;
        
        float duration = 0.6f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            img.fillAmount = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        img.fillAmount = 1f;
    }

    public void ResetHotspot()
    {
        isFound = false;
        if (highlightObj) highlightObj.SetActive(false);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = true;
    }
}
