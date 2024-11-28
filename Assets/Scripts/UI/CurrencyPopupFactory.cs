using Zenject;

public class CurrencyPopupFactory : IFactory<CurrencyPopup>
{
    private readonly DiContainer _container;
    private readonly CurrencyPopup _currencyPopupPrefab;

    public CurrencyPopupFactory(DiContainer container, CurrencyPopup currencyPopupPrefab)
    {
        _container = container;
        _currencyPopupPrefab = currencyPopupPrefab;
    }

    public CurrencyPopup Create()
    {
        // Создаем объект с помощью контейнера
        return _container.InstantiatePrefabForComponent<CurrencyPopup>(_currencyPopupPrefab);
    }
}