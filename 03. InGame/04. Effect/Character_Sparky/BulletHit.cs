using UnityEngine;
using System.Collections;

public class BulletHit : MonoBehaviour
{
    private float m_playTime;
    private AudioSource m_hitSound;

    public void init()
    {
        m_hitSound = GetComponent<AudioSource>();
    }

    public void setPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void setPosition(Vector3 position, Vector3 direction)
    {
        if (m_hitSound != null)
            m_hitSound.Play();

        transform.position = position + direction * 0.1f;
        transform.up = direction;
    }

    public void deActive()
    {
        gameObject.SetActive(false);
    }
}
