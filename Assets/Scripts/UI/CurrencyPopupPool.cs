using Zenject;
using UnityEngine;

public class CurrencyPopupPool : MonoMemoryPool<Vector2, CurrencyPopup>
{
    private readonly IFactory<CurrencyPopup> _popupFactory;

    public CurrencyPopupPool(IFactory<CurrencyPopup> popupFactory)
    {
        _popupFactory = popupFactory;
    }

    protected override void Reinitialize(Vector2 position, CurrencyPopup item)
    {
        item.Show(position);
    }
}