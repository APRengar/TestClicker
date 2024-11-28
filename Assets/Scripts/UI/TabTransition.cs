using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TabTransition : MonoBehaviour
{
    [SerializeField] private Image dimmer;

    public void TransitionToTab(System.Action onComplete)
    {
        dimmer.DOFade(0.5f, 0.5f).OnComplete(() =>
        {
            onComplete?.Invoke();
            dimmer.DOFade(0f, 0.5f);
        });
    }
}