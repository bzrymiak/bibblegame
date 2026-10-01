using UnityEngine;
using TMPro;

public class Health : MonoBehaviour
{
    const float DAMAGE = 1;
    private float startingLives = 3;
    private float currentLives;
    [SerializeField] private TMP_Text healthText;

    private void Awake()
    {
        currentLives = startingLives;
    }

    private void Start()
    {
        UpdateHealthText();
    }

    public void TakeDamage()
    {
        Debug.Log("hit");
        currentLives = Mathf.Clamp(currentLives - DAMAGE, 0, startingLives);
        UpdateHealthText();

        if (currentLives > 0)
        {
            // player is hurt
        }
        else
        {
            // player died
        }
    }

    private void UpdateHealthText()
    {
        healthText.text = "lives: " + currentLives;
    }
}