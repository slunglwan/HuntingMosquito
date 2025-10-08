using System.Collections;
using UnityEngine;
using static Constants;

[RequireComponent(typeof(Animator))]
public class Main_MosquitoController : MonoBehaviour
{
    private Animator animator;
    private bool isGoingRight;
    private float minAngle = 45f;
    private float maxAngle = 135f;
    private float maxX = 8f;
    private float maxZ = 4f;
    private float idleChance;
    private float idleRoutineCooldown;
    private const float epsilon = 0.05f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        isGoingRight = Random.Range(0, 2) == 0;
        idleChance = Random.Range(25f, 40f);
        idleRoutineCooldown = Random.Range(2.5f, 4f);
    }

    private void Start()
    {
        StartCoroutine(FlyRoutine());
        StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine()
    {
        while (true)
        {
            // 
            var actionChance = Random.Range(0f, 100f);

            if (idleChance > actionChance)
            {
                animator.SetTrigger(EnemyAniParamIdle);
            }
            yield return new WaitForSeconds(idleRoutineCooldown);
        }
    }
    private IEnumerator FlyRoutine()
    {
        while (true)
        {
            var dir = isGoingRight ? 1f : -1f;

            // 다음 목표 지점 계산
            var x = maxX * -dir;
            var angleY = Random.Range(minAngle, maxAngle);
            var z = (angleY >= 90f) ? Random.Range(0f, maxZ) : Random.Range(-maxZ, 0f);

            // 위치 갱신
            transform.position = new Vector3(x, transform.position.y, z);

            // 회전 (y축만 변경)
            Vector3 currentEuler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, angleY * dir, 0f);

            // 이동이 완료될 때까지 대기
            yield return new WaitUntil(() => 
                    (isGoingRight && transform.position.x >= maxX - epsilon) || 
                    (!isGoingRight && transform.position.x <= -maxX + epsilon));


            isGoingRight = Random.Range(0, 2) == 0;

            // 다음 행동까지 잠시 대기 (자연스러운 이동을 위해)
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        }
    }

    private void OnAnimatorMove()
    {
        transform.position = animator.rootPosition; 
    }
}
