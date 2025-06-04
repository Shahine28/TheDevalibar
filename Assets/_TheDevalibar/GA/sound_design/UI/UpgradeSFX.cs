using UnityEngine;

public class UpgradeSFX: MonoBehaviour
{
    [Header("音频设置")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

    [Header("播放选项")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool loop = false;
    [SerializeField] private float volume = 1f;

    void Start()
    {
        // 如果没有指定AudioSource，自动获取或创建一个
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        // 设置AudioSource属性
        audioSource.clip = audioClip;
        audioSource.loop = loop;
        audioSource.volume = volume;

        // 如果设置了开始时播放
        if (playOnStart && audioClip != null)
        {
            PlayClip();
        }
    }

    /// <summary>
    /// 播放音频片段
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
    /// 停止播放音频
    /// </summary>
    public void StopClip()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// 暂停播放音频
    /// </summary>
    public void PauseClip()
    {
        if (audioSource != null)
        {
            audioSource.Pause();
        }
    }

    /// <summary>
    /// 恢复播放音频
    /// </summary>
    public void ResumeClip()
    {
        if (audioSource != null)
        {
            audioSource.UnPause();
        }
    }

    /// <summary>
    /// 设置音量
    /// </summary>
    /// <param name="newVolume">音量值 (0-1)</param>
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    /// <summary>
    /// 检查音频是否正在播放
    /// </summary>
    /// <returns>是否正在播放</returns>
    public bool IsPlaying()
    {
        return audioSource != null && audioSource.isPlaying;
    }

    /// <summary>
    /// 设置新的音频片段
    /// </summary>
    /// <param name="newClip">新的音频片段</param>
    public void SetAudioClip(AudioClip newClip)
    {
        audioClip = newClip;
        if (audioSource != null)
        {
            audioSource.clip = audioClip;
        }
    }
}