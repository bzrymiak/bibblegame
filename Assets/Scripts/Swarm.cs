using UnityEngine;

// the bees chasing bibble. they ride the left edge of the camera, so the
// scroll speed is their speed - getting caught by the edge is getting caught by them
public class Swarm : MonoBehaviour
{
    public static Swarm Instance { get; private set; }

    public Camera cam;
    public Transform player;

    [Tooltip("How far past the left edge they sit. negative pushes them off screen")]
    public float edgeOffset = 6f;
    public float verticalOffset = 0f;

    [Header("Following bibble")]
    public bool followPlayerY = true;
    [Tooltip("Higher = laggier. the lag is what makes them look like they're chasing")]
    public float followSmoothTime = 0.8f;
    [Tooltip("Keeps them from drifting off the top or bottom of the screen")]
    public float screenEdgePadding = 10f;

    [Header("Bobbing")]
    public float bobHeight = 2.5f;
    public float bobSpeed = 2.2f;
    [Tooltip("Spread so they don't all bob in sync")]
    public float phaseSpread = 1.1f;

    [Header("Retreat and return")]
    [Tooltip("How long they hang around at the start before backing off")]
    public float introVisibleTime = 6f;
    [Tooltip("How long they come back for after bibble takes a hit")]
    public float returnVisibleTime = 5f;
    [Tooltip("How far further left they sit while hidden. needs to clear the screen")]
    public float hiddenShift = -70f;
    public float slideSmoothTime = 0.7f;

    [Header("Drift")]
    [Tooltip("Small forward and back sway so the group feels alive")]
    public float driftAmount = 1.5f;
    public float driftSpeed = 1.4f;

    [Header("Sound")]
    [Tooltip("How far (in units) the bees need to slide in before the buzzing reaches full volume")]
    public float audibleRange = 6f;

    private Transform[] bees;
    private Vector3[] homes;
    private float followY;
    private float followVelocity;
    private float visibleTimer;
    private float hideShift;
    private float hideShiftVelocity;
    private bool soundSilenced;

    void Awake()
    {
        if (cam == null) cam = Camera.main;

        bees = new Transform[transform.childCount];
        homes = new Vector3[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            bees[i] = transform.GetChild(i);
            homes[i] = bees[i].localPosition;
        }

        followY = transform.position.y;
        Instance = this;
        visibleTimer = introVisibleTime;
    }

    // bibble clipped something, so they close back in for a few seconds
    public void Surge()
    {
        visibleTimer = returnVisibleTime;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        // stick to the left edge of whatever the camera can see
        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        float goalY = cam.transform.position.y + verticalOffset;

        if (followPlayerY && player != null)
        {
            // drift towards whatever height bibble is at, but lazily, so they
            // trail him up and down instead of snapping
            goalY = player.position.y + verticalOffset;

            float top = cam.transform.position.y + cam.orthographicSize - screenEdgePadding;
            float bottom = cam.transform.position.y - cam.orthographicSize + screenEdgePadding;
            goalY = Mathf.Clamp(goalY, bottom, top);
        }

        followY = Mathf.SmoothDamp(followY, goalY, ref followVelocity, followSmoothTime);

        if (visibleTimer > 0f) visibleTimer -= Time.deltaTime;

        // slide off to the left when the timer runs out, slide back in when it's topped up
        float targetShift = visibleTimer > 0f ? 0f : hiddenShift;
        hideShift = Mathf.SmoothDamp(hideShift, targetShift, ref hideShiftVelocity, slideSmoothTime);

        transform.position = new Vector3(leftEdge + edgeOffset + hideShift, followY, 0f);

        for (int i = 0; i < bees.Length; i++)
        {
            float phase = i * phaseSpread;
            float bob = Mathf.Sin(Time.time * bobSpeed + phase) * bobHeight;
            float drift = Mathf.Cos(Time.time * driftSpeed + phase) * driftAmount;

            bees[i].localPosition = homes[i] + new Vector3(drift, bob, 0f);
        }

        // 0 when they've slid fully off screen, 1 when they're at their home position
        if (!soundSilenced && AudioManager.Instance != null)
        {
            float audible = Mathf.InverseLerp(-audibleRange, 0f, hideShift);
            AudioManager.Instance.SetLoop(AudioManager.SoundType.Bees, audible);
        }
    }

    void OnDisable()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(AudioManager.SoundType.Bees);
    }

    // call when the lose (or win) sequence takes over the audio
    public void SilenceSound()
    {
        soundSilenced = true;
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopLoop(AudioManager.SoundType.Bees);
    }
}
