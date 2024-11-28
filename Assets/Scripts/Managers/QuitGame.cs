
using UnityEngine;
using UnityEngine.UI;

public class QuitGame : MonoBehaviour
{
    private Button button;

    private void Awake() 
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(QuitGameInitiate);
    }

    public void QuitGameInitiate()
    {
        Application.Quit();
    }
}
