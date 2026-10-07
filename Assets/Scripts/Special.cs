using UnityEngine;

public class Special : MonoBehaviour
{
    public EndScreen endScreen;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponentInParent<Health>();
        if (health == null) return;

        switch (gameObject.tag)
        {
            case "RedBerry":
                if (health.currentLives < health.startingLives)
                    health.AddHealth();
                GameScore.AddBerry();
                Destroy(gameObject);
                break;

            case "Coin":
                GameScore.AddCoin();
                Destroy(gameObject);
                break;

            case "Peony":
                Time.timeScale = 0;
                endScreen.ShowWinScreen(GameScore.Total);
                break;
            default:
                break;
        }
    }
}