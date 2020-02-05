using UnityEngine;

public class MeleeAttackSMB : SceneLinkedSMB<PlayerController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.ResetAttack();
        m_MonoBehaviour.DisMovable();
        m_MonoBehaviour.DisShapable();
    }
    
    public override void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.MeleeAttack();
        
    }

    public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.CheckForMeleeAttack();
        m_MonoBehaviour.EnMovable();
        m_MonoBehaviour.EnShapable();
    }
}