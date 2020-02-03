using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseController<T> : MonoBehaviour 
    where T : CharacterMove
{
    protected T m_body;
    public Vector2 Position { get { return m_body.Position; } }

    //****************** animator
    protected Animator m_animator;
    protected int attackIndex = 0;
    protected readonly int hash_xDir = Animator.StringToHash("xDir");
    protected readonly int hash_yDir = Animator.StringToHash("yDir");
    protected readonly int hash_velocity = Animator.StringToHash("velocity");
    protected readonly int hash_attack = Animator.StringToHash("meleeAttack");
    protected readonly int hash_hit = Animator.StringToHash("hit");
    protected readonly int hash_dead = Animator.StringToHash("dead");


    //****************** Audios
    [Header("Audio settings")]
    public AudioClip attackClip;
    public AudioClip hitClip;
    protected AudioSource audioSource;


    //public Anim
    public void DisMovable() => m_body.DisableMove();
    public void EnMovable() => m_body.EnableMove();
    public void ResetAttack() => m_animator.SetInteger(hash_attack, 0);

    void Awake()
    {
        OnAwake();
    }

    protected virtual void OnAwake()
    {
        audioSource = GetComponent<AudioSource>();
        m_body = GetComponent<T>();
    }


    void FixedUpdate()
    {
        OnFixedUpdate();
    }

    protected virtual void OnFixedUpdate()
    {
        AnimationUpdate();
    }

    protected void AnimationUpdate()
    {
        m_animator.SetFloat(hash_xDir, m_body.FaceDir.x);
        m_animator.SetFloat(hash_yDir, m_body.FaceDir.y);
        m_animator.SetFloat(hash_velocity, m_body.Velocity);
    }

    protected virtual void StartDie()
    {
        m_animator.SetTrigger(hash_dead);
    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }

    protected void PlaySource(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
