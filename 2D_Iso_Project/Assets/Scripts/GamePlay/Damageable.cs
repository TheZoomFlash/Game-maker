using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Serializable]
    public class HealthEvent : UnityEvent<int>
    { }

    [Serializable]
    public class DamageEvent : UnityEvent<Damager, Damageable>
    { }

    [Serializable]
    public class HealEvent : UnityEvent<int, Damageable>
    { }

    public HealthBar healthBar;
    protected IntBar health;
    public int maxHealeh = 100;
    public bool IsAlive => !health.IsEmpty;
    public bool IsDead => health.IsEmpty;
    public bool NeedHealth => !health.IsFull;

    protected Vector2 m_DamageDirection;
    public Vector2 DamageDirection { get { return m_DamageDirection; } }
    public bool isBeatBack = false;
    public float DamageBackDis = 1f;

    public bool invulnerableAfterDamage = true;
    public float invulnerabilityDuration = 0.5f;
    protected float m_InulnerabilityTimer;
    protected bool m_Invulnerable = false;
    public void DisableInvulnerability() => m_Invulnerable = false;
    public void EnableInvulnerability(bool ignoreTimer = false)
    {
        m_Invulnerable = true;
        //technically don't ignore timer, just set it to an insanly big number. Allow to avoid to add more test & special case.
        m_InulnerabilityTimer = ignoreTimer ? float.MaxValue : invulnerabilityDuration;
    }

    [Tooltip("An offset from the obejct position used to set from where the distance to the Damager is computed")]
    public Vector2 centreOffset = new Vector2(0f, 1f);
    //public bool disableOnDeath = false;

    //[HideInInspector]
    public HealthEvent OnHealthSet;
    public HealthEvent OnMaxHealthSet;
    public HealEvent OnGainHealth;
    public DamageEvent OnHit;
    public DamageEvent OnStun;
    public DamageEvent OnDie;

    private void Awake()
    {
        if (healthBar == null)
            healthBar = GetComponentInChildren<HealthBar>();

        if (healthBar)
        {
            OnHealthSet.AddListener(healthBar.SetHealth);
            OnMaxHealthSet.AddListener(healthBar.SetMaxHealth);
        }
    }

    void Start()
    {
        SetMaxHealth();
    }

    void FixedUpdate()
    {
        if (m_Invulnerable)
        {
            m_InulnerabilityTimer -= Time.deltaTime;

            if (m_InulnerabilityTimer <= 0f)
            {
                m_Invulnerable = false;
            }
        }
    }


    public void TakeDamage(Damager Damager, bool ignoreInvincible = false, bool isStun = false)
    {
        //Debug.Log(transform.name + " TakeDamage");
        if (IsDead || (m_Invulnerable && !ignoreInvincible) )
            return;

        //we can reach that point if the Damager was one that was ignoring invincible state.
        //We still want the callback that we were hit, but not the damage to be removed from health.
        SetHealth(health.CurrentValue - Damager.damage);

        if (IsAlive)
        {
            if (isStun)
                OnStun.Invoke(Damager, this);
            else
                OnHit.Invoke(Damager, this);

            if (invulnerableAfterDamage)
                EnableInvulnerability();
        }

        CheckDie();
        //m_DamageDirection = transform.position + (Vector3)centreOffset - Damager.transform.position;
    }

    private void SetMaxHealth()
    {
        health = new IntBar(maxHealeh);
        OnMaxHealthSet.Invoke(maxHealeh);
    }

    private void SetHealth(int amount)
    {
        health.SetValue(amount);
        OnHealthSet.Invoke(amount);
    }

    public void GainHealth(int amount)
    {
        SetHealth(health.CurrentValue + amount);
        OnGainHealth.Invoke(amount, this);
    }

    public void CheckDie()
    {
        if (IsDead)
        {
            OnDie.Invoke(null, this);
            EnableInvulnerability();
            //if (disableOnDeath)
            //    gameObject.SetActive(false);
        }
    }


}

