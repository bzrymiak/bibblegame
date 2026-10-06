using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Tooltip("Fall below this and it counts as falling off the map")]
    public float killY = -20f;

    [Tooltip("How often we remember where he was standing")]
    public float checkpointInterval = 0.25f;

    [Tooltip("Dropped back slightly left of the checkpoint so he isn't on top of whatever he fell off")]
    public float respawnOffsetY = 1f;

    private Player player;
    private Health health;
    private Rigidbody2D rb;
    private Vector2 lastSafeSpot;
    private float checkpointTimer;

    private void Awake()
    {
        player = GetComponent<Player>();
        health = GetComponent<Health>();
        rb = GetComponent<Rigidbody2D>();
        lastSafeSpot = transform.position;
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsPlaying) return;

        RememberSafeSpot();

        if (transform.position.y < killY)
            FellOff();
    }

    private void RememberSafeSpot()
    {
        // only remember spots where he was actually standing and in control
        if (player == null || !player.IsGrounded || player.IsStaggered) return;

        checkpointTimer -= Time.deltaTime;
        if (checkpointTimer > 0f) return;

        checkpointTimer = checkpointInterval;
        lastSafeSpot = transform.position;
    }

    private void FellOff()
    {
        // falling always costs a life, even mid invulnerability, otherwise
        // you can fall straight after a hit for free
        if (health != null) health.TakeDamage(transform.position, true);

        transform.position = new Vector2(lastSafeSpot.x, lastSafeSpot.y + respawnOffsetY);
        if (rb != null) rb.linearVelocity = Vector2.zero;
    }
}
