using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolSMB : SceneLinkedSMB<EnemyController>
{
    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.ScanForPlayer();
    }
}