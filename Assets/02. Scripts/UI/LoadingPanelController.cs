using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class LoadingPanelController : MonoBehaviour
{
    [SerializeField] private Image gaugeImage;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        SetProgress(0f);
    }

    public void SetProgress(float progress)
    {
        gaugeImage.fillAmount = progress;
    }

    public void Show(Action onComplete)
    {
        canvasGroup.DOFade(1f, 0.2f).OnComplete(() => onComplete?.Invoke());
        SetProgress(0f);
    }

    public void Hide(Action onComplete)
    {
        SetProgress(1f);
        canvasGroup.DOFade(0f, 0.2f).OnComplete(() => onComplete?.Invoke());
    }
}
