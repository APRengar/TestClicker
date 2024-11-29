using UnityEngine;

public class CurrencyPopup : MonoBehaviour
{
    // Метод для показа эффекта
    public void Show(Vector2 position)
    {
        transform.position = position;
    }
}