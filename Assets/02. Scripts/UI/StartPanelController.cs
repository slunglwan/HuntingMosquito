using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class StartPanelController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timer;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 1f;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        _canvasGroup.DOFade(0f, 5f);
        var elapsed = 0f;
        while (elapsed < 5f)
        {
            timer.text = $"{5f - elapsed}";
            elapsed += 1f;
            yield return new WaitForSeconds(1f);
        }
        GameManager.Instance.SetGameState(Constants.EGameState.Play);
    }
}
