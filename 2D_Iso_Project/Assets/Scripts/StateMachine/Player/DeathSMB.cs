using UnityEngine;


public class DeathSMB : SceneLinkedSMB<PlayerController>
{
    public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.DisMovable();
        m_MonoBehaviour.DisShapable();
    }

    public override void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        m_MonoBehaviour.RealDie();
    }
}