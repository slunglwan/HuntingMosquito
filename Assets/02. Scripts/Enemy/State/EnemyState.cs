using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyState
{
    protected EnemyController _enemyController;
    protected Animator animator;
    protected NavMeshAgent agent;
    protected Transform target;
    protected float _coolDownTime = 0.25f;
    protected Coroutine stateRoutine;

    public EnemyState(EnemyController enemyController, Animator animator, NavMeshAgent agent, Transform target)
    {
        this._enemyController = enemyController;
        this.animator = animator;
        this.agent = agent;
        this.target = target;
    }

    protected abstract IEnumerator StateCoroutine();

    protected void StopStateCoroutine()
    {
        if (stateRoutine != null)
        {
            _enemyController.StopCoroutine(stateRoutine);
            stateRoutine = null;
        }
    }
}
