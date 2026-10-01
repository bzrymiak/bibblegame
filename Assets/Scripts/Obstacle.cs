using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("hit " + collision.gameObject.name);
        if (collision.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage();
        }
    }
}