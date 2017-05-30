using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(AudioSource))]

public class PrintVideo : MonoBehaviour
{
    public MovieTexture m_movieTexture;
    private AudioSource m_audioSource;

    void Start()
    {
        GetComponent<RawImage>().texture = m_movieTexture as MovieTexture;
        m_audioSource = GetComponent<AudioSource>();
        m_audioSource.clip = m_movieTexture.audioClip;

        m_movieTexture.Play();
        m_movieTexture.loop = true;
        m_audioSource.Play();
    }

    public void playVideo()
    {
        m_movieTexture.Play();
    }
    public void stopVideo()
    {
        m_movieTexture.Stop();
    }
}
