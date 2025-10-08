using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Constants;

public class EnemyStateRest : EnemyState, ICharacterState
{
    private float waitTime;
    private float healAmount = 2.5f;

    public EnemyStateRest(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
        : base(enemyController, animator, agent, target) { }

    public void Enter()
    {
        waitTime = 0f;
        agent.isStopped = true;
        animator.SetBool(EnemyAniParamRest, true);
        stateRoutine = enemyController.StartCoroutine(StateCoroutine());
    }

    public void Exit()
    {
        animator.SetBool(EnemyAniParamRest, false);
        StopStateCoroutine();
    }

    public void Update()
    {

    }

    protected override IEnumerator StateCoroutine()
    {
        while (true)
        {
            enemyController.RestoreHp(healAmount);

            if (enemyController.DetectionTargetInCircle(enemyController.PlayerDetectionDistance))
            {
                enemyController.IsSafe = false;
                enemyController.SetState(EEnemyState.Move);
                yield break;
            }

            if (waitTime > enemyController.ActionWaitTime)
            {
                var randomMoveChance = Random.Range(0, 100);
                var randomIdleChance = Random.Range(0, 100);

                if (randomMoveChance < enemyController.MoveChance && enemyController.IsSafe)
                {
                    // 이동 시작
                    var patrolPosition = FindRandomPatrolDestination();

                    // 정찰 위치가 현 위치에서 2Unit 이상 벗어난 경우에만 정찰 시작
                    var realDistance = Vector3.Magnitude(patrolPosition - enemyController.transform.position);
                    var minimumDistance = agent.stoppingDistance + 2f;

                    if (realDistance > minimumDistance)
                    {
                        agent.SetDestination(patrolPosition);
                        enemyController.SetState(EEnemyState.Move);
                        yield break;
                    }
                }

                if (randomIdleChance < enemyController.IdleChance && enemyController.IsSafe)
                {
                    enemyController.SetState(EEnemyState.Idle);
                    yield break;
                }

                waitTime = 0f;
            }
            yield return new WaitForSeconds(coolDownTime);
            waitTime += coolDownTime;
        }
    }

    // 정찰 목적지를 반환하는 함수
    private Vector3 FindRandomPatrolDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * enemyController.PatrolDistance;
        randomDirection += enemyController.transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, enemyController.PatrolDistance, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return enemyController.transform.position;
        }
    }
}
