using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateMove : EnemyState, ICharacterState
{
    private float runSpeed = 3f;
    private float walkSpeed = 1.5f;

    public EnemyStateMove(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
        : base(enemyController, animator, agent, target) { }

    public void Enter()
    {
        agent.isStopped = false;
        animator.SetBool(EnemyAniParamMove, true);
        animator.SetFloat(EnemyAniParamSpeedMultiflier, walkSpeed);
        stateRoutine = _enemyController.StartCoroutine(StateCoroutine());
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
            var sqrDistance = (_enemyController.transform.position - target.position).sqrMagnitude;

            if (_enemyController.DetectionTargetInCircle(_enemyController.RunDistance))
            {
                animator.SetFloat(EnemyAniParamSpeedMultiflier, runSpeed);
                agent.speed = runSpeed;
                _enemyController.IsSafe = false;
                SetFleeDestination();
            }
            else if (_enemyController.DetectionTargetInCircle(_enemyController.PlayerDetectionDistance))
            {
                animator.SetFloat(EnemyAniParamSpeedMultiflier, walkSpeed);
                agent.speed = walkSpeed;
                _enemyController.IsSafe = false;
                SetFleeDestination();
            }

            // Move >> Idle
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && _enemyController.IsSafe)
            {
                _enemyController.SetState(EEnemyState.Idle);
                yield break;
            }

            if (sqrDistance > _enemyController.SafeDistance * _enemyController.SafeDistance && !_enemyController.IsSafe)
            {
                _enemyController.IsSafe = true;
                _enemyController.SetState(EEnemyState.Idle);
                yield break;
            }

            yield return new WaitForSeconds(_coolDownTime);
        }
    }

    private void SetFleeDestination()
    {
        var fleeDir = _enemyController.transform.position - target.position;
        fleeDir += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));
        fleeDir.Normalize();
        var newPos = _enemyController.transform.position + fleeDir.normalized * _enemyController.SafeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 3f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
