using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            // pass our position so the knockback pushes the right way
            health.TakeDamage(transform.position);
        }
    }
}
