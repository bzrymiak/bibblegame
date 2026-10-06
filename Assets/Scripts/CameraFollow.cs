using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.15f;
    public Vector2 offset = new Vector2(2f, 1f);

    [Header("Auto scroll")]
    [Tooltip("Camera drives itself to the right instead of following the player")]
    public bool autoScroll = false;
    public float scrollSpeed = 4f;
    [Tooltip("Breathing room before the swarm starts moving")]
    public float startDelay = 1f;

    // stops the camera dropping down a pit with the player
    public bool lockY = false;
    public float fixedY = 0f;

    private Vector3 velocity;
    private float delayLeft;
    private float shakeTimeLeft;
    private float shakeStrength;
    private float shakeTotal;

    void Start()
    {
        delayLeft = startDelay;
    }

    void LateUpdate()
    {
        if (autoScroll)
        {
            AutoScroll();
        }
        else
        {
            if (target == null) return;

            float x = target.position.x + offset.x;
            float y = lockY ? fixedY : target.position.y + offset.y;

            Vector3 goal = new Vector3(x, y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
        }

        if (shakeTimeLeft > 0f)
        {
            // unscaled so the shake still runs during the hit stop freeze
            shakeTimeLeft -= Time.unscaledDeltaTime;

            // fade it out over the duration so it settles instead of stopping dead
            float falloff = Mathf.Clamp01(shakeTimeLeft / shakeTotal);
            Vector2 jitter = Random.insideUnitCircle * shakeStrength * falloff;
            transform.position += new Vector3(jitter.x, jitter.y, 0f);
        }
    }

    private void AutoScroll()
    {
        if (delayLeft > 0f)
        {
            delayLeft -= Time.deltaTime;
            return;
        }

        float y = lockY ? fixedY : transform.position.y;
        transform.position = new Vector3(
            transform.position.x + scrollSpeed * Time.deltaTime,
            y,
            transform.position.z);
    }

    public void Shake(float strength, float duration)
    {
        shakeStrength = strength;
        shakeTimeLeft = duration;
        shakeTotal = duration;
    }
}
