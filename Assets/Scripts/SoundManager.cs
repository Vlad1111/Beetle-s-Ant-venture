using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * music credits:
 */

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SfxClips
    {
        public string name;
        public AudioClip[] clips;
        public AudioSource source;
        public Vector2 volume;
        public Vector2 pitch;
        public float playAfterLastTimeSlice = 0;
        public bool isItLoop = false;
        public float startAfterSeconds = 0;

        private void RecalculateValues(float volumeMultiplyer = 1, float pitchMultiplyer = 1)
        {
            var vol = volumeMultiplyer * (volume.x + Random.value * (volume.y - volume.x));
            var pit = pitchMultiplyer * (pitch.x + Random.value * (pitch.y - pitch.x));
            source.volume = vol;// * SettingMenuUI.setting.masterVolume * SettingMenuUI.setting.sfxVolume;
            source.pitch = pit;
            source.clip = clips[Random.Range(0, clips.Length)];// Extensions.PickRandom(clips);
        }

        public void Play(float volumeMultiplyer = 1, float pitchMultiplyer = 1)
        {
            if (playAfterLastTimeSlice >= 1 &&
                source.clip != null &&
                source.isPlaying &&
                source.time < source.clip.length / playAfterLastTimeSlice)
                return;
            RecalculateValues();
            source.volume *= volumeMultiplyer;
            source.pitch *= pitchMultiplyer;
            source.loop = isItLoop;
            if (startAfterSeconds == 0)
                source.Play();
            else if (startAfterSeconds > 0)
                source.PlayDelayed(startAfterSeconds);
            else source.PlayDelayed(-startAfterSeconds * Random.value);
        }

        public void Initialize(GameObject gameObject)
        {
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
        }

        internal void Stop()
        {
            source.Stop();
        }
    }

    public AudioClip startingBackgroundMusic;
    public SfxClips[] clips;

    private float fadingValue = -1;
    private AudioSource backgroundMusicSource1;
    private AudioSource backgroundMusicSource2;

    private Dictionary<string, float> backgroundMusicRetake = new Dictionary<string, float>();

    public void Awake()
    {
        if (Instance != null)
        {
            Instance.PlayMusic(startingBackgroundMusic);
            Instance.startingBackgroundMusic = startingBackgroundMusic;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {
        backgroundMusicSource1 = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource2 = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource1.loop = true;
        backgroundMusicSource2.loop = true;
        RecalculateMusicVolume();
        if (startingBackgroundMusic)
            PlayStartingBackgroundMusic();

        foreach(var clip in clips)
        {
            clip.Initialize(gameObject);
        }
    }

    public void Update()
    {
        if(fadingValue >= 0)
        {
            fadingValue -= 2*Time.deltaTime;
            if (fadingValue < 0)
            {
                backgroundMusicSource1.Pause();
                if (backgroundMusicSource1.clip != null)
                    backgroundMusicRetake[backgroundMusicSource1.clip.name] = backgroundMusicSource1.time;
                var aux = backgroundMusicSource2;
                backgroundMusicSource2 = backgroundMusicSource1;
                backgroundMusicSource1 = aux;
            }
            else
            {
                backgroundMusicSource1.volume = fadingValue;// *
                                                            //SettingMenuUI.setting.masterVolume * SettingMenuUI.setting.musicVolume;
                backgroundMusicSource2.volume = (1 - fadingValue);// *
                                                                  //SettingMenuUI.setting.masterVolume * SettingMenuUI.setting.musicVolume;
            }
        }
    }

    public void RecalculateMusicVolume()
    {
        if (fadingValue > 0)
            return;
        //if (SettingMenuUI.setting == null)
        //    return;
        //backgroundMusicSource1.volume = SettingMenuUI.setting.masterVolume * SettingMenuUI.setting.musicVolume;
        //backgroundMusicSource2.volume = SettingMenuUI.setting.masterVolume * SettingMenuUI.setting.musicVolume;
    }

    private void PlayMusic(AudioClip music)
    {
        if (backgroundMusicSource1.clip != null && backgroundMusicSource1.clip.name == music.name)
            return;
        if (backgroundMusicSource2.clip == null || backgroundMusicSource2.clip.name != music.name)
        { 
            backgroundMusicSource2.clip = music;
            if(backgroundMusicRetake.ContainsKey(music.name))
                backgroundMusicSource2.time = backgroundMusicRetake[music.name];
        }
        backgroundMusicSource2.Play();
        fadingValue = 1;
    }
    public void PlayMusic(string musicName)
    {
        if(musicName == null)
        {
            PlayStartingBackgroundMusic();
            return;
        }
        //musicName = GENERAL.FileConstants.musicLocation + "/" + musicName;
        //var clip = GENERAL.loadAudioClip(musicName, GENERAL.ModFileType.Replace);
        //if (clip != null)
        //    PlayMusic(clip);
    }

    public void PlayStartingBackgroundMusic()
    {
        PlayMusic(startingBackgroundMusic);
    }

    public static void PlaySfxClip(string clipName)
    {
        if (SoundManager.Instance != null)
            Instance.PlayClip(clipName);
    }

    public static bool PlayClipFrom(string clipName, SfxClips[] clips)
    {
        foreach (var clip in clips)
            if (clip.name == clipName)
            {
                clip.Play();
                return true;
            }
        return false;
    }

    public static void StopClipFrom(string clipName, SfxClips[] clips)
    {
        foreach (var clip in clips)
            if (clip.name == clipName)
            {
                clip.Stop();
                break;
            }
    }
    public void PlayClip(string clipName)
    {
        if(!PlayClipFrom(clipName, clips))
        {
            //var file = GENERAL.FileConstants.sfxLocation + "/" + clipName;
            //var newClipSound = GENERAL.loadAudioClip(file, GENERAL.ModFileType.Replace);
            //if(newClipSound != null)
            //{
            //    var newClip = new SfxClips()
            //    {
            //        name = clipName,
            //        clips = new[] { newClipSound },
            //        isItLoop = false,
            //        pitch = Vector2.one,
            //        volume = Vector2.one,
            //        playAfterLastTimeSlice = 0,
            //        startAfterSeconds = 0
            //    };
            //    newClip.Initialize(gameObject);
            //    var newClips = new List<SfxClips>(clips);
            //    newClips.Add(newClip);
            //    clips = newClips.ToArray();
            //    newClip.Play();
            //}
        }
    }

    public void StopClip(string clipName)
    {
        StopClipFrom(clipName, clips);
    }
}
