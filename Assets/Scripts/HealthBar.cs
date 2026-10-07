using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField]private Image totalHealth;
    [SerializeField]private Image currentHealth;

    void Start()
    {
        currentHealth.fillAmount = playerHealth.currentLives / 10;
    }

    void Update()
    {
        currentHealth.fillAmount = playerHealth.currentLives / 10;
    }
}
