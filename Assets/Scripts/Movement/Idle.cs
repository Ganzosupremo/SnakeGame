using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[DisallowMultipleComponent]
public class Idle : MonoBehaviour
{
    private Rigidbody2D m_Rb;
    private IdleEvent m_IdleEvent;

    private void Awake()
    {
        m_Rb = GetComponent<Rigidbody2D>();
        m_IdleEvent = GetComponent<IdleEvent>();
    }

    private void OnEnable()
    {
        //Suscribe to the idle event
        m_IdleEvent.OnIdle += IdleEvent_OnIdle;
    }

    private void OnDisable()
    {
        //Unsuscribe to the idle event
        m_IdleEvent.OnIdle -= IdleEvent_OnIdle;
    }

    private void IdleEvent_OnIdle(IdleEvent idleEvent)
    {
        MoveRigidBody();
    }

    /// <summary>
    /// Moves The Rigid Body
    /// </summary>
    private void MoveRigidBody()
    {
        m_Rb.velocity = Vector2.zero;
    }
}
