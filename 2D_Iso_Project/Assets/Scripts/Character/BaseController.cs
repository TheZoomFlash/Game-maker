﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Damager))]
[RequireComponent(typeof(Damageable))]
public abstract class BaseController<T> : MonoBehaviour 
    where T : CharacterMove
{
    protected T m_body;
    protected Damager damager;
    protected Damageable damageable;
    public Vector2 Position { get { return m_body.Position; } }
    public Vector2 FaceDir { get { return m_body.FaceDir; } }

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
    public AudioClip[] attackClip;
    public AudioClip hitClip;
    public AudioClip dieClip;
    protected AudioSource audioSource;


    //[Tooltip("Time in seconds during which the enemy flicker after being hit")]
    [HideInInspector]
    public SpriteRenderer m_sprite;
    protected float flickerDeltaTime = 0.1f;
    protected Coroutine m_FlickeringCoroutine = null;
    protected Color m_OriginalColor;
    

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
        m_body = GetComponent<T>();
        m_sprite = GetComponentInChildren<SpriteRenderer>();
        damager = GetComponent<Damager>();
        damageable = GetComponent<Damageable>();
        audioSource = GetComponent<AudioSource>();

        damageable.OnTakeDamage.AddListener(Hit);
        damageable.OnDie.AddListener(Die);
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


    public virtual void Teleport(Vector2 pos)
    {
        m_body.Teleport(pos);
    }


    protected virtual void MeleeAttackStart()
    {
        m_animator.SetInteger(hash_attack, attackIndex);
    }


    public void MeleeAttack(Vector2 dir)
    {
        damager.Attack(dir);
        m_body.SetFaceDir(dir);

        if (damager.attackDash)
            m_body.ForceMove(dir * damager.attackMoveDis * Mathf.Sqrt(2 * attackIndex));

        //Debug.Log("attackIndex " + attackIndex);
        if (attackIndex != 0 && attackClip.Length > attackIndex - 1)
            PlaySource(attackClip[attackIndex - 1]);
    }





    protected virtual void HitStart()
    {
        m_animator.SetTrigger(hash_hit);
        PlaySource(hitClip);
    }


    public virtual void Hit(Damager Damager, Damageable Damageable)
    {
        Vector2 DamagerDir = transform.position - Damager.transform.position;

        if (Damageable.isBeatBack)
            m_body.ForceMove(DamagerDir.normalized * Damageable.DamageBackDis);

        if (m_FlickeringCoroutine != null)
            StopCoroutine(m_FlickeringCoroutine);
        m_FlickeringCoroutine = StartCoroutine(Flicker(Damageable.invulnerabilityDuration));

        HitStart();
    }


    protected IEnumerator Flicker(float duration)
    {
        //Debug.Log("Flicker");
        float timer = 0f;
        float sinceLastChange = 0.0f;

        m_OriginalColor = m_sprite.color;
        Color transparent = m_OriginalColor;
        transparent.a = 0.2f;
        int state = 1;

        m_sprite.color = transparent;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            sinceLastChange += Time.deltaTime;
            if (sinceLastChange > flickerDeltaTime)
            {
                sinceLastChange -= flickerDeltaTime;
                state ^= 1;
                m_sprite.color = state == 1 ? transparent : m_OriginalColor;
            }
        }

        m_OriginalColor.a = 1f;
        m_sprite.color = m_OriginalColor;
    }





    protected virtual void DieStart()
    {
        m_animator.SetTrigger(hash_dead);
        PlaySource(dieClip);
    }

    public void Die(Damager Damager, Damageable Damageable)
    {
        DieStart();
    }

    // real dead
    public virtual void RealDie()
    {
        gameObject.SetActive(false);
    }



    protected void PlaySource(AudioClip clip)
    {
        //audioSource.loop = false;
        if(clip)
            audioSource.PlayOneShot(clip);
    }

    protected void PlayEffect(ParticleSystem particle)
    {
        particle.gameObject.SetActive(true);
        if (particle && !particle.isPlaying)
            particle.Play();
    }

    protected void CloseEffect(ParticleSystem particle)
    {
        if (particle)
        {
            particle.gameObject.SetActive(false);
        }
    }
}
