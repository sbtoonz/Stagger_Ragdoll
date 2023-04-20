using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RagDoller.MonoScripts;

public class Ragdoller : MonoBehaviour
{
    public List<Rigidbody> ragdollRBs;
    public List<Collider> ragdollColliders;
    public static CapsuleCollider? _playerOrigCapsule = null!;
    public static Animator? m_animator;
    
    private static bool isRagDollActive = false;
    private void Start()
    {
        ragdollColliders = new List<Collider>();
        ragdollRBs = new List<Rigidbody>();
        _playerOrigCapsule = GetComponent<CapsuleCollider>();
        m_animator = gameObject.GetComponent<Player>().m_animator;
        
        Utilities.ExecuteRbBuild(gameObject.GetComponent<Player>(), ragdollRBs, ragdollColliders);
        StartCoroutine(starter());
        ToggleColliders(false);
        ToggleRotation(true);
    }

    public void SetRagDoll()
    {
        if (!isRagDollActive)
        {
            ToggleColliders(true);
            ToggleRotation(false);
            isRagDollActive = true;
            m_animator.enabled = false;
        }
        else
        {
            ToggleColliders(false);
            ToggleRotation(true);
            isRagDollActive = false;
            m_animator.enabled = true;

        }
    }
    public void ToggleColliders(bool s)
    {
        foreach (var c in ragdollColliders)
        {
            c.enabled = s;
        }
    }

    public void ToggleRotation(bool s)
    {
        switch (s)
        {
            case true:
                foreach (var rb in ragdollRBs)
                {
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }
                break;
            case false:
                foreach (var rb in ragdollRBs)
                {
                    rb.constraints = RigidbodyConstraints.None;
                }
                break;
        }
        
    }
    public IEnumerator starter()
    {
        yield return new WaitForSeconds(1.25f);
        StartCoroutine(repeater());
    }

    public IEnumerator repeater()
    {
        while (true)
        {
            foreach (var c in  ragdollColliders)
            {
                if(c == null)continue;
                c.material = _playerOrigCapsule!.material;
                if(!isRagDollActive)c.gameObject.layer =  _playerOrigCapsule.gameObject.layer;
                if(isRagDollActive)c.gameObject.layer =  18;
                c!.gameObject!.tag =  _playerOrigCapsule!.gameObject!.tag;
            }
            yield return new WaitForSeconds(0.01f); 
        }
    }
}