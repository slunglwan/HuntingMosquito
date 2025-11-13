using System.Collections;
using TMPro;
using UnityEngine;
using static Constants;

public class StageManager : MonoBehaviour
{
    [SerializeField] private StageInfo _stageInfo;
    [SerializeField] private MosquitoObjectPool pool;
    [SerializeField] private GameObject player;
    [SerializeField] private float corpseKeepTime = 3f;
    [SerializeField] private float totalSpawnTime = 1f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private LayerMask obstacleLayerMask;
    [SerializeField] private TextMeshProUGUI remainedMosquitoCountTxt;
    [SerializeField] private TextMeshProUGUI killCountTxt;
    private int _killCount;
    private bool _isCleared;

    public static System.Action OnGameClear;

    private void Awake()
    {
        _killCount = 0;
        _isCleared = false;
        int remainedMosquitoCount = Mathf.Clamp(_stageInfo.RequiredMosquitoCount - _killCount, 0, _stageInfo.RequiredMosquitoCount);
        remainedMosquitoCountTxt.text = $"Required Mosquito Count : {remainedMosquitoCount}";
        killCountTxt.text = $"Kill Count : {_killCount}";

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < _stageInfo.MosquitoPoolSize; i++)
        {
            SpawnMosquito();

            yield return new WaitForSeconds(totalSpawnTime / _stageInfo.MosquitoPoolSize);
        }
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 playerPos = player.transform.position;
        Vector3 playerForward = player.transform.forward;


        Vector2 mapMin = _stageInfo.mapMin;
        Vector2 mapMax = _stageInfo.mapMax;
        Vector3 candidate = Vector3.zero;

        for (int attempt = 0; attempt < _stageInfo.MaxSpawnAttempts; attempt++)
        {
            if (GameManager.Instance.GameState == EGameState.Play)
            {
                // 기본적으로 플레이어 뒤쪽 방향에서 시도
                float angle = Random.Range(105f, 255f); // 플레이어 뒤쪽 ±75도
                float distance = Random.Range(_stageInfo.MinSpawnRadius, _stageInfo.MaxSpawnRadius);

                // 도넛 범위 좌표 계산
                Vector3 dir = Quaternion.Euler(0f, angle, 0f) * playerForward;
                candidate = playerPos + dir * distance;
                candidate.y = 10f; // Raycast 시작 높이 (지면 위)

                // 맵 경계 제한
                candidate.x = Mathf.Clamp(candidate.x, mapMin.x, mapMax.x);
                candidate.z = Mathf.Clamp(candidate.z, mapMin.y, mapMax.y);
            }
            else if (GameManager.Instance.GameState == EGameState.Ready)
            {
                var randomDir = Random.insideUnitCircle.normalized;
                var randomDistance = Random.Range(_stageInfo.MinSpawnRadius, _stageInfo.MaxSpawnRadius);

                candidate = playerPos + new Vector3(randomDir.x, 0f, randomDir.y) * randomDistance;

            }

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

        return playerPos + playerForward * _stageInfo.MinSpawnRadius;
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
        _killCount++;
        int remainedMosquitoCount = Mathf.Clamp(_stageInfo.RequiredMosquitoCount - _killCount, 0, _stageInfo.RequiredMosquitoCount);
        remainedMosquitoCountTxt.text = $"Required Mosquito Count : {remainedMosquitoCount}";
        killCountTxt.text = $"Kill Count : {_killCount}";
        if (_killCount >= _stageInfo.RequiredMosquitoCount && !_isCleared)
        {
            _isCleared = true ;
            OnGameClear?.Invoke();
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
