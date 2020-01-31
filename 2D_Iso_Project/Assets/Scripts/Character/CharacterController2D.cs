using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Collider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("move settings")]
    public float moveSpeed = 3f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashInterval = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    public Rigidbody2D Rigidbody2D { get; protected set; }
    public Vector2 position { get { return Rigidbody2D.position; } }
    public float velocity { get; protected set; }
    public Vector2 faceDir { get; protected set; }

    Vector2 m_PreviousPosition;
    Vector2 m_CurrentPosition;
    Vector2 m_NextMovement;

    public bool isMovable = true;
    public void EnableMove() => isMovable = true;
    public void DisableMove() => isMovable = false;

    public LayerMask m_layerMask;
    public ContactFilter2D m_ContactFilter;
    //CapsuleCollider2D m_Capsule;

    void Awake()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        //m_Capsule = GetComponent<CapsuleCollider2D>();

        m_ContactFilter.layerMask = m_layerMask;
        m_ContactFilter.useLayerMask = true;
        m_ContactFilter.useTriggers = false;

        Physics2D.queriesStartInColliders = false;
    }

    void FixedUpdate()
    {
        m_PreviousPosition = Rigidbody2D.position;
        m_CurrentPosition = m_PreviousPosition + m_NextMovement;

        Rigidbody2D.MovePosition(m_CurrentPosition);
        m_NextMovement = Vector2.zero;
    }

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move(Vector2 movement)
    {
        if (!isMovable)
            return;

        DirUpdate(movement);
        SpeedUpdate(movement);

        m_NextMovement = faceDir * velocity * Time.deltaTime;
    }

    public void Dash()
    {
        if (!isMovable || !canDash || isDashing)
            return;

        canDash = false;
        isDashing = true;
        StartCoroutine("StopDash");
        StartCoroutine("CanDash");
    }

    IEnumerator StopDash()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }
    IEnumerator CanDash()
    {
        yield return new WaitForSeconds(dashInterval);
        canDash = true;
    }


    void DirUpdate(Vector2 dir)
    {
        if (!Mathf.Approximately(dir.x, 0.0f) || !Mathf.Approximately(dir.y, 0.0f))
        {
            faceDir = dir.normalized;
        }
    }

    void SpeedUpdate(Vector2 movement)
    {
        velocity = Mathf.Clamp(movement.magnitude, 0, 1);
        if (isDashing)
            velocity = dashSpeed;
        else
            velocity *= moveSpeed;
    }

    /// <summary>
    /// This moves the character without any implied velocity.
    /// </summary>
    /// <param name="position">The new position of the character in global space.</param>
    public void Teleport(Vector2 position)
    {
        Rigidbody2D.MovePosition(position);
    }
}