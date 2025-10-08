using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Constants;

public class GameManager : Singleton<GameManager>
{
    //[SerializeField] private GameObject playerPrefab;

    public EGameState GameState { get; private set; }

    //private GameObject player;
    private Canvas canvas;
    private bool isCursorLock;


    public void SetCursorLock()
    {
        Cursor.visible = isCursorLock;
        Cursor.lockState = isCursorLock ? CursorLockMode.None : CursorLockMode.Locked;
        isCursorLock = !isCursorLock;
    }

    public void SetGameState(EGameState gameState)
    {
        GameState = gameState;
    }

    public void LoadScene(ESceneName sceneName)
    {
        StartCoroutine(LoadSceneASyncCoroutine(sceneName));
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void GameClear()
    {
        // 클리어 화면 띄우기
        var clearPanelPrefab = Resources.Load<GameObject>("[Panel] StageClear");
        var clearPanelObject = Instantiate(clearPanelPrefab, canvas.transform);
        var clearPanelController = clearPanelObject.GetComponent<StageClearPanelController>();

        clearPanelController.Show();
    }

    private IEnumerator LoadSceneASyncCoroutine(ESceneName sceneName)
    {
        // 로딩 화면 띄우기
        var loadingPanelPrefab = Resources.Load<GameObject>("[Panel] Loading");
        var loadingPanelObject = Instantiate(loadingPanelPrefab, canvas.transform);
        var loadingPanelController = loadingPanelObject.GetComponent<LoadingPanelController>();


        bool showDone = false;
        loadingPanelController.Show(() => showDone = true);
        yield return new WaitUntil(() => showDone);

        // 씬 로드 진행
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName.ToString());
        asyncOperation.allowSceneActivation = false;

        while (asyncOperation.progress < 0.9f)
        {
            loadingPanelController.SetProgress(asyncOperation.progress);
            yield return null;
        }
        loadingPanelController.SetProgress(1f);
        asyncOperation.allowSceneActivation = true;

        bool hideDone = false;
        loadingPanelController.Hide(() => hideDone = true);
        yield return new WaitUntil(() => hideDone);

        Destroy(loadingPanelObject);
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        canvas = GetCanvas();

        //switch (scene.name)
        //{
        //    case "Main":
        //        break;
        //    case "Stage01":
        //    case "Stage02":
        //        var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

        //        if (player)
        //        {
        //            player.SetActive(true);
        //            player.transform.position = spawnPoint.position;
        //            player.transform.rotation = spawnPoint.rotation;
        //        }
        //        else
        //        {
        //            player = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        //            DontDestroyOnLoad(player);
        //        }
        //        break;
        //}

        //GameState = EGameState.Play;
    }

    private Canvas GetCanvas()
    {
        var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
        Canvas result = null;

        if (!canvasObject)
        {
            canvasObject = new GameObject("Canvas");
            canvasObject.AddComponent<Canvas>();
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            result = canvasObject.GetComponent<Canvas>();
            result.renderMode = RenderMode.ScreenSpaceOverlay;
            result.tag = "Canves";
        }
        else
        {
            result = canvasObject.GetComponent<Canvas>();
        }

        return result;
    }

    protected override void OnSceneUnloaded(Scene scene)
    {
        //player.SetActive(false);
    }
}
