using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        gameStartButton.onClick.AddListener(LoadStage01);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        gameStartButton?.onClick.RemoveListener(LoadStage01);
        quitButton?.onClick.RemoveListener(QuitGame);
    }

    private void LoadStage01()
    {
        GameManager.Instance.LoadScene(ESceneName.Stage01);
    }

    private void QuitGame()
    {
        GameManager.Instance.Quit();
    }
}
