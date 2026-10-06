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
                // only eat it if it actually does something, otherwise leave it
                // on the map so you can come back for it after taking a hit
                if (health.currentLives >= health.startingLives) return;

                health.AddHealth();
                totalBerries++;
                Destroy(gameObject);
                break;
            case "Coin":
                totalCoins++;
                break;
            default:
                break;
        }
    }
}