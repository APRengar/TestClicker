using System.Collections;
using UnityEngine;

public class CurrencyPopup : MonoBehaviour
{
    // [SerializeField] private float moveSpeed = 2f;
    // [SerializeField] private float transitionLenght = 1f;
    // [SerializeField] private GameObject particles;

    // Метод для показа эффекта
    public void Show(Vector2 position)
    {
        transform.position = position;
        // Можно добавить другие эффекты, например, партиклы
        // Instantiate(particles, transform.position, Quaternion.identity);
    }


}