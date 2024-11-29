using DG.Tweening;
using UnityEngine;

public class ButtonEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem _clickParticles;
    [SerializeField] private Transform _currencyFlyTarget;

    public void PlayClickEffect(Transform button)
    {
        // Анимации кнопки
        button.DOScale(1.1f, 0.1f).OnComplete(() => button.DOScale(1f, 0.1f));

        _clickParticles.Play();
        
        var currencyImage = Instantiate(new GameObject("Currency"), button.position, Quaternion.identity);
        currencyImage.transform.DOMove(_currencyFlyTarget.position, 1f).OnComplete(() => Destroy(currencyImage));
    }
}