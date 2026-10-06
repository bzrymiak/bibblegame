using UnityEngine;

public class Powerup : MonoBehaviour
{
    private int totalBerries = 0;
    private int totalCoins = 0;
    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponentInParent<Health>();
        if (health == null) return;

        switch (gameObject.tag)
        {
            case "RedBerry":
                if (health.currentLives < health.startingLives)
                {
                    health.AddHealth();
                }
                Destroy(gameObject);
                totalBerries++;
                break;
            case "Coin":
                totalCoins++;
                break;
            default:
                break;
        }
    }
}