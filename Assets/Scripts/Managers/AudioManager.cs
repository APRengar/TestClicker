using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;

    public AudioClip clickSound;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (_audioSource && clickSound)
        {
            _audioSource.PlayOneShot(clickSound);
        }
    }
}
