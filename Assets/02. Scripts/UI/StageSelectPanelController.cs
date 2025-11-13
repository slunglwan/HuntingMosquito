using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Constants;

[RequireComponent(typeof(CanvasGroup))]
public class StageSelectPanelController : MonoBehaviour
{
    [SerializeField] private Button stage01Button;
    [SerializeField] private Button stage02Button;
    [SerializeField] private Button stage03Button;
    [SerializeField] private Button closeButton;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        stage01Button.onClick.AddListener(OpenStage01);
        stage02Button.onClick.AddListener(OpenStage02);
        stage03Button.onClick.AddListener(OpenStage03);
        closeButton.onClick.AddListener(() => StartCoroutine(ClosRoutine()));
        _canvasGroup.DOFade(1f, 0.2f);
    }

    private void OnDisable()
    {
        stage01Button?.onClick.RemoveListener(OpenStage01);
        stage02Button?.onClick.RemoveListener(OpenStage02);
        stage03Button?.onClick.RemoveListener(OpenStage03);
        closeButton?.onClick.RemoveAllListeners();
        _canvasGroup.DOFade(0f, 0.2f);
    }

    private IEnumerator ClosRoutine()
    {
        yield return _canvasGroup.DOFade(0f, 0.2f);
        gameObject.SetActive(false);
    }

    private void OpenStage01()
    {
        GameManager.Instance.LoadScene(ESceneName.Stage01);
    }

    private void OpenStage02()
    {
        GameManager.Instance.LoadScene(ESceneName.Stage02);
    }

    private void OpenStage03()
    {
        GameManager.Instance.LoadScene(ESceneName.Stage03);
    }
}
