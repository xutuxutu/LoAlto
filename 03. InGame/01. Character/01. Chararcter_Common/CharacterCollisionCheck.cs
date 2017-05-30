using UnityEngine;
using System.Collections;

public class CharacterCollisionCheck : MonoBehaviour
{
    private CharacterController m_controller;

    void Start()
    {
        m_controller = GetComponent<CharacterController>();
    }
}
