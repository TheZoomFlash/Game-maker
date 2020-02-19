using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Damager : MonoBehaviour
{
    [Serializable]
    public class DamagableEvent : UnityEvent<Damager, Damageable>
    { }


    [Serializable]
    public class NonDamagableEvent : UnityEvent<Damager>
    { }

    public int damage = 1;
    public float damageRate = 1f;
    private float nextDamageTimer = 0f;
    
    [Tooltip("if true, the enemy will jump/dash forward when it melee attack")]
    public bool attackDash;
    [Tooltip("The Dis used by the dash")]
    public float attackMoveDis = 0.3f;

    // range
    public float damageRange = 1.0f;
    public float angle = 0;
    public Vector2 size = new Vector2(1.5f, 1.0f);

    //[Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
    //public SpriteRenderer spriteRenderer;

    [Tooltip("If disabled, Damager ignore trigger when casting for damage")]
    public bool canHitTriggers;
    public LayerMask hittableLayers;

    [Tooltip("If set, an invincible Damager hit will still get the onHit message (but won't loose any life)")]
    public bool ignoreInvincibility = false;
    //public bool disableDamageAfterHit = false;

    protected bool m_CanDamage = true;
    public bool IsCanDamage { get { return m_CanDamage; } }
    public void EnableDamage() => m_CanDamage = true;
    public void DisableDamage() => m_CanDamage = false;

    protected ContactFilter2D m_AttackContactFilter;
    protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
    protected Vector2 m_DamagerTransform;

    //call that from inside the onDamagerHIt or OnNonDamagerHit to get what was hit.
    protected Collider2D m_LastHit;
    public Collider2D LastHit { get { return m_LastHit; } }

    //public DamagableEvent OnDamageableHit;
    //public NonDamagableEvent OnNonDamageableHit;

    void Awake()
    {
        m_AttackContactFilter.layerMask = hittableLayers;
        m_AttackContactFilter.useLayerMask = true;
        m_AttackContactFilter.useTriggers = canHitTriggers;
    }

    void FixedUpdate()
    {
        if (nextDamageTimer > 0)
        {
            nextDamageTimer -= Time.deltaTime;
            return;
        }
    }

    public int Attack(Vector2 dir, bool stun = false)
    {
        if (!IsCanDamage || nextDamageTimer > 0)
            return 0;

        nextDamageTimer = damageRate;
        //Vector2 scale = m_DamagerTransform.lossyScale;
        //Vector2 scaledSize = Vector2.Scale(size, scale);

        m_DamagerTransform = (Vector2)transform.position + size / 2 * dir;
        angle = Vector2.SignedAngle(Vector2.right, dir);
        int hitCount = Physics2D.OverlapCapsule(m_DamagerTransform,
            size, CapsuleDirection2D.Horizontal, angle, m_AttackContactFilter, m_AttackOverlapResults);
        //Debug.Log("Damage : " + m_DamagerTransform + "; angle" + angle + " , hitCount : " + hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            m_LastHit = m_AttackOverlapResults[i];
            Damageable damageable = m_LastHit.GetComponentInParent<Damageable>();
            if (damageable)
            {
                damageable.TakeDamage(this, ignoreInvincibility, stun);
                //OnDamageableHit.Invoke(this, damageable);
                //if (disableDamageAfterHit)
                //    DisableDamage();
            }
        }

        if(stun)
        {
            GameObject particle = Resources.Load("RisingSteam") as GameObject;
            GameObject obj = Instantiate(particle, m_DamagerTransform, Quaternion.identity);
        }

        //OnNonDamageableHit.Invoke(this);
        return hitCount;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 dir = size / 2 * new Vector2(1.0f, 0.0f);
        Vector3 _center = new Vector3(transform.position.x+ dir.x, transform.position.y + dir.y, 0);

        Handles.color = new Color(1.0f, 0, 0, 0.5f);
        Handles.DrawWireCube(_center, size);

        //Draw attack range
        Handles.color = new Color(0.0f, 0, 1.0f, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.back, damageRange);
    }
#endif

}
