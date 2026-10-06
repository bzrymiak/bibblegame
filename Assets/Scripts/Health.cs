using System.Collections;
using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    const float DAMAGE = 1;
    public float startingLives = 3;
    public float currentLives;
    [SerializeField] private TMP_Text healthText;

    [Header("Getting hit")]
    public float staggerDuration = 0.3f;
    public float invulnDuration = 1.2f;
    public float knockbackX = 4.5f;
    public float knockbackY = 1.5f;

    [Header("Flash")]
    public Color flashColour = new Color(1f, 0.80f, 0.82f);
    public float flashTime = 0.12f;
    public float blinkInterval = 0.08f;

    [Header("Juice")]
    public float hitStopDuration = 0.08f;
    public float shakeAmount = 0.03f;
    public float shakeDuration = 0.12f;

    private Player player;
    private SpriteRenderer sprite;
    private CameraFollow cam;
    private Color normalColour;
    private bool invulnerable;

    private void Awake()
    {
        currentLives = startingLives;
        player = GetComponent<Player>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null) normalColour = sprite.color;
        if (Camera.main != null) cam = Camera.main.GetComponent<CameraFollow>();
    }

    private void Start()
    {
        UpdateHealthText();
    }

    // kept so anything already calling TakeDamage() with no args still works
    public void TakeDamage()
    {
        TakeDamage(transform.position);
    }

    public void TakeDamage(Vector2 source, bool ignoreInvulnerability = false)
    {
        if (currentLives <= 0) return;
        if (invulnerable && !ignoreInvulnerability) return;

        currentLives = Mathf.Clamp(currentLives - DAMAGE, 0, startingLives);
        UpdateHealthText();

        // push him away from whatever hit him, mostly sideways
        // small hop on the y so he doesn't just scrape along the ground
        float dir = transform.position.x < source.x ? -1f : 1f;
        if (player != null)
            player.Stagger(staggerDuration, new Vector2(knockbackX * dir, knockbackY));

        if (cam != null) cam.Shake(shakeAmount, shakeDuration);
        StartCoroutine(HitStop());
        StartCoroutine(FlashAndBlink());

        if (currentLives <= 0 && GameManager.Instance != null)
            GameManager.Instance.LoseRun("bibble ran out of lives");
    }

    // tiny freeze on impact
    private IEnumerator HitStop()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);

        // don't un-pause if the run ended during the freeze
        if (GameManager.Instance == null || GameManager.Instance.IsPlaying)
            Time.timeScale = 1f;
    }

    private IEnumerator FlashAndBlink()
    {
        invulnerable = true;

        if (sprite != null)
        {
            sprite.color = flashColour;
            yield return new WaitForSecondsRealtime(flashTime);
            sprite.color = normalColour;
        }

        float remaining = invulnDuration - flashTime;
        while (remaining > 0f)
        {
            if (sprite != null) sprite.enabled = !sprite.enabled;
            yield return new WaitForSecondsRealtime(blinkInterval);
            remaining -= blinkInterval;
        }

        if (sprite != null)
        {
            sprite.enabled = true;
            sprite.color = normalColour;
        }

        invulnerable = false;
    }

    private void UpdateHealthText()
    {
        // guard it - if the text isn't hooked up the whole hit reaction used to blow up here
        if (healthText == null) return;
        healthText.text = "lives: " + currentLives;
    }

    public void AddHealth()
    {
        currentLives = Mathf.Clamp(currentLives + 1, 0, startingLives);
        UpdateHealthText();
    }
}
