using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAttackSMB : SceneLinkedSMB<EnemyController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.ResetAttack();
    }

    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.MeleeAttack();
    }
}