using DG.Tweening;
using UnityEngine;

public class ButtonEffects : MonoBehaviour
{
    [SerializeField] private ParticleSystem clickParticles;
    [SerializeField] private Transform currencyFlyTarget;

    public void PlayClickEffect(Transform button)
    {
        // Анимация кнопки
        button.DOScale(1.1f, 0.1f).OnComplete(() => button.DOScale(1f, 0.1f));

        // Частицы
        clickParticles.Play();

        // Анимация валюты
        var currencyImage = Instantiate(new GameObject("Currency"), button.position, Quaternion.identity);
        currencyImage.transform.DOMove(currencyFlyTarget.position, 1f).OnComplete(() => Destroy(currencyImage));
    }
}