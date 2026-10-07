using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    [Header("EndScreen objects")]
    [SerializeField] private GameObject endScreen;  
    [SerializeField] private Image endBg;
    [SerializeField] private Image endTitle;
    [SerializeField] private TMP_Text scoreText;

    [Header("Win sprites")]
    [SerializeField] private Sprite winBgSprite;
    [SerializeField] private Sprite winTitleSprite;

    [Header("Lose sprites")]
    [SerializeField] private Sprite loseBgSprite;
    [SerializeField] private Sprite loseTitleSprite;

    private void Start()
    {
        // endScreen.SetActive(false);
    }

    public void ShowWinScreen(int totalScore)
    {
        endBg.sprite = winBgSprite;
        endTitle.sprite = winTitleSprite;
        scoreText.text = "SCORE: " + totalScore;
        scoreText.gameObject.SetActive(true);
        endScreen.SetActive(true);
    }

    public void ShowLoseScreen(string blurb)
    {
        Time.timeScale = 0;
        endBg.sprite = loseBgSprite;
        endTitle.sprite = loseTitleSprite;
        scoreText.text = blurb;
        endScreen.SetActive(true);
    }
}