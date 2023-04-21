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
    public bool isRagDollActive = false;
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
            ToggleColliders(true);
            ToggleRotation(false);
            isRagDollActive = true;
            var targ = transform.position - collisionLocation;
            m_animator.enabled = false;
            gameObject.GetComponent<Rigidbody>().AddForce(targ.normalized * targ.magnitude);
        }
        else
        {
            
            ToggleColliders(false);
            ToggleRotation(true);
            isRagDollActive = false;
            m_animator.enabled = true;
            m_animator.SetBool("Wakeup", true);

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
    int i = 0;
    private void LateUpdate()
    {
        if (!isRagDollActive)
        {
            i = 0;
            return;
        }

        ++i;
        Vector3 target = Vector3.Lerp(transform.position, hips.position, Time.deltaTime);
        float f;
        Heightmap.GetHeight(hips!.position, out f);
        target.y = f+0.5f;
        hips.position = target;
        if (i >=  (60*RagDollerMod._lengthToWait.Value))SetRagDoll(Vector3.zero);
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