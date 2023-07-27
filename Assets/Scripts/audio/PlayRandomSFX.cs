using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayRandomSFX : MonoBehaviour
{
    [SerializeField] float volume;
    [SerializeField] float2 volumeVariance;
    [SerializeField] AudioClip[] basket;
    // Start is called before the first frame update
    void Start()
    {
        transform.SetParent(null);  // Detach this from the destroyed spark GameObject

        AudioSource source = GetComponent<AudioSource>();
        int i = UnityEngine.Random.Range(0, basket.Length);
        AudioClip clip = basket[i];
        float length = clip.length;

        float variance = UnityEngine.Random.Range(volumeVariance.x, volumeVariance.y);
        volume = math.clamp(volume + variance, 0, 1);
        source.PlayOneShot(clip, volume);

        Destroy(gameObject, length);
    }

}
