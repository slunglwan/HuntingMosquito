using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateMove : EnemyState, ICharacterState
{
    private float runSpeed = 2f;
    private float walkSpeed = 1f;

    public EnemyStateMove(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
        : base(enemyController, animator, agent, target) { }

    public void Enter()
    {
        agent.isStopped = false;
        animator.SetBool(EnemyAniParamMove, true);
        animator.SetFloat(EnemyAniParamSpeedMultiflier, walkSpeed);
        stateRoutine = enemyController.StartCoroutine(StateCoroutine());
    }

    public void Exit()
    {
        animator.SetBool(EnemyAniParamMove, false);
        animator.SetFloat(EnemyAniParamSpeedMultiflier, walkSpeed);
        StopStateCoroutine();
    }

    public void Update() { }

    protected override IEnumerator StateCoroutine()
    {
        while (true)
        {
            var sqrDistance = (enemyController.transform.position - target.position).sqrMagnitude;

            if (enemyController.DetectionTargetInCircle(enemyController.RunDistance))
            {
                animator.SetFloat(EnemyAniParamSpeedMultiflier, runSpeed);
                agent.speed = runSpeed;
                enemyController.IsSafe = false;
                SetFleeDestination();
            }
            else if (enemyController.DetectionTargetInCircle(enemyController.PlayerDetectionDistance))
            {
                animator.SetFloat(EnemyAniParamSpeedMultiflier, walkSpeed);
                agent.speed = walkSpeed;
                enemyController.IsSafe = false;
                SetFleeDestination();
            }

            // Move >> Idle
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                enemyController.SetState(EEnemyState.Idle);
                yield break;
            }

            if (sqrDistance > enemyController.SafeDistance * enemyController.SafeDistance && !enemyController.IsSafe)
            {
                enemyController.IsSafe = true;
                enemyController.SetState(EEnemyState.Idle);
                yield break;
            }

            yield return new WaitForSeconds(coolDownTime);
        }
    }

    private void SetFleeDestination()
    {
        var fleeDir = enemyController.transform.position - target.position;
        fleeDir += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
        fleeDir.Normalize();
        var newPos = enemyController.transform.position + fleeDir.normalized * enemyController.SafeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
