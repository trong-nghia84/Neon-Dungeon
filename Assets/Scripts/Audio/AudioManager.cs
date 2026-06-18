using UnityEngine;
using System;

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music (BGM)")]
    public AudioClip menuBGM;
    public AudioClip gameBGM;

    [Header("Sound Effects (SFX)")]
    public Sound[] sfxSounds;

    // Biến lưu trữ trạng thái hiện tại (Đang tắt âm hay bật âm)
    public bool isMuted { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Tải lại trạng thái Bật/Tắt âm thanh đã lưu
        LoadMuteSettings();
    }

    private void Start()
    {
        PlayMenuMusic();
    }

    // --- CÁC HÀM PHÁT NHẠC (BGM) ---
    public void PlayMenuMusic()
    {
        if (musicSource.clip == menuBGM) return;
        musicSource.clip = menuBGM;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        if (musicSource.clip == gameBGM) return;
        musicSource.clip = gameBGM;
        musicSource.Play();
    }

    // --- CÁC HÀM PHÁT HIỆU ỨNG (SFX) ---
    public void PlaySFX(string soundName)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == soundName);
        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy âm thanh SFX: " + soundName);
        }
    }

    // --- HÀM XỬ LÝ NÚT BẤM BẬT/TẮT ÂM THANH ---
    public void ToggleAudio()
    {
        // Đảo ngược trạng thái hiện tại
        isMuted = !isMuted;

        // Cập nhật trạng thái cho AudioSource
        ApplyMuteState();

        // Lưu trạng thái vào PlayerPrefs (1 là Mute, 0 là Unmute)
        PlayerPrefs.SetInt("IsMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadMuteSettings()
    {
        // Lấy dữ liệu đã lưu, mặc định là 0 (nghĩa là đang bật âm thanh)
        isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1;
        ApplyMuteState();
    }

    private void ApplyMuteState()
    {
        // Sử dụng thuộc tính .mute của AudioSource để tắt tiếng nhưng không làm mất tiến trình nhạc đang chạy
        musicSource.mute = isMuted;
        sfxSource.mute = isMuted;
    }
}