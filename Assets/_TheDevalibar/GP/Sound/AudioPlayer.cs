using System;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
   [SerializeField] private SoundType soundType = SoundType.SFX;
   [SerializeField] private AudioClip audioClipToPlay;

    public void PlaySound()
    {
        switch (soundType)
        {
            case SoundType.SFX:
            {
                AudioManager.Instance?.PlaySFX(audioClipToPlay);
                break;
            }
            case SoundType.Music:
            {
                AudioManager.Instance?.PlayMusic(audioClipToPlay);
                break;
            }
        }
        
    }

}

[Serializable]
public enum SoundType
{
    Music,
    SFX
}
