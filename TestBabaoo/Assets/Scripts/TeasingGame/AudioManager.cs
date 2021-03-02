using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager _instance;

    private AudioSource audioSource;

    private void Awake()
    {
        if(_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(Sound sound)
    {
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        audioSource.Play();
    }
}
