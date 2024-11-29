using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupObject : MonoBehaviour
{
     public TextMeshProUGUI titleText;
     public TextMeshProUGUI descriptionText;
     public Button okButton;

     private void OnEnable() 
     {
          okButton.onClick.AddListener(OnOk);
     }

     void OnOk()
     {
        PopupManager.Instance.HidePopup();
     }
}
