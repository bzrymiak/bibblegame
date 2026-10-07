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
    public EndScreen endScreen;
    // public TMP_Text endScreenText;

    [Header("Caught by the swarm")]
    [Tooltip("How far past the left edge of the screen before the bees get him")]
    public float caughtMargin = 1f;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (cam == null) cam = Camera.main;
        // if (endScreen != null) endScreen.SetActive(false);
    }

    private void Update()
    {
        if (!IsPlaying || player == null || cam == null) return;

        // left edge of what the camera can see, in world units
        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        if (player.position.x < leftEdge - caughtMargin)
            // LoseRun("the bees caught you");
            endScreen.ShowLoseScreen("The bees caught Bibble!");
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
