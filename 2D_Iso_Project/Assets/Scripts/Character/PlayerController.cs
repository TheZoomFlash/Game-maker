using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class PlayerController : MonoBehaviour
{
    static public PlayerController PlayerInstance { get; private set; }

    //****************** animator
    Animator animator;


    //****************** Audio
    [Header("Audio settings")]
    public AudioClip attackClip;
    public AudioClip hitClip;
    AudioSource audioSource;


    // script
    CharacterController2D m_body;
    Damager damager;
    Damageable damageable;

    void Awake()
    {
        PlayerInstance = this;
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        m_body = GetComponent<CharacterController2D>();
        damager = GetComponent<Damager>();
        damageable = GetComponent<Damageable>();
    }

    void Start()
    {
        SceneLinkedSMB<PlayerController>.Initialise(animator, this);
    }

    void FixedUpdate()
    {
        MoveUpdate();
        AnimationUpdate();
        CheckAttack();
    }


    void MoveUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(horizontal, Vertical);
        m_body.Move(movement);
    }


    void AnimationUpdate()
    {
        animator.SetFloat("xDir", m_body.faceDir.x);
        animator.SetFloat("yDir", m_body.faceDir.y);
        animator.SetFloat("speed", m_body.velocity);
    }

    void CheckAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AttemptAttack();
        }
    }

    void AttemptAttack()
    {
        Vector2 mousePos = CameraController.Instance.GameplayCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - (Vector2)m_body.position).normalized;
        damager.Attack(dir);
    }



    public void PlaySource(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void SetChekpoint(Checkpoint checkpoint)
    {
        //m_LastCheckpoint = checkpoint;
    }











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
