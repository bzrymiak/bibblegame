using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum RunState { Playing, Lost, Won }
    public RunState State { get; private set; } = RunState.Playing;
    public bool IsPlaying => State == RunState.Playing;

    [Header("Scene refs")]
    public Transform player;
    public Camera cam;

    [Header("End screen")]
    public GameObject endScreen;
    public TMP_Text endScreenText;

    [Header("Caught by the swarm")]
    [Tooltip("How far past the left edge of the screen before the bees get him")]
    public float caughtMargin = 1f;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (cam == null) cam = Camera.main;
        if (endScreen != null) endScreen.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlaying || player == null || cam == null) return;

        // left edge of what the camera can see, in world units
        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        if (player.position.x < leftEdge - caughtMargin)
            LoseRun("the bees caught you");
    }

    public void LoseRun(string reason)
    {
        if (!IsPlaying) return;

        State = RunState.Lost;
        ShowEndScreen(reason);
        Time.timeScale = 0f;
    }

    public void WinRun()
    {
        if (!IsPlaying) return;

        State = RunState.Won;
        ShowEndScreen("you made it home");
        Time.timeScale = 0f;
    }

    private void ShowEndScreen(string message)
    {
        if (endScreenText != null) endScreenText.text = message;
        if (endScreen != null) endScreen.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
