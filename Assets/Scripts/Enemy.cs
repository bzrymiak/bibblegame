using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public float speed = 20f;
    public Transform[] points;
    public float arriveDistance = 0.1f;

    private int i;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (points == null || points.Length == 0) return;

        float dx = points[i].position.x - rb.position.x;

        if (Mathf.Abs(dx) < arriveDistance)
        {
            i = (i + 1) % points.Length;
            dx = points[i].position.x - rb.position.x;
        }

        float dir = Mathf.Sign(dx);

        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);

        spriteRenderer.flipX = dir > 0f;
    }
}