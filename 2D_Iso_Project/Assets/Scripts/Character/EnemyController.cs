using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;
#if UNITY_EDITOR
using UnityEditor;
#endif


[RequireComponent(typeof(CharacterMove))]
public class EnemyController : BaseController<CharacterMove>
{
    //****************** script
    Damager meleeDamager;
    Damageable damageable;
    //Damager contactDamager;


    //****************** Move
    public Transform Target;
    public bool isTargetInView = false;

    protected bool isDead { get { return damageable.IsDead; } }


    //[Header("References")]
    //[Tooltip("If the enemy will be using ranged attack, set a prefab of the projectile it should use")]
    //public Bullet projectilePrefab;
    //protected BulletPool m_BulletPool;

    [Header("Scanning settings")]
    [Tooltip("The angle of the forward of the view cone. 0 is right, 90 is up, 180 left etc.")]
    [Range(0.0f, 360.0f)]
    public float viewDirection = 0.0f;
    [Range(0.0f, 360.0f)]
    public float viewFov;
    public float viewDistance;
    [Tooltip("Time in seconds without the target in the view cone before the target is considered lost from sight")]
    public float timeBeforeTargetLost = 3.0f;
    protected float m_TimeSinceLastTargetView;


    [Header("Melee Attack Data")]
    [EnemyMeleeRangeCheck]
    public float meleeRange = 3.0f;
    [Tooltip("if true, the enemy will jump/dash forward when it melee attack")]
    public bool attackDash;
    [Tooltip("The force used by the dash")]
    public Vector2 attackForce;

    [Header("Range Attack Data")]
    [Tooltip("From where the projectile are spawned")]
    public Transform shootingOrigin;
    protected Vector3 m_TargetShootPosition;

    [Header("Misc")]
    [Tooltip("Time in seconds during which the enemy flicker after being hit")]
    public float flickeringDuration;
    protected Coroutine m_FlickeringCoroutine = null;
    protected Color m_OriginalColor;

    protected SpriteRenderer m_SpriteRenderer;

    protected Collider2D m_Collider;



    
    protected float m_FireTimer = 0.0f;

    //as we flip the sprite instead of rotating/scaling the object, this give the forward vector according to the sprite orientation
    //protected Vector2 m_SpriteForward;
    //protected Bounds m_LocalBounds;
    //protected RaycastHit2D[] m_RaycastHitCache = new RaycastHit2D[8];
    //static Collider2D[] s_ColliderCache = new Collider2D[16];


    //****************** animator
    protected readonly int m_HashSpottedPara = Animator.StringToHash("Spotted");
    protected readonly int m_HashShootingPara = Animator.StringToHash("Shooting");
    protected readonly int m_HashTargetLostPara = Animator.StringToHash("TargetLost");
    protected readonly int m_HashMeleeAttackPara = Animator.StringToHash("MeleeAttack");
    protected readonly int m_HashHitPara = Animator.StringToHash("Hit");
    protected readonly int m_HashDeathPara = Animator.StringToHash("Death");


    //****************** Audio
    [Header("Audio")]
    public RandomAudioPlayer shootingAudio;
    public RandomAudioPlayer meleeAttackAudio;
    public RandomAudioPlayer dieAudio;
    public RandomAudioPlayer footStepAudio;

    protected override void OnAwake()
    {
        base.OnAwake();

        m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_OriginalColor = m_SpriteRenderer.color;

        m_animator = GetComponentInChildren<Animator>();
        m_Collider = GetComponentInChildren<Collider2D>();
        meleeDamager = GetComponent<Damager>();
        damageable = GetComponent<Damageable>();

        //if (projectilePrefab != null)
        //    m_BulletPool = BulletPool.GetObjectPool(projectilePrefab.gameObject, 8);
    }

    private void Start()
    {
        SceneLinkedSMB<EnemyController>.Initialise(m_animator, this);
        Target = PlayerController.PlayerInstance.transform;
        //m_LocalBounds = new Bounds();
        //int count = m_body.Rigidbody2D.GetAttachedColliders(s_ColliderCache);
        //for (int i = 0; i < count; ++i)
        //{
        //    m_LocalBounds.Encapsulate(transform.InverseTransformBounds(s_ColliderCache[i].bounds));
        //}
    }

    protected override void OnFixedUpdate()
    {
        if (isDead)
            return;

        base.OnFixedUpdate();
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if (m_TimeSinceLastTargetView > 0.0f)
            m_TimeSinceLastTargetView -= Time.deltaTime;

        if (m_FireTimer > 0.0f)
            m_FireTimer -= Time.deltaTime;
    }

    public void SetHorizontalSpeed(float horizontalSpeed)
    {
        //m_MoveVector.x = horizontalSpeed * m_SpriteForward.x;
    }

    public bool CheckForObstacle(float forwardDistance)
    {
        ////we circle cast with a size sligly small than the collider height. That avoid to collide with very small bump on the ground
        //if (Physics2D.CircleCast(m_Collider.bounds.center, m_Collider.bounds.extents.y - 0.2f, m_SpriteForward, forwardDistance, m_body.m_ContactFilter.layerMask.value))
        //{
        //    return true;
        //}

        //Vector3 castingPosition = (Vector2)(transform.position + m_LocalBounds.center) + m_SpriteForward * m_LocalBounds.extents.x;
        //Debug.DrawLine(castingPosition, castingPosition + Vector3.down * (m_LocalBounds.extents.y + 0.2f));

        //if (!Physics2D.CircleCast(castingPosition, 0.1f, Vector2.down, m_LocalBounds.extents.y + 0.2f, m_body.m_layerMask.value))
        //{
        //    return true;
        //}

        return false;
    }

    public void SetFacingData(Vector2 faceDir)
    {
        m_body.SetFaceDir(faceDir);
    }

    public void SetMoveVector(Vector2 newMoveVector)
    {
        m_body.Move(newMoveVector);
    }

    public void SetTarget(Transform trans)
    {
        Target = trans;
    }


    public void ScanForPlayer()
    {
        //If the player don't have control, they can't react, so do not pursue them
        if (!PlayerInput.Instance.HaveControl)
            return;

        CheckTargetIsNear();
        if (!isTargetInView)
            return;

        //m_animator.SetTrigger(m_HashSpottedPara);
    }

    public void CheckTargetIsNear()
    {
        Vector2 dis = Target.position - transform.position;

        if (dis.sqrMagnitude > viewDistance * viewDistance)
        {
            isTargetInView = false;
            return;
        }
            

        float angle = Vector2.Angle(m_body.FaceDir, dis);
        Debug.Log("dis :" + dis + ", angle :" + angle + ", viewFov :" + viewFov * 0.5f);

        if (angle > viewFov * 0.5f)
        {
            isTargetInView = false;
            return;
        }

        isTargetInView = true;
        m_TimeSinceLastTargetView = timeBeforeTargetLost;
    }


    public void CheckTargetStillVisible()
    {
        if (Target == null)
            return;

        CheckTargetIsNear();

        if (!isTargetInView && m_TimeSinceLastTargetView <= 0.0f)
        {
            ForgetTarget();
        }
    }

    public void OrientToTarget()
    {
        if (Target == null)
            return;

        Vector2 toTarget = Target.position - transform.position;
        SetFacingData(toTarget.normalized);
    }

    

    public void ForgetTarget()
    {
        //m_animator.SetTrigger(m_HashTargetLostPara);
        //Target = null;
    }

    //This is used in case where there is a delay between deciding to shoot and shoot (e.g. Spitter that have an animation before spitting)
    //so we shoot where the target was when the animation started, not where it is when the actual shooting happen
    public void RememberTargetPos()
    {
        if (Target == null)
            return;

        m_TargetShootPosition = Target.position;
    }

    //Call every frame when the enemy is in pursuit to check for range & Trigger the attack if in range
    public void CheckMeleeAttack()
    {
        if (Target == null)
        {//we lost the target, shouldn't shoot
            return;
        }

        if ((Target.position - transform.position).sqrMagnitude < (meleeRange * meleeRange))
        {
            m_animator.SetTrigger(m_HashMeleeAttackPara);
            meleeAttackAudio.PlayRandomSound();
        }
    }

    //This is called when the Damager get enabled (so the enemy can damage the player). Likely be called by the animation throught animation event (see the attack animation of the Chomper)
    public void StartAttack()
    {
        //meleeDamager.EnableDamage();
        //meleeDamager.gameObject.SetActive(true);

        //if (attackDash)
        //    m_MoveVector = new Vector2(m_SpriteForward.x * attackForce.x, attackForce.y);
    }

    public void EndAttack()
    {
        //if (meleeDamager != null)
        //{
        //    meleeDamager.gameObject.SetActive(false);
        //    meleeDamager.DisableDamage();
        //}
    }

    //This is call each update if the enemy is in a attack/shooting state, but the timer will early exit if too early to shoot.
    public void CheckShootingTimer()
    {
        if (m_FireTimer > 0.0f)
            return;

        if (Target == null)
        {//we lost the target, shouldn't shoot
            return;
        }

        //m_animator.SetTrigger(m_HashShootingPara);
        //shootingAudio.PlayRandomSound();
        //m_FireTimer = 1.0f;
    }

    public void Shooting()
    {
        //Vector2 shootPosition = shootingOrigin.transform.localPosition;
        //shootingAudio.PlayRandomSound();
        //BulletObject obj = m_BulletPool.Pop(shootingOrigin.TransformPoint(shootPosition));
        //obj.rigidbody2D.velocity = (GetProjectilVelocity(m_TargetShootPosition, shootingOrigin.transform.position));
    }

    //This will give the velocity vector needed to give to the bullet rigidbody so it reach the given target from the origin.
    private Vector3 GetProjectilVelocity(Vector3 target, Vector3 origin)
    {
        const float projectileSpeed = 30.0f;

        Vector3 velocity = Vector3.zero;
        Vector3 toTarget = target - origin;

        float gSquared = Physics.gravity.sqrMagnitude;
        float b = projectileSpeed * projectileSpeed + Vector3.Dot(toTarget, Physics.gravity);
        float discriminant = b * b - gSquared * toTarget.sqrMagnitude;

        // Check whether the target is reachable at max speed or less.
        if (discriminant < 0)
        {
            velocity = toTarget;
            velocity.y = 0;
            velocity.Normalize();
            velocity.y = 0.7f;

            velocity *= projectileSpeed;
            return velocity;
        }

        float discRoot = Mathf.Sqrt(discriminant);

        // Highest
        float T_max = Mathf.Sqrt((b + discRoot) * 2f / gSquared);

        // Lowest speed arc
        float T_lowEnergy = Mathf.Sqrt(Mathf.Sqrt(toTarget.sqrMagnitude * 4f / gSquared));

        // Most direct with max speed
        float T_min = Mathf.Sqrt((b - discRoot) * 2f / gSquared);

        float T = 0;

        // 0 = highest, 1 = lowest, 2 = most direct
        int shotType = 1;

        switch (shotType)
        {
            case 0:
                T = T_max;
                break;
            case 1:
                T = T_lowEnergy;
                break;
            case 2:
                T = T_min;
                break;
            default:
                break;
        }

        velocity = toTarget / T - Physics.gravity * T / 2f;

        return velocity;
    }

    public void Die(Damager Damager, Damageable Damageable)
    {

        //m_animator.SetTrigger(m_HashDeathPara);

        //dieAudio.PlayRandomSound();
    }

    public void Hit(Damager Damager, Damageable Damageable)
    {
        if (isDead)
            return;

        
        Vector2 DamagerDir = Damager.transform.position - transform.position;

        m_body.ForceMove(DamagerDir.normalized * damageable.DamageBackDis);

        if (m_FlickeringCoroutine != null)
        {
            StopCoroutine(m_FlickeringCoroutine);
            m_SpriteRenderer.color = m_OriginalColor;
        }

        m_FlickeringCoroutine = StartCoroutine(Flicker(Damageable.invulnerabilityDuration));

        //m_animator.SetTrigger(m_HashHitPara);
    }



    protected IEnumerator Flicker(float duration)
    {
        float timer = 0f;
        float sinceLastChange = 0.0f;

        Color transparent = m_OriginalColor;
        transparent.a = 0.2f;
        int state = 1;

        m_SpriteRenderer.color = transparent;

        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            sinceLastChange += Time.deltaTime;
            if (sinceLastChange > flickeringDuration)
            {
                sinceLastChange -= flickeringDuration;
                state = 1 - state;
                m_SpriteRenderer.color = state == 1 ? transparent : m_OriginalColor;
            }
        }

        m_SpriteRenderer.color = m_OriginalColor;
    }

    public void DisableDamage()
    {
        if (meleeDamager != null)
            meleeDamager.DisableDamage();
        //if (contactDamager != null)
        //    contactDamager.DisableDamage();
    }

    public void PlayFootStep()
    {
        footStepAudio.PlayRandomSound();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //draw the cone of view
        bool spriteFaceLeft = true;
        Vector3 forward = spriteFaceLeft ? Vector2.left : Vector2.right;
        forward = Quaternion.Euler(0, 0, spriteFaceLeft ? -viewDirection : viewDirection) * forward;

        //if (GetComponent<SpriteRenderer>().flipX) forward.x = -forward.x;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, viewFov, viewDistance);

        //Draw attack range
        Handles.color = new Color(1.0f, 0, 0, 0.1f);
        Handles.DrawSolidDisc(transform.position, Vector3.back, meleeRange);
    }
#endif
}

//bit hackish, to avoid to have to redefine the whole inspector, we use an attirbute and associated property drawer to 
//display a warning above the melee range when it get over the view distance.
public class EnemyMeleeRangeCheckAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(EnemyMeleeRangeCheckAttribute))]
public class EnemyMeleeRangePropertyDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty viewRangeProp = property.serializedObject.FindProperty("viewDistance");
        if (viewRangeProp.floatValue < property.floatValue)
        {
            Rect pos = position;
            pos.height = 30;
            EditorGUI.HelpBox(pos, "Melee range is bigger than View distance. Note enemies only attack if target is in their view range first", MessageType.Warning);
            position.y += 30;
        }

        EditorGUI.PropertyField(position, property, label);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty viewRangeProp = property.serializedObject.FindProperty("viewDistance");
        if (viewRangeProp.floatValue < property.floatValue)
        {
            return base.GetPropertyHeight(property, label) + 30;
        }
        else
            return base.GetPropertyHeight(property, label);
    }
}
#endif