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
    [Space]

    //****************** Move
    [HideInInspector]
    public Transform Target;
    //[HideInInspector]
    bool isTargetInView = false;

    Vector2[] PatrolList;
    float partrolWaitTime = 1f;
    float partrolTimer = 0f;
    int patrolIndex = 0;

    //[Header("References")]
    //[Tooltip("If the enemy will be using ranged attack, set a prefab of the projectile it should use")]
    //public Bullet projectilePrefab;
    //protected BulletPool m_BulletPool;

    [Space]
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

    [Header("Range Attack Data")]
    [Tooltip("From where the projectile are spawned")]
    public GameObject attackPre;
    public void ShowAttention(bool b) { if (attackPre) attackPre.SetActive(b); }

    //public Transform shootingOrigin;
    protected Vector3 m_TargetShootPosition;

    public LayerMask obstacleLay;
    Collider2D m_Collider;
    float checkDistance;

    //as we flip the sprite instead of rotating/scaling the object, this give the forward vector according to the sprite orientation
    //protected Vector2 m_SpriteForward;
    //protected Bounds m_LocalBounds;
    //protected RaycastHit2D[] m_RaycastHitCache = new RaycastHit2D[8];
    //static Collider2D[] s_ColliderCache = new Collider2D[16];


    //****************** animator
    protected readonly int m_HashSpottedPara = Animator.StringToHash("Spotted");
    protected readonly int m_HashShootingPara = Animator.StringToHash("Shooting");
    protected readonly int m_HashTargetLostPara = Animator.StringToHash("TargetLost");


    //****************** Audio
    //[Header("Audio")]


    protected override void OnAwake()
    {
        base.OnAwake();

        m_animator = GetComponentInChildren<Animator>();
        m_Collider = GetComponentInChildren<Collider2D>();

        //if (projectilePrefab != null)
        //    m_BulletPool = BulletPool.GetObjectPool(projectilePrefab.gameObject, 8);
    }

    void Start()
    {
        Target = PlayerController.PlayerInstance.transform;
        Transform PatrolPosition = transform.Find("PatrolPosition");
        if(PatrolPosition)
        {
            PatrolList = new Vector2[PatrolPosition.childCount];
            for (int i = 0; i < PatrolPosition.childCount; i++)
            {
                PatrolList[i] = PatrolPosition.GetChild(i).position;
            }
        }

        SceneLinkedSMB<EnemyController>.Initialise(m_animator, this);

        //m_LocalBounds = new Bounds();
        //int count = m_body.Rigidbody2D.GetAttachedColliders(s_ColliderCache);
        //for (int i = 0; i < count; ++i)
        //{
        //    m_LocalBounds.Encapsulate(transform.InverseTransformBounds(s_ColliderCache[i].bounds));
        //}
    }

    protected override void OnFixedUpdate()
    {
        base.OnFixedUpdate();
        UpdateTimers();
    }

    void UpdateTimers()
    {
        if (m_TimeSinceLastTargetView > 0.0f)
            m_TimeSinceLastTargetView -= Time.deltaTime;
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
        if (m_TimeSinceLastTargetView > 0f || isTargetInView)
            AttackOrRun();
        else
            Patrol();
    }

    void CheckTargetIsNear()
    {
        isTargetInView = false;
        Vector2 dis = Target.position - transform.position;
        if (CheckForObstacle(dis))
        {
            m_TimeSinceLastTargetView = 0f;
            return;
        }
            
        if (dis.sqrMagnitude > viewDistance * viewDistance)
            return;
        float angle = Vector2.Angle(m_body.FaceDir, dis);
        if (angle > viewFov * 0.5f)
            return;

        isTargetInView = true;
        m_TimeSinceLastTargetView = timeBeforeTargetLost;
    }


    bool CheckForObstacle(Vector2 dis)
    {
        //we circle cast with a size sligly small than the collider height. That avoid to collide with very small bump on the ground
        //if (Physics2D.CircleCast(m_Collider.bounds.center, m_Collider.bounds.extents.y - 0.2f,
        //    dir, checkDistance, m_body.m_ContactFilter.layerMask.value))
        //{
        //    return true;
        //}

        Vector2 castingPosition = transform.position;// + m_LocalBounds.center) + dir * m_LocalBounds.extents.x;
        //Debug.DrawLine(castingPosition, castingPosition + dis);// Vector3.down * (m_LocalBounds.extents.y + 0.2f));

        RaycastHit2D hit = Physics2D.Raycast(castingPosition, dis.normalized,
            Mathf.Min(viewDistance, dis.magnitude), obstacleLay.value);
        if (hit.collider != null)
        {
            return true;
        }

        return false;
    }

    public void CheckTargetStillVisible()
    {
        //if (Target == null)
        //    return;

        //CheckTargetIsNear();

        //if (!isTargetInView && m_TimeSinceLastTargetView <= 0.0f)
        //{
        //    ForgetTarget();
        //}
    }

    public void OrientToTarget()
    {
        if (Target == null)
            return;

        Vector2 toTarget = Target.position - transform.position;
        SetFacingData(toTarget.normalized);
    }

    void AttackOrRun()
    {
        if(CheckForMeleeAttack())
        {
            attackIndex = 1;
            MeleeAttackAnim();
        }
        else
        {
            RunToTarget();
        }
    }

    void RunToTarget()
    {
        Vector2 toTarget = (Target.position - transform.position).normalized;
        m_body.Move(toTarget);
    }


    void Patrol()
    {
        if (PatrolList == null || PatrolList.Length == 0)
            return;

        Vector2 moveDis = Vector2.zero;
        if(Vector2.Distance(m_body.Position, PatrolList[patrolIndex]) > 0.1f)
            moveDis = (PatrolList[patrolIndex] - m_body.Position).normalized;
        else
        {
            if (partrolTimer > partrolWaitTime)
            {
                patrolIndex = (patrolIndex + 1) % PatrolList.Length;
                partrolTimer = 0;
            }   
            else
                partrolTimer += Time.deltaTime;
        }
        m_body.Move(moveDis);
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

    bool CheckForMeleeAttack()
    {
        if (!damager.IsCanDamage)
            return false;

        if (Vector2.Distance(Target.position, transform.position) < damager.damageRange)
            return true;
        else
            return false;
    }

    //This is called when the Damager get enabled (so the enemy can damage the player). Likely be called by the animation throught animation event (see the attack animation of the Chomper)
    public void MeleeAttack()
    {
        if (Target == null)
            return;

        Vector2 dir = (Target.position - transform.position).normalized;
        MeleeAttack(dir);
    }

    public override void Hit(Damager Damager, Damageable Damageable)
    {
        base.Hit(Damager, Damageable);

        Vector2 DamagerDir = (Damager.transform.position - transform.position).normalized;
        m_body.SetFaceDir(DamagerDir);
    }

    //This is call each update if the enemy is in a attack/shooting state, but the timer will early exit if too early to shoot.
    public void CheckShootingTimer()
    {
        //if (m_FireTimer > 0.0f)
        //    return;

        //if (Target == null)
        //{//we lost the target, shouldn't shoot
        //    return;
        //}

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




    public void DisableDamage()
    {
        if (damager != null)
            damager.DisableDamage();
        //if (contactDamager != null)
        //    contactDamager.DisableDamage();
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //draw the cone of view
        Vector3 forward = Vector2.right;
        forward = Quaternion.Euler(0, 0, viewDirection) * forward;

        Vector3 endpoint = transform.position + (Quaternion.Euler(0, 0, viewFov * 0.5f) * forward);

        Handles.color = new Color(0, 1.0f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, -Vector3.forward, (endpoint - transform.position).normalized, viewFov, viewDistance);

        Handles.color = Color.yellow;
        Transform PatrolPosition = transform.Find("PatrolPosition");
        if (PatrolPosition)
        {
            PatrolList = new Vector2[PatrolPosition.childCount];
            for (int i = 0; i < PatrolPosition.childCount; i++)
            {
                int j = (i + 1) % PatrolPosition.childCount;
                Handles.DrawLine(PatrolPosition.GetChild(i).position, PatrolPosition.GetChild(j).position);
            }
        }
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