using TMPro;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [SerializeField] private GameObject popupObject;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowPopup(string title, string description)
    {
        popupObject.GetComponent<PopupObject>().titleText.text = title;
        popupObject.GetComponent<PopupObject>().descriptionText.text = description;
        popupObject.SetActive(true);

    }
    public void HidePopup()
    {
        popupObject.SetActive(true);
    }
}
