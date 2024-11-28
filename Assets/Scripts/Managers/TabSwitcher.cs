using DG.Tweening;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup clickerTab;
    [SerializeField] private CanvasGroup factsTab;
    [SerializeField] private CanvasGroup fadeEffect;

    public void SwitchToClicker()
    {
        SwitchTabs(clickerTab, factsTab).Forget();
    }

    public void SwitchToFacts()
    {
        SwitchTabs(factsTab, clickerTab).Forget();
    }

    private async UniTaskVoid SwitchTabs(CanvasGroup show, CanvasGroup hide)
    {
        await fadeEffect.DOFade(1, 0.3f).AsyncWaitForCompletion();
        hide.gameObject.SetActive(false);
        show.gameObject.SetActive(true);
        await fadeEffect.DOFade(0, 0.3f).AsyncWaitForCompletion();
    }
}