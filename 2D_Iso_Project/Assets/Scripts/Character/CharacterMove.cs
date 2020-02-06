using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Collider2D))]
public class CharacterMove : MonoBehaviour
{
    [Header("move settings")]
    public float moveSpeed = 3f;

    public Rigidbody2D Rigidbody2D { get; protected set; }
    public Vector2 Position { get { return Rigidbody2D.position; } }
    public float Velocity { get; protected set; }
    public Vector2 FaceDir { get; protected set; }

    protected Vector2 m_PreviousPosition;
    protected Vector2 m_CurrentPosition;
    protected Vector2 m_NextMovement;

    public bool isMovable = true;
    public void EnableMove() => isMovable = true;
    public void DisableMove() => isMovable = false;

    public LayerMask m_layerMask;
    public ContactFilter2D m_ContactFilter;
    //CapsuleCollider2D m_Capsule;

    void Init()
    {
        Velocity = 0;
        FaceDir = new Vector2(0, -1);
        m_NextMovement = Vector2.zero;
    }

    void Awake()
    {
        OnAwake();
    }
    protected virtual void OnAwake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();

        m_ContactFilter.layerMask = m_layerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

        Physics2D.queriesStartInColliders = false;

        Init();
    }

    void FixedUpdate()
    {
        if (m_NextMovement == Vector2.zero)
            return;

        m_PreviousPosition = Rigidbody2D.position;
        m_CurrentPosition = m_PreviousPosition + m_NextMovement;
        m_NextMovement = Vector2.zero;

        Rigidbody2D.MovePosition(m_CurrentPosition);
    }


    //***************************** public ***********************************/
    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move(Vector2 movement)
    {
        if (!isMovable)
            movement = Vector2.zero;
            
        SetFaceDir(movement);
        SpeedUpdate(movement);

        m_NextMovement = FaceDir * Velocity * Time.deltaTime;
    }


    // don't change FaceDir and Speed
    public void ForceMove(Vector2 movement)
    {
        m_NextMovement = movement;
    }



    public void SetFaceDir(Vector2 dir)
    {
        if (!Mathf.Approximately(dir.x, 0.0f) || !Mathf.Approximately(dir.y, 0.0f))
        {
            FaceDir = dir.normalized;
        }
    }

    public void FaceBack()
    {
        FaceDir = -FaceDir;
    }

    protected virtual void SpeedUpdate(Vector2 movement)
    {
        Velocity = Mathf.Clamp(movement.magnitude, 0, 1);
        Velocity *= moveSpeed;
    }

    /// <summary>
    /// This moves the character without any implied velocity.
    /// </summary>
    /// <param name="position">The new position of the character in global space.</param>
    public void Teleport(Vector2 position)
    {
        Rigidbody2D.position = position;
    }
}