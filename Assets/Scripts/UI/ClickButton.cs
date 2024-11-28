using UnityEngine;
using Zenject;

public class ClickButton : MonoBehaviour
{
    // private GameManager _gameManager;
    private GameManager _gameManager;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public void OnButtonClick()
    {
        if (_gameManager == null)
        {
            Debug.LogError("GameManager не назначен!");
            return;
        }
        // Vector3 clickPosition = Input.mousePosition; // Текущая позиция клика
        Vector3 clickPositionScreenSpace = Input.mousePosition;
        Vector3 clickPositionWorldSpace = Camera.main.ScreenToWorldPoint(clickPositionScreenSpace);

        // _gameManager.PerformClick(clickPosition);
        _gameManager.PerformClick(clickPositionWorldSpace);
    }
}