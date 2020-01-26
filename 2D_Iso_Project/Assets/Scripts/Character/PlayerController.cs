using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gamekit2D;

public class PlayerController : MonoBehaviour
{
    static public PlayerController PlayerInstance { get; private set; }

    //****************** Move
    Rigidbody2D rbody;
    public float moveSpeed = 3f;
    public Vector2 faceDir = new Vector2(0,0);
    
    public bool isMovable = true;
    public void EnableMove() => isMovable = true;
    public void DisableMove() => isMovable = false;

    

    //****************** animator
    Animator animator;
    private int lastDirIndex;

    public static readonly string[] staticDirections = { "Static N", "Static NW", "Static W", "Static SW", "Static S", "Static SE", "Static E", "Static NE" };
    public static readonly string[] runDirections = { "Run N", "Run NW", "Run W", "Run SW", "Run S", "Run SE", "Run E", "Run NE" };

    

    void Awake()
    {
        PlayerInstance = this;
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        SceneLinkedSMB<PlayerController>.Initialise(animator, this);

    }

    void FixedUpdate()
    {
        Move();
    }


    void Move()
    {
        if (!isMovable)
            return;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        faceDir = Vector2.ClampMagnitude(new Vector2(horizontalInput, verticalInput), 1);
        Vector2 newPos = rbody.position + faceDir * moveSpeed * Time.fixedDeltaTime;
        rbody.MovePosition(newPos);

        SetDirection(faceDir);
    }

    public void SetChekpoint(Checkpoint checkpoint)
    {
        //m_LastCheckpoint = checkpoint;
    }


    public void SetDirection(Vector2 direction)
    {

        //use the Run states by default
        string[] directionArray = null;

        //measure the magnitude of the input.
        if (direction.magnitude < .01f)
        {
            //if we are basically standing still, we'll use the Static states
            //we won't be able to calculate a direction if the user isn't pressing one, anyway!
            directionArray = staticDirections;
        }
        else
        {
            //we can calculate which direction we are going in
            //use DirectionToIndex to get the index of the slice from the direction vector
            //save the answer to lastDirIndex
            directionArray = runDirections;
            lastDirIndex = DirectionToIndex(direction, 8);
        }

        //tell the animator to play the requested state
        animator.Play(directionArray[lastDirIndex]);
    }

    //helper functions

    //this function converts a Vector2 direction to an index to a slice around a circle
    //this goes in a counter-clockwise direction.
    public static int DirectionToIndex(Vector2 dir, int sliceCount)
    {
        //get the normalized direction
        Vector2 normDir = dir.normalized;
        //calculate how many degrees one slice is
        float step = 360f / sliceCount;
        //calculate how many degress half a slice is.
        //we need this to offset the pie, so that the North (UP) slice is aligned in the center
        float halfstep = step / 2;
        //get the angle from -180 to 180 of the direction vector relative to the Up vector.
        //this will return the angle between dir and North.
        float angle = Vector2.SignedAngle(Vector2.up, normDir);
        //add the halfslice offset
        angle += halfstep;
        //if angle is negative, then let's make it positive by adding 360 to wrap it around.
        if (angle < 0)
        {
            angle += 360;
        }
        //calculate the amount of steps required to reach this angle
        float stepCount = angle / step;
        //round it, and we have the answer!
        return Mathf.FloorToInt(stepCount);
    }

    //this function converts a string array to a int (animator hash) array.
    public static int[] AnimatorStringArrayToHashArray(string[] animationArray)
    {
        //allocate the same array length for our hash array
        int[] hashArray = new int[animationArray.Length];
        //loop through the string array
        for (int i = 0; i < animationArray.Length; i++)
        {
            //do the hash and save it to our hash array
            hashArray[i] = Animator.StringToHash(animationArray[i]);
        }
        //we're done!
        return hashArray;
    }
}
