using System.Collections.Generic;
using UnityEngine;

public class Skill_Punch : MonoBehaviour, ISkillObservable<GameObject>
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float attackRadius;
    private readonly List<ISkillObserver<GameObject>> observers = new List<ISkillObserver<GameObject>>();
    private static readonly Collider[] hitCache = new Collider[50];

    public void PerformAttack()
    {
        // 지정 반경 내의 모든 대상 탐색
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, hitCache, targetLayerMask);

        for (int i = 0; i < hitCount; i++)
        {
            Notify(hitCache[i].gameObject);
        }
        
        if(hitCount > 0) 
            Debug.Log($"공격으로 {hitCount}마리 적 감지됨");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in observers)
        {
            observer.OnNext(value);
        }
    }

    public void Subscribe(ISkillObserver<GameObject> observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void UnSubscribe(ISkillObserver<GameObject> observer)
    {
        observers.Remove(observer);
    }
}
