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

        // Возвращаем поп-ап для дальнейшей настройки высоты
        // return new Popup(popupObject, popupObject.GetComponent<PopupObject>().descriptionText);
    }
    public void HidePopup()
    {
        popupObject.SetActive(true);
        // Destroy(popupObject);
    }
}

// public class Popup
// {
//     private GameObject popupObject;
//     private TextMeshProUGUI descriptionText;

//     public Popup(GameObject popupObject, TextMeshProUGUI descriptionText)
//     {
//         this.popupObject = popupObject;
//         this.descriptionText = descriptionText;
//     }

    // public void AdjustHeightBasedOnContent()
    // {
    //     // Подгоняем высоту поп-апа в зависимости от контента
    //     var contentHeight = descriptionText.preferredHeight;
    //     popupObject.GetComponent<RectTransform>().sizeDelta = new Vector2(popupObject.GetComponent<RectTransform>().sizeDelta.x, contentHeight + 100); // Дополнительные 100 для отступов
    // }
// }
