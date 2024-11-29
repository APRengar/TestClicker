using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup _clickerTab;
    [SerializeField] private CanvasGroup _factsTab;
    [SerializeField] private CanvasGroup _fadeEffect;

    public void SwitchToClicker()
    {
        SwitchTabs(_clickerTab, _factsTab).Forget();
    }

    public void SwitchToFacts()
    {
        SwitchTabs(_factsTab, _clickerTab).Forget();
    }

    private async UniTaskVoid SwitchTabs(CanvasGroup show, CanvasGroup hide)
    {
        await _fadeEffect.DOFade(1, 0.3f).AsyncWaitForCompletion();
        hide.gameObject.SetActive(false);
        show.gameObject.SetActive(true);
        await _fadeEffect.DOFade(0, 0.3f).AsyncWaitForCompletion();
    }
}