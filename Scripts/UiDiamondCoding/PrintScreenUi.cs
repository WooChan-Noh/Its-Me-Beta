
using UnityEngine;

using UnityEngine.UI;
public class PrintScreenUi : MonoBehaviour
{
    public GameObject finalPrint;
    public GameObject selectButton;
    public GameObject selectedPhotoNum;
    public GameObject panel;
    public GameObject infoText;
    public GameObject startScreen;

    // Start is called before the first frame update
    public void StartPrint()
    {
        if (SelectedPhotoManager.Instance.count != 3)
        {
            Debug.Log("�� ������ 3�� �Ʒ��Դϴ�.");
            return;
        }

        selectButton.SetActive(false);
        selectedPhotoNum.SetActive(false);
        panel.SetActive(false);
        infoText.SetActive(false);

        finalPrint.SetActive(true);
    }

    public void BackToMain()
    {
        startScreen.SetActive(true);
        gameObject.SetActive(false);
    }

  
}
