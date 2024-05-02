using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] float pickupVolumeScale = 1;
    [SerializeField] AudioClip[] pickupAudio;

    [SerializeField] float throwVolumeScale = 1;
    [SerializeField] AudioClip[] throwAudio;

    [SerializeField] float popVolumeScale = 1;
    [SerializeField] AudioClip[] popAudio;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void StartPickupAudio()
    {
        audioSource.PlayOneShot(pickupAudio[Random.Range(0, pickupAudio.Length)], pickupVolumeScale);
    }

    public void StartThrowAudio()
    {
        audioSource.PlayOneShot(throwAudio[Random.Range(0, throwAudio.Length)], throwVolumeScale);
    }

    public void StartPopAudio()
    {
        audioSource.PlayOneShot(popAudio[Random.Range(0, popAudio.Length)], popVolumeScale);
    }
}
