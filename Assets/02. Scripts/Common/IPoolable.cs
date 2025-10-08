using UnityEngine;

public interface IPoolable
{
    void OnSpawned(Vector3 spawnPos);
    void OnDespawned();
}
