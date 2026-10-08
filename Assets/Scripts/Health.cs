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
    public float staggerDuration = 0.2f;
    public float invulnDuration = 1.2f;
    public float knockbackX = 3f;
    public float knockbackY = 1.5f;

    [Header("Flash")]
    public Color flashColour = new Color(1f, 0.80f, 0.82f);
    public float flashTime = 0.12f;
    public float blinkInterval = 0.08f;

    [Header("Blueberry invincibility")]
    public Color outlineColour = new Color(0.15f, 0.62f, 1f, 1f);
    public float outlineThickness = 0.16f;
    public Color bodyTint = new Color(0.40f, 0.78f, 1f, 1f);
    [Range(0f, 1f)] public float tintStrength = 0.7f;
    public float tintPulse = 0.18f;
    public float tintPulseSpeed = 6f;
    public float speedBoost = 2f;
    [Tooltip("Outline starts flashing this long before it wears off")]
    public float warnTime = 0.8f;
    public float warnFlashInterval = 0.09f;

    [Header("Juice")]
    public float hitStopDuration = 0.08f;
    public float shakeAmount = 0.03f;
    public float shakeDuration = 0.12f;

    private Player player;
    private SpriteRenderer sprite;
    private CameraFollow cam;
    private Color normalColour;
    private bool invulnerable;

    private bool invincible;
    private float invincibleTimer;
    private SpriteRenderer[] outline;

    public bool IsInvincible => invincible;

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
        // blueberry shrugs off obstacles and enemies, but not falling off the map
        if (invincible && !ignoreInvulnerability) return;
        if (invulnerable && !ignoreInvulnerability) return;

        // AudioManager.Instance.Play(AudioManager.SoundType.Damage);
        currentLives = Mathf.Clamp(currentLives - DAMAGE, 0, startingLives);
        UpdateHealthText();

        // push him away from whatever hit him, mostly sideways
        // small hop on the y so he doesn't just scrape along the ground
        float dir = transform.position.x < source.x ? -1f : 1f;
        if (player != null)
            player.Stagger(staggerDuration, new Vector2(knockbackX * dir, knockbackY));

        if (cam != null) cam.Shake(shakeAmount, shakeDuration);
        if (Swarm.Instance != null) Swarm.Instance.Surge();
        StartCoroutine(HitStop());
        StartCoroutine(FlashAndBlink());

        if (currentLives <= 0 && GameManager.Instance != null)
        {
            AudioManager.Instance.Play(AudioManager.SoundType.Dead);
            GameManager.Instance.LoseRun("The bees caught Bibble!");
        } else
        {
            AudioManager.Instance.Play(AudioManager.SoundType.Damage);
        }
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

    public void StartInvincibility(float duration)
    {
        // grabbing a second berry tops the timer up rather than stacking coroutines
        invincibleTimer = Mathf.Max(invincibleTimer, duration);
        if (!invincible) StartCoroutine(RunInvincibility());
    }

    private IEnumerator RunInvincibility()
    {
        invincible = true;
        if (player != null) player.speedMultiplier = speedBoost;

        if (outline == null) BuildOutline();
        SetOutlineVisible(true);

        float flashTimer = 0f;
        bool showing = true;

        while (invincibleTimer > 0f)
        {
            invincibleTimer -= Time.deltaTime;

            // flash near the end so you know it's about to run out
            if (invincibleTimer <= warnTime)
            {
                flashTimer -= Time.deltaTime;
                if (flashTimer <= 0f)
                {
                    showing = !showing;
                    flashTimer = warnFlashInterval;
                    SetOutlineVisible(showing);
                }
            }

            // re-apply every frame so a leftover hit-flash can't wipe it
            if (sprite != null)
            {
                if (showing)
                {
                    float t = Mathf.Clamp01(tintStrength + Mathf.Sin(Time.time * tintPulseSpeed) * tintPulse);
                    sprite.color = Color.Lerp(normalColour, bodyTint, t);
                }
                else
                {
                    sprite.color = normalColour;
                }
            }

            MatchOutlineToSprite();
            yield return null;
        }

        SetOutlineVisible(false);
        if (sprite != null) sprite.color = normalColour;
        if (player != null) player.speedMultiplier = 1f;
        invincible = false;
    }

    // eight copies of the sprite nudged outwards in a ring - where they stick out
    // past bibble you get a clean even outline, no blur, no scaling
    private static readonly Vector2[] OutlineDirections =
    {
        new Vector2( 1f,  0f), new Vector2(-1f,  0f),
        new Vector2( 0f,  1f), new Vector2( 0f, -1f),
        new Vector2( 0.7f,  0.7f), new Vector2(-0.7f,  0.7f),
        new Vector2( 0.7f, -0.7f), new Vector2(-0.7f, -0.7f),
    };

    private void BuildOutline()
    {
        Transform parent = sprite != null ? sprite.transform : transform;
        outline = new SpriteRenderer[OutlineDirections.Length];

        for (int i = 0; i < OutlineDirections.Length; i++)
        {
            var go = new GameObject("Outline" + i);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = OutlineDirections[i] * outlineThickness;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.color = outlineColour;

            if (sprite != null)
            {
                sr.sprite = sprite.sprite;
                sr.sortingLayerID = sprite.sortingLayerID;
                sr.sortingOrder = sprite.sortingOrder - 1;
            }

            outline[i] = sr;
            go.SetActive(false);
        }
    }

    private void MatchOutlineToSprite()
    {
        if (sprite == null || outline == null) return;

        for (int i = 0; i < outline.Length; i++)
        {
            outline[i].sprite = sprite.sprite;
            outline[i].flipX = sprite.flipX;
            outline[i].transform.localPosition = OutlineDirections[i] * outlineThickness;
        }
    }

    private void SetOutlineVisible(bool on)
    {
        if (outline == null) return;
        foreach (var sr in outline) sr.gameObject.SetActive(on);
    }

    public void AddHealth()
    {
        currentLives = Mathf.Clamp(currentLives + 1, 0, startingLives);
        UpdateHealthText();
    }
}
