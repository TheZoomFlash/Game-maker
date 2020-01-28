using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSMB : SceneLinkedSMB<PlayerController>
{
    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(m_MonoBehaviour)
            m_MonoBehaviour.CheckForMeleeAttack();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}