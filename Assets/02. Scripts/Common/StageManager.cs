using System.Collections;
using TMPro;
using UnityEngine;
using static Constants;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageInfo stageInfo;
    [SerializeField] private MosquitoObjectPool pool;
    [SerializeField] private GameObject player;
    [SerializeField] private float corpseKeepTime = 3f;
    [SerializeField] private float totalSpawnTime = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private TextMeshProUGUI remainedMosquitoCountTxt;
    [SerializeField] private TextMeshProUGUI killCountTxt;
    private int killCount;
    private bool isCleared;

    private void Awake()
    {
        killCount = 0;
        isCleared = false;
        int remainedMosquitoCount = Mathf.Clamp(stageInfo.RequiredMosquitoCount - killCount, 0, stageInfo.RequiredMosquitoCount);
        remainedMosquitoCountTxt.text = $"Required Mosquito Count : {remainedMosquitoCount}";
        killCountTxt.text = $"Kill Count : {killCount}";

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < stageInfo.MosquitoPoolSize; i++)
        {
            SpawnMosquito();

            yield return new WaitForSeconds(totalSpawnTime / stageInfo.MosquitoPoolSize);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 playerForward = player.transform.forward;


        Vector2 mapMin = stageInfo.mapMin;
        Vector2 mapMax = stageInfo.mapMax;

        for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
        {
            // 기본적으로 플레이어 뒤쪽 방향에서 시도
            float angle = Random.Range(105f, 255f); // 플레이어 뒤쪽 ±75도
            float distance = Random.Range(MinSpawnRadius, MaxSpawnRadius);

            // 도넛 범위 좌표 계산
            Vector3 dir = Quaternion.Euler(0f, angle, 0f) * playerForward;
            Vector3 candidate = playerPos + dir * distance;
            candidate.y = 10f; // Raycast 시작 높이 (지면 위)

            // 맵 경계 제한
            candidate.x = Mathf.Clamp(candidate.x, mapMin.x, mapMax.x);
            candidate.z = Mathf.Clamp(candidate.z, mapMin.y, mapMax.y);

            // 장애물 위인지 검사 (지형 충돌 감지)
            if (Physics.Raycast(candidate, Vector3.down, out RaycastHit hit, 20f, groundLayerMask))
            {
                Vector3 groundPoint = hit.point;

                // 주변에 장애물이 너무 가까운지 검사
                bool isBlocked = Physics.CheckSphere(groundPoint, 1.5f, obstacleLayerMask);
                if (!isBlocked)                
                    return groundPoint;                
            }
        }

        return playerPos + playerForward * MinSpawnRadius;
    }

    public void SpawnMosquito()
    {
        var newMosquito = pool.GetObject();
        Vector3 spawnPos = GetValidSpawnPosition(); 
        var poolable = newMosquito.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnSpawned(spawnPos);
        }
        else
        {
            Debug.LogError("잘못된 형식의 게임오브젝트입니다.");
        }
    }

    private void OnEnable()
    {
        EnemyController.OnMosquitoKilled += AddKillCount;
        EnemyController.OnMosquitoKilled += RespawnMosquito;
    }

    private void OnDisable()
    {
        EnemyController.OnMosquitoKilled -= AddKillCount;
        EnemyController.OnMosquitoKilled -= RespawnMosquito;
    }

    private void AddKillCount(GameObject mosquitoObj)
    {
        killCount++;
        int remainedMosquitoCount = Mathf.Clamp(stageInfo.RequiredMosquitoCount - killCount, 0, stageInfo.RequiredMosquitoCount);
        remainedMosquitoCountTxt.text = $"Required Mosquito Count : {remainedMosquitoCount}";
        killCountTxt.text = $"Kill Count : {killCount}";
        if (killCount >= stageInfo.RequiredMosquitoCount && !isCleared)
        {
            isCleared = true ;
            GameManager.Instance.GameClear();
            Debug.Log("Stage Clear!");
        }
    }

    private void RespawnMosquito(GameObject mosquitoObj)
    {
        StartCoroutine(RespawnRoutine(mosquitoObj));
    }

    private IEnumerator RespawnRoutine(GameObject mosquitoObj)
    {
        yield return new WaitForSeconds(corpseKeepTime);
        var poolable = mosquitoObj.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnDespawned();
        }
        else
        {
            Debug.LogError("잘못된 형식의 게임오브젝트입니다.");
            yield break;
        }
        pool.ReturnObject(mosquitoObj);
        SpawnMosquito();
    }
}
