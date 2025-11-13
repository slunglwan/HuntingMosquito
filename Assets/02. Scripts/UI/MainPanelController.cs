using UnityEngine;
using UnityEngine.UI;
using static Constants;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private Button stageSelectButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject stageSelectPanel;

    private void OnEnable()
    {
        stageSelectPanel.SetActive(false);
        stageSelectButton.onClick.AddListener(OpenStageSelectPanel);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        stageSelectPanel.SetActive(false);
        stageSelectButton?.onClick.RemoveListener(OpenStageSelectPanel);
        quitButton?.onClick.RemoveListener(QuitGame);
    }

    private void OpenStageSelectPanel()
    {
        stageSelectPanel.SetActive(true);
    }

    private void QuitGame()
    {
        GameManager.Instance.Quit();
    }
}
