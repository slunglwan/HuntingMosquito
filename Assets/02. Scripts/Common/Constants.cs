using UnityEngine;

public static class Constants
{
    public const float Gravity = -9.81f;

    /// <summary>
    /// 스테이지 정보
    /// </summary>
    public enum EStageState
    {
        Normal, Boss
    }

    /// <summary>
    /// 신 이름
    /// </summary>
    public enum ESceneName
    {
        Main, Stage01, Stage02, Stage03, Stage04, Stage05, Stage06, Stage07, Stage08, Stage09, Stage10
    }

    /// <summary>
    /// 게임 상태
    /// </summary>
    public enum EGameState
    {
        None, Play, Pause, Ready, Clear
    }

    /// <summary>
    /// 모기들의 상태 정보
    /// </summary>
    public enum EEnemyState
    {
        None, Idle, Move, Rest, Hit, Dead, Attack
    }

    /// <summary>
    /// 플레이어의 상태 정보
    /// </summary>
    public enum EPlayerState
    {
        None, Idle, Move, Attack, Hit, Dead, Victory
    }

    /// <summary>
    /// Enemy 애니메이터 파라미터
    /// </summary>
    public static readonly int EnemyAniParamIdle = Animator.StringToHash("idle");
    public static readonly int EnemyAniParamMove = Animator.StringToHash("move");
    public static readonly int EnemyAniParamRest = Animator.StringToHash("rest");
    public static readonly int EnemyAniParamHit = Animator.StringToHash("hit");
    public static readonly int EnemyAniParamDead = Animator.StringToHash("dead");
    public static readonly int EnemyAniParamAttack = Animator.StringToHash("attack");
    public static readonly int EnemyAniParamSpeedMultiflier = Animator.StringToHash("speed_multiflier");

    /// <summary>
    /// Player 애니메이터 파라미터
    /// </summary>
    public static readonly int PlayerAniParamIdle = Animator.StringToHash("idle");
    public static readonly int PlayerAniParamMove = Animator.StringToHash("move");
    public static readonly int PlayerAniParamPunch = Animator.StringToHash("punch");
    public static readonly int PlayerAniParamSwing = Animator.StringToHash("swing");
    public static readonly int PlayerAniParamHit = Animator.StringToHash("hit");
    public static readonly int PlayerAniParamDead = Animator.StringToHash("dead");
    public static readonly int PlayerAniParamMoveSpeed = Animator.StringToHash("move_speed");
    public static readonly int PlayerAniParamVictory = Animator.StringToHash("victory");
}
