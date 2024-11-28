using Zenject;
using UnityEngine;

public class CurrencyPopupPool : MonoMemoryPool<Vector2, CurrencyPopup>
{
    // Используем фабрику, чтобы создать объекты при необходимости
    private readonly IFactory<CurrencyPopup> _popupFactory;

    // Инжектируем фабрику через конструктор
    public CurrencyPopupPool(IFactory<CurrencyPopup> popupFactory)
    {
        _popupFactory = popupFactory;
    }

    protected override void Reinitialize(Vector2 position, CurrencyPopup item)
    {
        item.Show(position);
    }
}