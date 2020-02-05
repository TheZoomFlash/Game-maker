using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHitSMB : SceneLinkedSMB<EnemyController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.DisMovable();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.EnMovable();
    }
}