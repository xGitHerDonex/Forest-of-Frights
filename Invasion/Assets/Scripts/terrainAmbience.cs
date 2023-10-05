using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class terrainAmbience : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    private AudioSource audioSource;


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(playAudioInSequence());

    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying)
        {
            StopCoroutine(playAudioInSequence());
            StartCoroutine(playAudioInSequence());
        }
    }
    IEnumerator playAudioInSequence()
    {
        audioSource.volume = .1f;
        int rand = Random.Range(0, audioClips.Length);
        rand = (rand + 1) % audioClips.Length;
        audioSource.clip = audioClips[rand];

        audioSource.Play();

        while (audioSource.isPlaying)
        {
            if (SceneLoadingStatus.InProgress == 0)
            {
                yield return new WaitForSeconds(1);
                
            }


                yield return null;
            
        }


    }
}
