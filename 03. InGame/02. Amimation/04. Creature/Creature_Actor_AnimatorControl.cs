using UnityEngine;
using System.Collections;

public class Creature_Actor_AnimatorControl : StateMachineBehaviour
{
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        //Debug.Log("OnStateEnter " + animator.name);
        Creature_AniDirector actor = animator.gameObject.GetComponent<Creature_AniDirector>();
        if (actor != null)
        {
            actor.AniOnStateEnter(animator, stateInfo, layerIndex);
        }
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Debug.Log("OnStateUpdate " + animator.name);
        Creature_AniDirector actor = animator.gameObject.GetComponent<Creature_AniDirector>();
        if (actor != null)
        {
            actor.AniOnStateUpdate(animator, stateInfo, layerIndex);
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
    {
        //Debug.Log("OnStateExit "+ animator.name);
        Creature_AniDirector actor = animator.gameObject.GetComponent<Creature_AniDirector>();
        if (actor != null)
        {
            actor.AniOnStateExit(animator, stateInfo, layerIndex);
        }
    }
	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
