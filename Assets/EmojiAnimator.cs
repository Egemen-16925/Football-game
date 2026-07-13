using UnityEngine;

public class EmojiAnimator : MonoBehaviour
{
    [Header("Hover (Floating) Settings")]
    public float hoverSpeed = 1.5f;       // Slow hover frequency
    public float hoverAmplitude = 0.06f;  // Subtle vertical floating amount

    [Header("Glow (Shining) Settings")]
    public float glowSpeed = 1.2f;        // Glowing cycle frequency
    public float maxGlowAlpha = 0.65f;     // Maximum transparency of the glow
    public Color glowColor = new Color(1f, 0.92f, 0.65f, 1f); // Warm soft gold glow
    public float glowScaleFactor = 1.4f;  // Size of the glow relative to the emoji

    private Vector3 startPos;
    private float phaseOffset;
    private SpriteRenderer glowRenderer;

    void Start()
    {
        // 1. Record starting position
        startPos = transform.position;

        // 2. Randomize phase offset so emojis float and pulse out of sync (organic feel)
        phaseOffset = Random.Range(0f, 2f * Mathf.PI);

        // 3. Generate a soft radial glow sprite dynamically
        Texture2D glowTexture = CreateRadialGlowTexture(32);
        Sprite glowSprite = Sprite.Create(glowTexture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));

        // 4. Create child GameObject for the glow overlay
        GameObject glowChild = new GameObject("GlowOverlay");
        glowChild.transform.SetParent(transform, false);
        
        // Position it slightly behind the emoji along Z-axis (local Z = 0.05)
        glowChild.transform.localPosition = new Vector3(0f, 0f, 0.05f);
        glowChild.transform.localRotation = Quaternion.identity;
        glowChild.transform.localScale = new Vector3(glowScaleFactor, glowScaleFactor, 1f);

        // Configure SpriteRenderer for the glow
        glowRenderer = glowChild.AddComponent<SpriteRenderer>();
        glowRenderer.sprite = glowSprite;
        glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, 0f);
        glowRenderer.sortingOrder = 4; // Draw behind the emoji (sortingOrder 5)
    }

    void Update()
    {
        // 1. Hover animation (sine wave floating)
        float hoverOffset = Mathf.Sin(Time.time * hoverSpeed + phaseOffset) * hoverAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + hoverOffset, startPos.z);

        // 2. Glow cycle (soft pulsing swell followed by quiet period)
        float cycle = (Time.time * glowSpeed + phaseOffset) % (2f * Mathf.PI);
        float sineVal = Mathf.Sin(cycle);
        
        // If sineVal > 0, do a soft glow pulse; otherwise keep it dark
        float alpha = (sineVal > 0f) ? Mathf.Pow(sineVal, 3f) * maxGlowAlpha : 0f;
        
        if (glowRenderer != null)
        {
            glowRenderer.color = new Color(glowColor.r, glowColor.g, glowColor.b, alpha);
        }
    }

    private Texture2D CreateRadialGlowTexture(int size)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        float center = size / 2f - 0.5f;
        Color[] colors = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float normDist = dist / (size / 2f);
                float alpha = Mathf.Clamp01(1f - normDist);
                
                // Apply a cubic falloff for a soft warm edge drop-off
                alpha = alpha * alpha * alpha;

                colors[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(colors);
        tex.Apply();
        return tex;
    }
}
