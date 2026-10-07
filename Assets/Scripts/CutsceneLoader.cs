using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneLoader : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game";
    [SerializeField] private float delaySeconds = 3f;

    private IEnumerator Start()
    {
        Time.timeScale = 1f;
        yield return new WaitForSecondsRealtime(delaySeconds);
        SceneManager.LoadScene(gameScene);
    }
}