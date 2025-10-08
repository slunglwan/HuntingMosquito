using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

[RequireComponent(typeof(CanvasGroup))]
public class StageClearPanelController : MonoBehaviour
{
    [SerializeField] Button goToMainButton;
    [SerializeField] Button quitButton;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
    }

    private void OnEnable()
    {
        goToMainButton.onClick.AddListener(GoToMainScene);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void OnDisable()
    {
        goToMainButton.onClick?.RemoveListener(GoToMainScene);
        quitButton?.onClick?.RemoveListener(QuitGame);
    }

    private void GoToMainScene()
    {
        Hide();
        GameManager.Instance.LoadScene(ESceneName.Main);
    }

    private void QuitGame()
    {
        Hide();
        GameManager.Instance.Quit();
    }

    public void Show(Action onComplete = null)
    {
        canvasGroup.DOFade(1f, 0.2f).OnComplete(() => onComplete?.Invoke());
    }

    public void Hide(Action onComplete = null)
    {
        canvasGroup.DOFade(0f, 0.2f).OnComplete(() => onComplete?.Invoke());
    }
}
