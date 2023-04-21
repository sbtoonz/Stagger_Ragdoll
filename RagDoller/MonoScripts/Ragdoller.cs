using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RagDoller.MonoScripts;

public class Ragdoller : MonoBehaviour
{
    public List<Rigidbody> ragdollRBs;
    public List<Collider> ragdollColliders;
    public static CapsuleCollider? _playerOrigCapsule = null!;
    public static Animator? m_animator;

    private static Transform hips;
    private static bool isRagDollActive = false;
    private void Start()
    {
        
        ragdollColliders = new List<Collider>();
        ragdollRBs = new List<Rigidbody>();
        _playerOrigCapsule = GetComponent<CapsuleCollider>();
        m_animator = gameObject.GetComponent<Player>().m_animator;
        
        Utilities.ExecuteRbBuild(gameObject.GetComponent<Player>(), ragdollRBs, ragdollColliders);
        hips = ragdollColliders.Find(x => x.name == "Hips").transform;
        StartCoroutine(starter());
        ToggleColliders(false);
        ToggleRotation(true);
    }

    public void SetRagDoll(Vector3 collisionLocation)
    {
        if (!isRagDollActive)
        {
            Vector3 pos = collisionLocation - transform.position;
            var impact = pos.normalized;
            var force = pos.magnitude * 10f;
            gameObject.GetComponent<Rigidbody>().AddForce(impact*force, ForceMode.Impulse);
            
            ToggleColliders(true);
            ToggleRotation(false);
            isRagDollActive = true;
            m_animator.enabled = false;
            hips.gameObject.GetComponent<Rigidbody>().AddRelativeForce(.11f,0,-0.1f);
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

    private void LateUpdate()
    {
        if (!isRagDollActive) return;
        Vector3 target = Vector3.Lerp(transform.position, hips.position, Time.deltaTime);
        float f;
        Heightmap.GetHeight(hips!.position, out f);
        target.y = f+0.5f;
        hips.position = target;

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
            foreach (var c in ragdollColliders.Where(c => c != null))
            {
                c.material = _playerOrigCapsule!.material;
                if(!isRagDollActive)c.gameObject.layer = _playerOrigCapsule!.gameObject.layer;
                if (isRagDollActive) c.gameObject.layer = 17;
                c!.gameObject!.tag =  _playerOrigCapsule!.gameObject!.tag;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }
}