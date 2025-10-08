using UnityEngine;

public class PlayerSmbAttack : StateMachineBehaviour
{
    private PlayerController playerController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerController == null)
            playerController = animator.GetComponent<PlayerController>();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerController.SetState(Constants.EPlayerState.Idle);
    }
}
