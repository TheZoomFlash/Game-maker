using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyAttackSMB : SceneLinkedSMB<EnemyBehaviour>
{
    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.SetHorizontalSpeed(0);
    }
}