using System.Collections.Generic;
using UnityEngine;

public class MosquitoObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject mosquitoPrefab;
    [SerializeField] private Transform parent;
    private Queue<GameObject> pool = new Queue<GameObject>();

    /// <summary>
    /// Object Pool에 새로운 오브젝트 생성 메서드
    /// </summary>
    private void CreateNewObject()
    {
        GameObject newObject = Instantiate(mosquitoPrefab, parent);
        pool.Enqueue(newObject);
    }

    /// <summary>
    /// 오브젝트 풀에 있는 오브젝트 반환 메서드
    /// </summary>
    /// <returns>오브젝트 풀에 있는 오브젝트</returns>
    public GameObject GetObject()
    {
        if (pool.Count == 0) CreateNewObject();

        GameObject dequeObject = pool.Dequeue();
        dequeObject.SetActive(true);
        return dequeObject;
    }

    /// <summary>
    /// 사용한 오브젝트를 오브젝트 풀로 되돌려 주는 메서드
    /// </summary>
    /// <param name="returnObject">반환할 오브젝트</param>
    public void ReturnObject(GameObject returnObject)
    {
        returnObject.SetActive(false);
        pool.Enqueue(returnObject);
    }
}
