using UnityEngine;

public class ScrollingLinesManager : MonoBehaviour
{
    public float scrollSpeed = 0.5f;     // Speed of scrolling to the left
    public float dashSpacing = 0.3f;     // World space spacing of dashes
    public float lineThickness = 0.05f;  // Thickness of the lines
    public Color lineColor = Color.white;

    private Material sharedMaterial;
    private Vector2 currentOffset = Vector2.zero;

    void Start()
    {
        // 1. Locate characters to calculate spacing
        GameObject char1 = GameObject.Find("Character_Seq1_Folder1");
        GameObject char20 = GameObject.Find("Character_Seq20_Folder3");
        if (char1 == null || char20 == null)
        {
            Debug.LogError("ScrollingLinesManager: Characters not found.");
            return;
        }

        float y1 = char1.transform.position.y;
        float y20 = char20.transform.position.y;
        float gap = (y1 - y20) / 19f;

        // 2. Locate background bounds for line length
        GameObject bg = GameObject.Find("Adsız tasarım(9)_0");
        float lineLength = 12.96f; // Default fallback
        float centerX = -8.92f;
        if (bg != null)
        {
            var sr = bg.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                lineLength = sr.bounds.size.x;
                centerX = sr.bounds.center.x;
            }
        }

        // 3. Create dashed texture (8 pixels white, 8 pixels transparent)
        Texture2D texture = new Texture2D(16, 1, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Repeat;
        Color[] colors = new Color[16];
        for (int i = 0; i < 16; i++)
        {
            colors[i] = (i < 8) ? lineColor : new Color(lineColor.r, lineColor.g, lineColor.b, 0f);
        }
        texture.SetPixels(colors);
        texture.Apply();

        // 4. Create Sprite from texture (PPU configured for exact dashSpacing repetition)
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 16, 1), new Vector2(0.5f, 0.5f), 16f / dashSpacing);

        // 5. Create transparent URP Unlit Material
        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null)
        {
            shader = Shader.Find("Unlit/Transparent");
        }
        sharedMaterial = new Material(shader);
        sharedMaterial.SetTexture("_BaseMap", texture);
        sharedMaterial.SetTexture("_MainTex", texture);

        sharedMaterial.SetFloat("_Surface", 1f); // 1 = Transparent
        sharedMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        sharedMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        sharedMaterial.SetInt("_ZWrite", 0);
        sharedMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // 6. Calculate pixel size for vertical snapping to prevent aliasing/varying thickness
        float orthographicSize = 11.52f;
        float screenHeight = 1920f;
        Camera cam = Camera.main;
        if (cam != null)
        {
            orthographicSize = cam.orthographicSize;
            if (Screen.height > 0)
            {
                screenHeight = Screen.height;
            }
        }
        float pixelSize = (orthographicSize * 2f) / screenHeight;

        // 7. Generate 19 lines using Tiled SpriteRenderers snapped to pixel boundaries
        for (int i = 0; i < 19; i++)
        {
            // Position: Midpoint between character i and i+1, shifted up by 0.1 units
            float yPos = y1 - (i + 0.5f) * gap + 0.1f;
            
            // Snap to the nearest physical screen pixel to guarantee uniform thickness
            float snappedY = Mathf.Round(yPos / pixelSize) * pixelSize;
            
            GameObject line = new GameObject("DividerLine_Seq" + (i + 1));
            line.transform.SetParent(transform);
            
            // Set position (Z = 0.1)
            line.transform.position = new Vector3(centerX, snappedY, 0.1f);
            line.transform.rotation = Quaternion.identity;
            
            // Add SpriteRenderer in Tiled mode
            var sr = line.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.size = new Vector2(lineLength, lineThickness);
            sr.sharedMaterial = sharedMaterial;
            sr.sortingOrder = 3; // Render behind characters (Z = 0) and flags (Z = -1)
        }
    }

    void Update()
    {
        if (sharedMaterial == null) return;

        // Scroll offset to the left (increase X offset)
        currentOffset.x += scrollSpeed * Time.deltaTime;
        sharedMaterial.SetTextureOffset("_BaseMap", currentOffset);
        sharedMaterial.SetTextureOffset("_MainTex", currentOffset);
    }
}
