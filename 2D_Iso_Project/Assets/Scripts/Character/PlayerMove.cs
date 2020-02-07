using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : CharacterMove
{
    [Space]
    [Header("dash Setting")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.2f;
    public float dashInvertal = 1f;
    Ability dashAB;

    public GhostTrail ghostTrail;

    protected override void OnAwake()
    {
        base.OnAwake();
        dashAB = gameObject.AddComponent<Ability>() as Ability;
        dashAB.InitSetParams(dashDuration, dashInvertal);

        //ghostTrail = GetComponentInChildren<GhostTrail>();
    }

    protected override void SpeedUpdate(Vector2 movement)
    {
        base.SpeedUpdate(movement);
        if (dashAB.IsUsing)
            Velocity = dashSpeed;
    }

    public void Dash()
    {
        if (!isMovable || !dashAB.Usable)
            return;

        dashAB.Use();
        ghostTrail.ShowGhost();
    }

}
