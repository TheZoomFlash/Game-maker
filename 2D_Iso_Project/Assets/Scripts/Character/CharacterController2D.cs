using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(Collider2D))]
public class CharacterController2D : MonoBehaviour
{
    [Header("move settings")]
    public float moveSpeed = 3f;

    public Rigidbody2D Rigidbody2D { get; protected set; }
    public Vector2 position { get { return Rigidbody2D.position; } }
    public float velocity { get; protected set; }
    public Vector2 faceDir { get; protected set; }

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

    /// <summary>
    /// This moves a rigidbody and so should only be called from FixedUpdate or other Physics messages.
    /// </summary>
    /// <param name="movement">The amount moved in global coordinates relative to the rigidbody2D's position.</param>
    public void Move(Vector2 movement)
    {
        if (!isMovable)
            return;

        velocity = movement.magnitude;

        if (!Mathf.Approximately(movement.x, 0.0f) || !Mathf.Approximately(movement.y, 0.0f))
        {
            DirUpdate(movement);
            Vector2 newPos = Rigidbody2D.position + faceDir * moveSpeed * Time.deltaTime;
            Rigidbody2D.MovePosition(newPos);
        }
    }

    void DirUpdate(Vector2 dir)
    {
        faceDir = dir.normalized;
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