# HuntingMosquito

이 프로젝트는 Unity 기반의 AI 중심 액션 게임 시스템으로,
플레이어가 벌레를 사냥하는 과정에서 AI 이동, 상태머신(FSM), 이벤트 시스템, 오브젝트 풀링, UI 전환 등을 종합적으로 구현한 프로젝트입니다.

개발자는 상태 기반 AI, 풀링, DOTween UI, 옵저버 패턴 기반 스킬 이벤트 시스템,
그리고 NavMesh 경로 탐색 + 안전 스폰 시스템을 직접 설계 및 구현했습니다.


기술적 특징 요약
| 기술 요소                                 | 설명                                         |
| ------------------------------------- | ------------------------------------------ |
| **Finite State Machine (FSM)**        | Idle / Move / Rest / Hit / Dead 상태 기반 적 AI |
| **NavMeshAgent AI**                   | 플레이어와의 거리 기반 안전·도망·추격 행동 처리                |
| **ISkillObservable / ISkillObserver** | 스킬 이벤트를 Observer Pattern으로 분리하여 적 결합도 제거   |
| **Object Pooling System**             | 몬스터 재활용을 통한 성능 최적화                         |
| **Safe Spawn System**                 | NavMesh 내부에서 유효한 스폰 위치만 선택                 |
| **Singleton GameManager**             | 글로벌 씬 관리 및 페이드 전환                          |
| **Coroutine State Isolation**         | 상태별 코루틴 시작·정지 체계를 정밀 제어                    |
