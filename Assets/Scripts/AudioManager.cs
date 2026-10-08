using UnityEngine;
using System.Collections.Generic;
 
public class AudioManager : MonoBehaviour
{
    public enum SoundType
    {
        Bees,
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
        public float Volume;
 
        [HideInInspector]
        public AudioSource Source;
    }
 
    //Singleton
    public static AudioManager Instance;
 
    //All sounds and their associated type - Set these in the inspector
    public Sound[] AllSounds;
 
    //Runtime collections
    private Dictionary<SoundType, Sound> _soundDictionary = new Dictionary<SoundType, Sound>();
    private Dictionary<SoundType, AudioSource> _loops = new Dictionary<SoundType, AudioSource>();
    private Dictionary<SoundType, List<GameObject>> _oneShots = new Dictionary<SoundType, List<GameObject>>();
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
        if (!_oneShots.TryGetValue(type, out var list))
            _oneShots[type] = list = new List<GameObject>();
        list.Add(soundObj);

        StartCoroutine(DestroyAfter(soundObj, type, s.Clip.length));
    }

    private System.Collections.IEnumerator DestroyAfter(GameObject obj, SoundType type, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (_oneShots.TryGetValue(type, out var list)) list.Remove(obj);
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

    // call every frame
    public void SetLoop(SoundType type, float amount)
    {
        if (!_soundDictionary.TryGetValue(type, out Sound s))
        {
            Debug.LogWarning($"Loop sound {type} not found!");
            return;
        }

        _loops.TryGetValue(type, out AudioSource src);

        if (amount <= 0.01f)
        {
            if (src != null && src.isPlaying) src.Stop();
            return;
        }

        if (src == null)
        {
            // child of the manager, so it's persistent and shares its lifetime
            var obj = new GameObject($"Loop_{type}");
            obj.transform.SetParent(transform);
            src = obj.AddComponent<AudioSource>();
            src.clip = s.Clip;
            src.loop = true;
            _loops[type] = src;
        }

        src.volume = s.Volume * Mathf.Clamp01(amount);
        if (!src.isPlaying) src.Play();
    }

    public void StopLoop(SoundType type)
    {
        if (_loops.TryGetValue(type, out AudioSource src))
        {
            if (src != null) Destroy(src.gameObject);
            _loops.Remove(type);
        }
    }

    public void StopSound(SoundType type)
    {
        StopLoop(type);

        if (_oneShots.TryGetValue(type, out var list))
        {
            foreach (var obj in list)
                if (obj != null) Destroy(obj);
            list.Clear();
        }
    }

    public void StopMusic()
    {
        if (_musicSource != null) _musicSource.Stop();
    }
}