using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable CS8618

public class ImpactRagdoller : MonoBehaviour
{
    public List<Collider> Colliders;
    public List<Rigidbody> Rigidbodies;
    [NonSerialized]public Rigidbody playerRigidBody;

    public static ImpactRagdoller? Instance { get; private set; }

    private CapsuleCollider _playerOrigCapsule = null!;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        _playerOrigCapsule = transform.parent.parent.gameObject.GetComponent<CapsuleCollider>();
        foreach (var c in Colliders)
        {
            c.material = _playerOrigCapsule.material;
            c.gameObject.layer = _playerOrigCapsule.gameObject.layer;
            c.gameObject.tag = _playerOrigCapsule.gameObject.tag;
        }
        StartCoroutine(TestRepeater());
    }

    public IEnumerator TestRepeater()
    {
        while (true)
        {
            foreach (var c in Colliders)
            {
                c.material = _playerOrigCapsule.material;
                c.gameObject.layer = _playerOrigCapsule.gameObject.layer;
                c.gameObject.tag = _playerOrigCapsule.gameObject.tag;
            }
            yield return new WaitForSeconds(0.01f); 
        }
    }

    public void FreezeRotation()
    {
        foreach (var rb in Rigidbodies)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    public void UnlockRotation()
    {
        foreach (var rb in Rigidbodies)
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public void SetKinematic(bool s)
    {
        foreach (var rb in Rigidbodies)
        {
            rb.isKinematic = s;
        }
    }

    public void ToggleColliders(bool s)
    {
        foreach (var c in Colliders)
        {
            c.enabled = s;
        }
    }
  
    private void OnDestroy()
    {
        if (Instance) Instance = null;
    }
}