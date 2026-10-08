using UnityEngine;

public class Special : MonoBehaviour
{
    public EndScreen endScreen;
    public float invincibleSeconds = 3f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var health = other.GetComponentInParent<Health>();
        if (health == null) return;

        switch (gameObject.tag)
        {
            case "RedBerry":
                AudioManager.Instance.Play(AudioManager.SoundType.Berry);
                if (health.currentLives < health.startingLives)
                    health.AddHealth();
                GameScore.AddBerry();
                Destroy(gameObject);
                break;

            case "Coin":
                AudioManager.Instance.Play(AudioManager.SoundType.Coin);
                GameScore.AddCoin();
                Destroy(gameObject);
                break;

            case "BlueBerry":
                AudioManager.Instance.Play(AudioManager.SoundType.Berry);
                health.StartInvincibility(invincibleSeconds);
                GameScore.AddBerry();
                Destroy(gameObject);
                break;

            case "Peony":
                if (Swarm.Instance != null) Swarm.Instance.SilenceSound();
                AudioManager.Instance?.StopMusic();
                AudioManager.Instance.Play(AudioManager.SoundType.Victory);
                Time.timeScale = 0;
                endScreen.ShowWinScreen(GameScore.Total);
                break;
            default:
                break;
        }
    }
}