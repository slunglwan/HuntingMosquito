using UnityEngine;
using static Constants;

[CreateAssetMenu(fileName = "StageInfo", menuName = "Scriptable Objects/StageInfo")]
public class StageInfo : ScriptableObject
{
    [Header("클리어 관련 정보")]
    public int RequiredMosquitoCount;

    [Header("스테이지 상태")]
    public EStageState StageState;

    [Header("맵 설정")]
    public int MosquitoPoolSize;
    public Vector2 mapMin;
    public Vector2 mapMax;
}
