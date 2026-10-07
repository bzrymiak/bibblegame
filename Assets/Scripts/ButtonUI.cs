using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private string introScene = "Intro";
    [SerializeField] private string menuScene = "Menu";
    [SerializeField] private GameObject instructions;
    [SerializeField] private GameObject menuDisplay;
    [SerializeField] private Sprite[] menus;

    [Header("Optional: hidden at the first/last page")]
    [SerializeField] private GameObject prevButton;
    [SerializeField] private GameObject nextButton;

    private Image image;
    private int i;

    void Awake()
    {
        image = menuDisplay.GetComponent<Image>();
    }

    public void NewGameButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(introScene);
    }

    public void ReplayButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void MenuButton()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void OpenInstructions()
    {
        i = 0;
        instructions.SetActive(true);
        ShowPage();
    }

    public void CloseInstructions()
    {
        instructions.SetActive(false);
    }

    public void NextMenu()
    {
        if (i < menus.Length - 1) i++;
        ShowPage();
    }

    public void PrevMenu()
    {
        if (i > 0) i--;
        ShowPage();
    }

   private void ShowPage()
    {
        if (menus == null || menus.Length == 0) return;

        if (image != null) image.sprite = menus[i];

        if (prevButton != null) prevButton.SetActive(i > 0);
        if (nextButton != null) nextButton.SetActive(i < menus.Length - 1);
    }
}
