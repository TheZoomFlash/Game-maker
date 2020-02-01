using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class PlayerController : MonoBehaviour
{
    static public PlayerController PlayerInstance { get; private set; }
    public Camera m_camera;

    //****************** script
    CharacterController2D m_body;
    Damager damager;
    Damageable damageable;
    CameraShaker m_shaker;


    //****************** animator
    Animator animator;
    protected readonly int hash_xDir = Animator.StringToHash("xDir");
    protected readonly int hash_yDir = Animator.StringToHash("yDir");
    protected readonly int hash_velocity = Animator.StringToHash("velocity");
    protected readonly int hash_attack = Animator.StringToHash("meleeAttack");
    protected readonly int hash_hit = Animator.StringToHash("hit");


    //****************** move
    public void DisMovable() => m_body.DisableMove();
    public void EnMovable() => m_body.EnableMove();
    public float attackMoveDis = 0.3f;


    //****************** meleeAttack
    private int attackIndex = 0;
    private const int Attack_Sequence = 4;
    private const float Press_Delay = 0.5f;
    private bool attack_pressDown = false;
    public void ResetAttack() => animator.SetInteger(hash_attack, 0);
    IEnumerator DisPressAfterDelay()
    {
        yield return new WaitForSeconds(Press_Delay);
        attack_pressDown = false;
        attackIndex = 0;
    }



    //****************** Audios
    [Header("Audio settings")]
    public AudioClip attackClip;
    public AudioClip hitClip;
    AudioSource audioSource;


    void Awake()
    {
        PlayerInstance = this;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        m_body = GetComponent<CharacterController2D>();
        damager = GetComponent<Damager>();
        damageable = GetComponent<Damageable>();
        m_shaker = Camera.main.GetComponent<CameraShaker>();
    }

    void Start()
    {
        SceneLinkedSMB<PlayerController>.Initialise(animator, this);
    }


    void FixedUpdate()
    {
        ProcessInput();
        AnimationUpdate();
    }

    void ProcessInput()
    {
        float horizontal = PlayerInput.Instance.Horizontal.Value;
        float Vertical = PlayerInput.Instance.Vertical.Value;
        Vector2 movement = new Vector2(horizontal, Vertical);
        if(PlayerInput.Instance.Jump.Down)
        {
            m_body.Dash();
        }
        m_body.Move(movement);

        if (PlayerInput.Instance.MeleeAttack.Down)
        {
            attack_pressDown = true;
            StopCoroutine(DisPressAfterDelay());
            StartCoroutine(DisPressAfterDelay());
        }
    }

    void AnimationUpdate()
    {
        animator.SetFloat(hash_xDir, m_body.FaceDir.x);
        animator.SetFloat(hash_yDir, m_body.FaceDir.y);
        animator.SetFloat(hash_velocity, m_body.Velocity);
    }


    public void CheckForMeleeAttack()
    {
        if(attack_pressDown && damager.IsCanDamage)
        {
            attackIndex = attackIndex % Attack_Sequence + 1;
            animator.SetInteger(hash_attack, attackIndex);
            //Debug.Log("attackIndex :" + attackIndex);
        }
    }

    public void MeleeAttack()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)m_body.Position).normalized;
        damager.Attack(dir);

        m_body.ForceMove(dir * attackMoveDis * Mathf.Sqrt(2 * attackIndex));

        m_shaker.Shake();
        //FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
    }




    public void PlaySource(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }












    //public void SetChekpoint(Checkpoint checkpoint)
    //{
    //    m_LastCheckpoint = checkpoint;
    //}

    //public void SetDirection(Vector2 direction)
    //{

    //    //use the Run states by default
    //    string[] directionArray = null;

    //    //measure the magnitude of the input.
    //    if (direction.magnitude < .01f)
    //    {
    //        //if we are basically standing still, we'll use the Static states
    //        //we won't be able to calculate a direction if the user isn't pressing one, anyway!
    //        directionArray = staticDirections;
    //    }
    //    else
    //    {
    //        //we can calculate which direction we are going in
    //        //use DirectionToIndex to get the index of the slice from the direction vector
    //        //save the answer to lastDirIndex
    //        directionArray = runDirections;
    //        lastDirIndex = DirectionToIndex(direction, 8);
    //    }

    //    //tell the animator to play the requested state
    //    animator.Play(directionArray[lastDirIndex]);
    //}

    ////helper functions

    ////this function converts a Vector2 direction to an index to a slice around a circle
    ////this goes in a counter-clockwise direction.
    //public static int DirectionToIndex(Vector2 dir, int sliceCount)
    //{
    //    //get the normalized direction
    //    Vector2 normDir = dir.normalized;
    //    //calculate how many degrees one slice is
    //    float step = 360f / sliceCount;
    //    //calculate how many degress half a slice is.
    //    //we need this to offset the pie, so that the North (UP) slice is aligned in the center
    //    float halfstep = step / 2;
    //    //get the angle from -180 to 180 of the direction vector relative to the Up vector.
    //    //this will return the angle between dir and North.
    //    float angle = Vector2.SignedAngle(Vector2.up, normDir);
    //    //add the halfslice offset
    //    angle += halfstep;
    //    //if angle is negative, then let's make it positive by adding 360 to wrap it around.
    //    if (angle < 0)
    //    {
    //        angle += 360;
    //    }
    //    //calculate the amount of steps required to reach this angle
    //    float stepCount = angle / step;
    //    //round it, and we have the answer!
    //    return Mathf.FloorToInt(stepCount);
    //}

}
