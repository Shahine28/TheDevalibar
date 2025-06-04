using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance;
    public AudioClip clickClip;
    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        } else {
            Destroy(gameObject);
        }
    }

    public void PlayClick()
    {
        if (clickClip != null)
            audioSource.PlayOneShot(clickClip);
    }
}
