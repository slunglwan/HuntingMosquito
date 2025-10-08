using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateDead : EnemyState, ICharacterState
{
    public EnemyStateDead(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
        : base(enemyController, animator, agent, target) { }

    public void Enter()
    {
        animator.SetTrigger(EnemyAniParamDead);
        agent.updatePosition = false;
        agent.updateRotation = false;
        enemyController.GetComponent<Collider>().enabled = false;
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }

    protected override IEnumerator StateCoroutine()
    {
        yield return null;
    }
}
