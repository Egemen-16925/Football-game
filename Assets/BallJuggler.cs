using UnityEngine;

public class BallJuggler : MonoBehaviour
{
    public float duration = 1.5f;        // Time for one full juggle loop
    public float forwardDistance = 0.75f; // How far forward the ball goes (X increase)
    public float peakHeight = 0.84f;      // How high the ball rises (Y increase)
    public float rotationSpeed = 360f;    // Degrees per second
    public Vector3 rotationAxis = new Vector3(0f, 0f, -1f); // Spin axis (Z-axis backspin)

    private float timer = 0f;
    private Vector3 startPos;

    void Start()
    {
        // Record the original world position of the ball
        startPos = transform.position;
    }

    void Update()
    {
        // Update timer
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            timer -= duration;
        }

        float u = timer / duration;

        // X progress: goes from 0 (at start) to 1 (at max X) and back to 0
        float xPct = Mathf.Sin(u * Mathf.PI);

        // Position: X and Y change, Z remains at original position
        float x = startPos.x + forwardDistance * xPct;

        // Symmetric parabola: Y starts at startPos.y, rises by peakHeight at xPct=0.5, and returns to startPos.y at xPct=1.0
        float y = startPos.y + 4f * peakHeight * xPct * (1f - xPct);

        float z = startPos.z;

        transform.position = new Vector3(x, y, z);

        // Rotation: Spin according to physics (constant angular speed)
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
