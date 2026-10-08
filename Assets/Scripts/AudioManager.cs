using UnityEngine;
using System.Collections.Generic;
 
public class AudioManager : MonoBehaviour
{
    public enum SoundType
    {
        Bees,
        Hit,
        BackgroundMusic,
        Damage,
        Berry,
        Coin,
        Dead,
        Victory,
        Button
    }
 
    [System.Serializable]
    public class Sound
    {
        public SoundType Type;
        public AudioClip Clip;
 
        [Range(0f, 1f)]
        public float Volume = 1f;
 
        [HideInInspector]
        public AudioSource Source;
    }
 
    //Singleton
    public static AudioManager Instance;
 
    //All sounds and their associated type - Set these in the inspector
    public Sound[] AllSounds;
 
    //Runtime collections
    private Dictionary<SoundType, Sound> _soundDictionary = new Dictionary<SoundType, Sound>();
    private AudioSource _musicSource;
 
    private void Awake()
{
    // if the manager exists in several scenes, keep only the first one
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);

    foreach (var s in AllSounds)
        _soundDictionary[s.Type] = s;
}

    public void Play(SoundType type)
    {
        if (!_soundDictionary.TryGetValue(type, out Sound s))
        {
            Debug.LogWarning($"Sound type {type} not found!");
            return;
        }

        var soundObj = new GameObject($"Sound_{type}");
        DontDestroyOnLoad(soundObj); // survives scene loads, so button clicks finish playing

        var audioSrc = soundObj.AddComponent<AudioSource>();
        audioSrc.clip = s.Clip;
        audioSrc.volume = s.Volume;
        audioSrc.Play();

        // Destroy(obj, t) uses scaled time and would never fire while timeScale is 0
        StartCoroutine(DestroyAfter(soundObj, s.Clip.length));
    }

    private System.Collections.IEnumerator DestroyAfter(GameObject obj, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (obj != null) Destroy(obj);
    }

    public void ChangeMusic(SoundType type)
    {
        if (!_soundDictionary.TryGetValue(type, out Sound track))
        {
            Debug.LogWarning($"Music track {type} not found!");
            return;
        }

        if (_musicSource == null)
        {
            var container = new GameObject("SoundTrackObj");
            DontDestroyOnLoad(container); // music keeps playing across scenes
            _musicSource = container.AddComponent<AudioSource>();
            _musicSource.loop = true;
        }

        // don't restart the track if it's already playing (e.g. a scene calls this in Start)
        if (_musicSource.clip == track.Clip && _musicSource.isPlaying) return;

        _musicSource.clip = track.Clip;
        _musicSource.volume = track.Volume;
        _musicSource.Play();
    }
}