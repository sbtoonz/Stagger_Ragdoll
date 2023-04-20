using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisEquipper : MonoBehaviour
{
    [Header("Attachment points")]
    public Transform m_leftHand;
    public Transform m_rightHand;
    public Transform m_helmet;
    public Transform m_backShield;
    public Transform m_backMelee;
    public Transform m_backTwohandedMelee;
    public Transform m_backBow;
    public Transform m_backTool;
    public Transform m_backAtgeir;
    public CapsuleCollider[] m_clothColliders = new CapsuleCollider[0];
    public Transform m_hips;
    public Transform m_leftFoot;
    public Transform m_rightFoot;
    
    private VisEquipment _visEquipment;
    private FootStep _footStep;
    private CharacterAnimEvent _animEvent;
    private void OnEnable()
    {
        _visEquipment = transform.parent.gameObject.GetComponentInParent<VisEquipment>();
        _visEquipment.m_leftHand = m_leftHand;
        _visEquipment.m_rightHand = m_rightHand;
        _visEquipment.m_helmet = m_helmet;
        _visEquipment.m_backShield = m_backShield;
        _visEquipment.m_backMelee = m_backMelee;
        _visEquipment.m_backTwohandedMelee = m_backTwohandedMelee;
        _visEquipment.m_backBow = m_backBow;
        _visEquipment.m_backTool = m_backTool;
        _visEquipment.m_backAtgeir = m_backAtgeir;
        _visEquipment.m_clothColliders = m_clothColliders;

        _footStep = transform.parent.gameObject.GetComponentInParent<FootStep>();
        _footStep.m_feet[0] = m_leftFoot;
        _footStep.m_feet[1] = m_rightFoot;

        _animEvent = transform.GetComponentInParent<CharacterAnimEvent>();
        _animEvent.m_feets[0].m_transform = m_leftFoot;
        _animEvent.m_feets[1].m_transform = m_rightFoot;
    }
}
