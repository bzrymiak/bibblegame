using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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

    [SerializeField] private GameObject swarm;
    private Image beeSwarm;
    [SerializeField] private float swarmSeconds = 1.3f;

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
        {
            AudioManager.Instance.Play(AudioManager.SoundType.Dead);
            LoseRun("The bees caught Bibble!");
        }
            
    }

    void Start()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopSound(AudioManager.SoundType.Bees);
        AudioManager.Instance?.ChangeMusic(AudioManager.SoundType.BackgroundMusic);
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
        // guard so it only fires once
        if (!IsPlaying) return;
        AudioManager.Instance?.StopMusic();

        State = RunState.Lost; 
        StartCoroutine(LoseSequence(blurb));
    }

    private IEnumerator LoseSequence(string blurb)
    {
        Time.timeScale = 0f;

        if (swarm != null)
        {
            if (Swarm.Instance != null) Swarm.Instance.SilenceSound();
            AudioManager.Instance.Play(AudioManager.SoundType.Bees);
            beeSwarm = swarm.GetComponent<Image>();
            beeSwarm.enabled = true;
            if (swarm.TryGetComponent(out Animator anim))
            {
                anim.updateMode = AnimatorUpdateMode.UnscaledTime; // makes sure it runs at timeScale 0
                anim.Play(0, 0, 0f);   // state name, layer 0, start at the beginning
            }
        }

        yield return new WaitForSecondsRealtime(swarmSeconds);

        if (endScreen != null) endScreen.ShowLoseScreen(blurb);
        // if (swarm != null) beeSwarm.enabled = false; 
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
