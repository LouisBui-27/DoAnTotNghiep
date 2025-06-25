using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("Audio Sources")]
    public AudioSource _musicSource; // AudioSource cho nhạc nền
    public AudioSource _sfxSource;   // AudioSource cho hiệu ứng âm thanh
  
    [Header("Button Click Sound")]
    public AudioClip buttonClickClip;

    [Header("Default Audio Clips (Optional - assign in Inspector)")]
    public AudioClip defaultMusicClip; // Nhạc nền mặc định (ví dụ: nhạc menu)

    [Header("SFX Clips")]
    public AudioClip collectEXP;
    public AudioClip playerShootClip;
    public AudioClip playerArrowClip;
    public AudioClip boomerang;
    public AudioClip heal;
    public AudioClip die;
    public AudioClip takeDam;
    public AudioClip OrbitingObject;
    public AudioClip rocket;
    public AudioClip bom;
    public AudioClip coin;
    public AudioClip tornado;
    public AudioClip lighting;

    private float _musicVolume = 1f; // Biến lưu trữ âm lượng nhạc nền
    private float _sfxVolume = 1f;   // Biến lưu trữ âm lượng SFX

    // Hằng số cho khóa lưu trữ trong PlayerPrefs
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SfxVolume";

    private const string MUSIC_MUTE_KEY = "MusicMuted";
    private const string SFX_MUTE_KEY = "SFXMuted";
    private int expSoundCount = 0;
    private float expSoundTimer = 0f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSettings();


        }
        else
        {
            Destroy(gameObject); 
        }
    }
    //private void Update()
    //{
    //    expSoundTimer += Time.deltaTime;
    //    if (expSoundTimer >= 1f)
    //    {
    //        Debug.Log("EXP sound count this second: " + expSoundCount);
    //        expSoundCount = 0;
    //        expSoundTimer = 0f;
    //    }
    //}

    private void InitializeAudioSettings()
    {
        // Tải âm lượng đã lưu từ PlayerPrefs, nếu không có thì đặt mặc định là 1
        _musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
        _sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 1f);

        // Áp dụng âm lượng cho các AudioSource
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
            _musicSource.mute = PlayerPrefs.GetInt(MUSIC_MUTE_KEY, 0) == 1;
        }
        if (_sfxSource != null)
        {
            _sfxSource.volume = _sfxVolume;
            _sfxSource.mute = PlayerPrefs.GetInt(SFX_MUTE_KEY, 0) == 1;
        }

        // Phát nhạc nền mặc định nếu có và chưa phát
        if (defaultMusicClip != null && _musicSource != null && !_musicSource.isPlaying)
        {
            PlayMusic(defaultMusicClip);
        }
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (_musicSource != null && clip != null)
        {
            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioManager: MusicSource or MusicClip is null. Cannot play music.");
        }
    }
    public void StopMusic()
    {
        if (_musicSource != null && _musicSource.isPlaying)
        {
            _musicSource.Stop();
        }
    }
    public void PlaySFX(AudioClip clip)
    {
        if (_sfxSource != null && clip != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioManager: SFXSource or SFXClip is null. Cannot play SFX.");
        }
    }
    public void SetMusicVolume(float volume)
    {
        _musicVolume = Mathf.Clamp01(volume);
        if (_musicSource != null)
        {
            _musicSource.volume = _musicVolume;
        }
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, _musicVolume); // Lưu vào PlayerPrefs
    }
    public void SetSFXVolume(float volume)
    {
        _sfxVolume = Mathf.Clamp01(volume);
        if (_sfxSource != null)
        {
            _sfxSource.volume = _sfxVolume;
        }
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, _sfxVolume); // Lưu vào PlayerPrefs
    }
    public float GetMusicVolume()
    {
        return _musicVolume;
    }
    public float GetSFXVolume()
    {
        return _sfxVolume;
    }
    public void ToggleMusic(bool isMuted)
    {
        if (_musicSource != null)
        {
            _musicSource.mute = isMuted;
            // Nếu bạn muốn khi tắt thì giảm volume về 0, bật thì dùng lại volume cũ:
            _musicSource.volume = isMuted ? 0f : _musicVolume;
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, isMuted ? 1 : 0);
        }
    }

    public void ToggleSFX(bool isMuted)
    {
        if (_sfxSource != null)
        {
            _sfxSource.mute = isMuted;
            _sfxSource.volume = isMuted ? 0f : _sfxVolume;
            PlayerPrefs.SetInt(SFX_MUTE_KEY, isMuted ? 1 : 0);
        }
    }
    public void PlayPlayerShoot() => PlaySFX(playerShootClip);
    public void PlayPlayerArrow() => PlaySFX(playerArrowClip);
    public void PlayPlayerBoomerang() => PlaySFX(boomerang);
    public void PlayPlayerEXP()
    {
        //expSoundCount++;
        PlaySFX(collectEXP);
    }
    public void PlayPlayerHeal() => PlaySFX(heal);
    public void PlayPlayerDie() => PlaySFX(die);
    public void PlayPlayerTakeDam() => PlaySFX(takeDam);
    public void PlayPlayerOrbitingObject() => PlaySFX(OrbitingObject);
    public void PlayPlayerRocket() => PlaySFX(rocket);
    public void PlayPlayerBom() => PlaySFX(bom);
    public void PlayPlayerCoin() => PlaySFX(coin);
    public void PlayPlayerTornado() => PlaySFX(tornado);
    public void PlayPlayerLighting() => PlaySFX(lighting);
    

}
