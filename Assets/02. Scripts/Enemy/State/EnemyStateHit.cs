using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateHit : EnemyState, ICharacterState
{
    public EnemyStateHit(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
        : base(enemyController, animator, agent, target) { }

    public void Enter()
    {
        agent.isStopped = true;
        animator.SetTrigger(EnemyAniParamHit);
        //stateRoutine = StartCoroutine(StateCoroutine());
    }

    public void Exit()
    {
        agent.isStopped = false;
        //StopStateCoroutine();
    }

    public void Update()
    {

    }

    protected override IEnumerator StateCoroutine()
    {
        yield return null;
    }
}
