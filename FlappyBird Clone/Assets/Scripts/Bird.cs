using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;

public class Bird : MonoBehaviour
{
    private const float JumpAmt = 90f;
    private Rigidbody2D rb;
    private static Bird instance;
    private State state;

    public event EventHandler OnDied;
    public event EventHandler OnStart;

    public static Bird GetInstance()
    {
        return instance;
    }

    private enum State
    {
        WaitingToStart,
        Playing,
        Dead,
    }

    private void Update()
    {
        switch (state)
        {
            default:
            case State.WaitingToStart:
                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0)))
                {
                    state = State.Playing;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    Jump();
                    if (OnStart != null) OnStart(this, EventArgs.Empty);
                }
                break;
            case State.Playing:
                if (Input.GetKeyDown(KeyCode.Space) || (Input.GetMouseButtonDown(0)))
                {
                    Jump();
                }
                break;
            case State.Dead:
                break;
        }
    }

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Static;
        state = State.WaitingToStart;
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * JumpAmt;
        SoundManager.PlaySound(SoundManager.Sound.BirdJump);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        rb.bodyType = RigidbodyType2D.Static;
        SoundManager.PlaySound(SoundManager.Sound.Lose);
        if (OnDied != null) OnDied(this, EventArgs.Empty);
    }
}
