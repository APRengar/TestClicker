using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip clickSound;

    private AudioSource _audioSource;

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
