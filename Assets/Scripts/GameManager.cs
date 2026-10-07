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

    [Header("Difficulty ramp")]
    public CameraFollow cameraFollow;
    public Player playerScript;
    [Tooltip("Seconds between each speed bump")]
    public float rampInterval = 20f;
    [Tooltip("How much the scroll speed goes up each time")]
    public float scrollSpeedStep = 5f;
    public float maxScrollSpeed = 40f;

    [Header("Caught by the swarm")]
    [Tooltip("How far past the left edge of the screen before the bees get him")]
    public float caughtMargin = 1f;

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;

        if (cam == null) cam = Camera.main;
        if (cameraFollow == null && cam != null) cameraFollow = cam.GetComponent<CameraFollow>();
        if (cameraFollow != null) baseScrollSpeed = cameraFollow.scrollSpeed;

        // fall back to finding him, the slot being empty is an easy one to miss
        if (playerScript == null && player != null) playerScript = player.GetComponent<Player>();
        if (playerScript == null) playerScript = FindFirstObjectByType<Player>();
        // if (endScreen != null) endScreen.SetActive(false);
    }

    private float baseScrollSpeed;
    private float rampTimer;

    private void Update()
    {
        if (!IsPlaying || player == null || cam == null) return;

        RampDifficulty();

        // left edge of what the camera can see, in world units
        float leftEdge = cam.transform.position.x - cam.orthographicSize * cam.aspect;

        if (player.position.x < leftEdge - caughtMargin)
            LoseRun("The bees caught Bibble!");
    }

    private void RampDifficulty()
    {
        if (cameraFollow == null) return;

        rampTimer += Time.deltaTime;
        if (rampTimer < rampInterval) return;

        rampTimer -= rampInterval;
        cameraFollow.scrollSpeed = Mathf.Min(cameraFollow.scrollSpeed + scrollSpeedStep, maxScrollSpeed);

        // keep bibble in the same relationship to the swarm as it speeds up,
        // otherwise the ramp just becomes unwinnable rather than harder
        if (playerScript != null && baseScrollSpeed > 0f)
            playerScript.difficultyMultiplier = cameraFollow.scrollSpeed / baseScrollSpeed;
    }

    public void LoseRun(string blurb)
    {
        // guard so it only fires once - Update was calling this every frame
        if (!IsPlaying) return;

        State = RunState.Lost;
        if (endScreen != null) endScreen.ShowLoseScreen(blurb);
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
