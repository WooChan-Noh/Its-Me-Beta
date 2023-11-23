using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading.Tasks;

public class FinalPhoto : MonoBehaviour
{
    public GameObject finalImage0;
    public GameObject finalImage1;
    public GameObject finalImage2;
    public GameObject mainButton;
    public GameObject printButton;
    public GameObject printText;
    public GameObject PrintLoadText;
    public GameObject areYouPrint;
   
    public GameObject startScreen;
    public GameObject printScreen;
    public PrintScreenUi printScreenUi;
    Texture2D texture0;
    Texture2D texture1;
    Texture2D texture2;
    public void FinalImage()
    {
        finalImage0.GetComponent<Image>().sprite = SelectedPhotoManager.Instance.selectedPhoto[0];
        finalImage1.GetComponent<Image>().sprite = SelectedPhotoManager.Instance.selectedPhoto[1];
        finalImage2.GetComponent<Image>().sprite = SelectedPhotoManager.Instance.selectedPhoto[2];

        texture0 = finalImage0.GetComponent<Image>().sprite.texture;
        texture1 = finalImage1.GetComponent<Image>().sprite.texture;
        texture2 = finalImage2.GetComponent<Image>().sprite.texture;

    }
  
    public async void OnPrintButtonClick()
    {
        mainButton.SetActive(false);
        printButton.SetActive(false);
        areYouPrint.SetActive(false);
        printText.SetActive(true);
        PrintLoadText.SetActive(true);
       


        //PhotoPrinter.Instance.StartPrinter(texture0);
        //PhotoPrinter.Instance.StartPrinter(texture1);
        //PhotoPrinter.Instance.StartPrinter(texture2);

        await Task.Delay(10000);


       SelectedPhotoManager.Instance.count = 0;
       SelectedPhotoManager.Instance.selectedPhoto[0] = null;
       SelectedPhotoManager.Instance.selectedPhoto[1] = null;
       SelectedPhotoManager.Instance.selectedPhoto[2] = null;


       startScreen.SetActive(true);
       printScreen.SetActive(false);

       mainButton.SetActive(true);
       printButton.SetActive(true);
       areYouPrint.SetActive(true);
       printText.SetActive(false);
       PrintLoadText.SetActive(false);
    

       printScreenUi.selectButton.SetActive(true);
       printScreenUi.selectedPhotoNum.SetActive(true);
       printScreenUi.panel.SetActive(true);
       printScreenUi.infoText.SetActive(true);
       printScreenUi.finalPrint.SetActive(false);
    }
    

}



