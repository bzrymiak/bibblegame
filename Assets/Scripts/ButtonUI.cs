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
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        Time.timeScale = 1f;
        SceneManager.LoadScene(introScene);
    }

    public void ReplayButton()
    {
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameScene);
    }

    public void MenuButton()
    {
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        SceneManager.LoadScene(menuScene);
    }

    public void OpenInstructions()
    {
        i = 0;
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        instructions.SetActive(true);
        ShowPage();
    }

    public void CloseInstructions()
    {
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        instructions.SetActive(false);
    }

    public void NextMenu()
    {
        if (i < menus.Length - 1) i++;
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
        ShowPage();
    }

    public void PrevMenu()
    {
        if (i > 0) i--;
        AudioManager.Instance.Play(AudioManager.SoundType.Button);
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
