using System;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour
{
    [Serializable]
    public class DamagableEvent : UnityEvent<Damager, Damageable>
    { }


    [Serializable]
    public class NonDamagableEvent : UnityEvent<Damager>
    { }

    //call that from inside the onDamagerHIt or OnNonDamagerHit to get what was hit.
    public Collider2D LastHit { get { return m_LastHit; } }

    public int damage = 1;
    public float damageRate = 1f;
    private float nextDamageTimer = 0f;

    public Vector2 offset = new Vector2(1.5f, 1f);
    public Vector2 size = new Vector2(2.5f, 1f);
    [Tooltip("SpriteRenderer used to read the flipX value used by offset Based OnSprite Facing")]
    public SpriteRenderer spriteRenderer;
    [Tooltip("If disabled, Damager ignore trigger when casting for damage")]
    public bool canHitTriggers;
    public bool disableDamageAfterHit = false;
    [Tooltip("If set, an invincible Damager hit will still get the onHit message (but won't loose any life)")]
    public bool ignoreInvincibility = false;
    public LayerMask hittableLayers;
    public DamagableEvent OnDamageableHit;
    public NonDamagableEvent OnNonDamageableHit;

    protected bool m_CanDamage = true;
    public void EnableDamage() => m_CanDamage = true;
    public void DisableDamage() => m_CanDamage = false;

    protected ContactFilter2D m_AttackContactFilter;
    protected Collider2D[] m_AttackOverlapResults = new Collider2D[10];
    protected Transform m_DamagerTransform;
    protected Collider2D m_LastHit;

    void Awake()
    {
        m_AttackContactFilter.layerMask = hittableLayers;
        m_AttackContactFilter.useLayerMask = true;
        m_AttackContactFilter.useTriggers = canHitTriggers;

        m_DamagerTransform = transform;
    }

    void FixedUpdate()
    {
        if (!m_CanDamage)
            return;

        if (nextDamageTimer > 0)
        {
            nextDamageTimer -= Time.deltaTime;
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
            nextDamageTimer = damageRate;
        }
             
    }

    void Attack()
    {
        Vector2 mousePos = CameraController.Instance.GameplayCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)m_DamagerTransform.position).normalized;

        Vector2 scale = m_DamagerTransform.lossyScale;
        Vector2 facingOffset = Vector2.Scale(offset * dir, scale);
        Vector2 scaledSize = Vector2.Scale(size, scale);

        Vector2 pointA = (Vector2)m_DamagerTransform.position + facingOffset - scaledSize * 0.5f;
        Vector2 pointB = pointA + scaledSize;
        int hitCount = Physics2D.OverlapArea(pointA, pointB, m_AttackContactFilter, m_AttackOverlapResults);

        Debug.Log("Damage : " + dir + " , hitCount : " + hitCount);
        for (int i = 0; i < hitCount; i++)
        {
            m_LastHit = m_AttackOverlapResults[i];
            Damageable damageable = m_LastHit.GetComponentInParent<Damageable>();

            if (damageable)
            {
                OnDamageableHit.Invoke(this, damageable);
                damageable.TakeDamage(this, ignoreInvincibility);
                if (disableDamageAfterHit)
                    DisableDamage();
            }
            else
            {
                OnNonDamageableHit.Invoke(this);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        
    }
#endif

}
