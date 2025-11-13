using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSweater : MonoBehaviour, ISkillObservable<GameObject>
{
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float attackRadius;
    [SerializeField] private GameObject attackEffect;
    [SerializeField] private float rotationSpeed = 180f;
    private readonly List<ISkillObserver<GameObject>> _observers = new List<ISkillObserver<GameObject>>();
    private static readonly Collider[] _hitCache = new Collider[50];
    private bool _isGet = false;

    private IEnumerator Start()
    {
        while (true)
        {
            if (_isGet) yield break;

            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isGet) return;

        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                _isGet = true;
                player.EquipWeapon(gameObject);
                transform.localRotation = Quaternion.Euler(new Vector3(-90f, 0f, 0f));
                transform.localPosition = Vector3.zero;
            }
        }
    }


    public IEnumerator PerformAttack()
    {
        // 지정 반경 내의 모든 대상 탐색
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, attackRadius, _hitCache, targetLayerMask);
        attackEffect.gameObject.SetActive(true);
        for (int i = 0; i < hitCount; i++)
        {
            Notify(_hitCache[i].gameObject);
        }
        yield return new WaitForSeconds(0.5f);
        attackEffect.gameObject.SetActive(false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }

    public void Subscribe(ISkillObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnSubscribe(ISkillObserver<GameObject> observer)
    {
        _observers.Remove(observer);
    }
}
