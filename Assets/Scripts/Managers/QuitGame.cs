
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    private Button _button;

    private void Awake() 
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(QuitGameInitiate);
    }

    public void QuitGameInitiate()
    {
        Application.Quit();
    }
}
