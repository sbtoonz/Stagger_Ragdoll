using System;
using UnityEngine;

namespace RagDoller.MonoScripts;

public class CollisionRedirector : MonoBehaviour
{
    // ReSharper disable once NotAccessedField.Local
    private Collider? m_collider;
    private float RayCastOriginOffset=3;

    private void Awake()
    {
        m_collider = gameObject.GetComponent<Collider>();
    }
    

    private void OnCollisionStay(Collision other)
    {
        if(Player.m_localPlayer)Player.m_localPlayer.OnCollisionStay(other);
    }
}