using System;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [Serializable]
    public class HealthEvent : UnityEvent<Damageable>
    { }

    [Serializable]
    public class DamageEvent : UnityEvent<Damager, Damageable>
    { }

    [Serializable]
    public class HealEvent : UnityEvent<int, Damageable>
    { }

    public int maxHealeh = 5;
    protected int m_CurrentHealth;
    public int CurrentHealth { get { return m_CurrentHealth; } }
    public bool IsAlive => m_CurrentHealth > 0;
    public bool IsDead => !IsAlive;
    public bool NeedHealth => m_CurrentHealth < maxHealeh;

    protected Vector2 m_DamageDirection;
    public Vector2 DamageDirection { get { return m_DamageDirection; } }
    public bool isBeatBack = false;
    public float DamageBackDis = 3f;

    public bool invulnerableAfterDamage = true;
    public float invulnerabilityDuration = 3f;
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
    public bool disableOnDeath = false;

    //[HideInInspector]
    public HealthEvent OnHealthSet;
    public DamageEvent OnTakeDamage;
    public DamageEvent OnDie;
    public HealEvent OnGainHealth;

    void Awake()
    {
        SetHealth(maxHealeh);
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


    public void TakeDamage(Damager Damager, bool ignoreInvincible = false)
    {
        //Debug.Log(transform.name + " TakeDamage");
        if ((m_Invulnerable && !ignoreInvincible) || m_CurrentHealth <= 0)
            return;

        //we can reach that point if the Damager was one that was ignoring invincible state.
        //We still want the callback that we were hit, but not the damage to be removed from health.
        if (!m_Invulnerable)
        {
            SetHealth(m_CurrentHealth - Damager.damage);
        }

        m_DamageDirection = transform.position + (Vector3)centreOffset - Damager.transform.position;

        if(m_CurrentHealth > 0)
            OnTakeDamage.Invoke(Damager, this);
    }

    private void SetHealth(int amount)
    {
        m_CurrentHealth = Mathf.Clamp(amount, 0, maxHealeh);
        OnHealthSet.Invoke(this);

        CheckDie();
    }

    public void GainHealth(int amount)
    {
        SetHealth(m_CurrentHealth + amount);
        OnGainHealth.Invoke(amount, this);
    }

    public void CheckDie()
    {
        if (m_CurrentHealth <= 0)
        {
            OnDie.Invoke(null, this);
            EnableInvulnerability();
            if (disableOnDeath)
                gameObject.SetActive(false);
        }
    }


}

