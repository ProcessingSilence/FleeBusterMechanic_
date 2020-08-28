using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Have constant sound effects overlap each other by spawning audio objects.
// The objects delete themselves after they're done playing.
public class SoundEffectObj : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying == false)
        {
            Destroy(gameObject);
        }
    }
}
