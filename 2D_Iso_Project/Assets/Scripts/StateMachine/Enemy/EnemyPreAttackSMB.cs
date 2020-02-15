using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPreAttackSMB : SceneLinkedSMB<EnemyController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.ShowAttention(true);
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.ShowAttention(false);
    }
}