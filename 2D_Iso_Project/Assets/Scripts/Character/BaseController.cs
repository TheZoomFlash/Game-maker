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
        audioSource = GetComponent<AudioSource>();
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


    protected virtual void MeleeAttackStart()
    {
        m_animator.SetInteger(hash_attack, attackIndex);
    }


    protected virtual void HitStart()
    {
        m_animator.SetTrigger(hash_hit);
    }


    public void Hit(Damager Damager, Damageable Damageable)
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

        m_sprite.color = m_OriginalColor;
    }



    protected virtual void DieStart()
    {
        m_animator.SetTrigger(hash_dead);
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
        audioSource.PlayOneShot(clip);
    }
}
