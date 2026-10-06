using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private GameObject instructions;
    public void NewGameButton()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void OpenInstructions()
    {
        instructions.SetActive(true);
    }

    public void CloseInstructions()
    {
        instructions.SetActive(false);
    }
}
