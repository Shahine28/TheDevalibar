using UnityEngine;

public class UpgradeSFX: MonoBehaviour
{
    [Header("Sound Setting")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

    [Header("Play Setting")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool loop = false;
    [SerializeField] private float volume = 1f;

    void Start()
    {
        // If AudioSource absent，Create one
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // AudioSource Setting
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;

        // If PlayOnStart
        if (playOnStart && audioClip != null)
        {
            PlayClip();
        }
    }

    /// <summary>
    /// Play AudioClip
    /// </summary>
    public void PlayClip()
    {
        if (audioClip != null && audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip 或 AudioSource 为空，无法播放音频！");
        }
    }

    /// <summary>
    /// Stop Playing Clip
    /// </summary>
    public void StopClip()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Pause Playing Clip
    /// </summary>
    public void PauseClip()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// Resume Playing Clip
    /// </summary>
    public void ResumeClip()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    /// <summary>
    /// Volume Setting
    /// </summary>
    /// <param name="newVolume">Volume (0-1)</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// If Audio Playing
    /// </summary>
    /// <returns>If Audio Playing</returns>
    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    /// <summary>
    /// New AudioClip Setting
    /// </summary>
    /// <param name="newClip">New AudioClip</param>
    public void SetAudioClip(AudioClip newClip)
    {
        audioClip = newClip;
        if (audioSource != null)
        {
            audioSource.clip = audioClip;
        }
    }
}