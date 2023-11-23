using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImageClickHandler : MonoBehaviour, IPointerClickHandler
{
    
    public Image checkImage = null;

    void Start()
    {

        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 1; i < images.Length; i++)
        {
            if (images[i] != null)
            {
                checkImage = images[i];
                break;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(SelectedPhotoManager.Instance.count<0)
        {
            Debug.Log("error");
            return;
        }

        if(SelectedPhotoManager.Instance.count< 3 && checkImage.enabled == false)
        {
            SelectedPhotoManager.Instance.count++;     
        }
        else if (SelectedPhotoManager.Instance.count <= 3 && checkImage.enabled == true)
        {
            Debug.Log("Cancle");
            SelectedPhotoManager.Instance.count--;
        }
        else if (SelectedPhotoManager.Instance.count == 3 && checkImage.enabled==false)
        {
            Debug.Log("You can't select more than 3 images");
            return;
        }
        Debug.Log(gameObject.name + " was clicked!");
        Debug.Log("count: " + SelectedPhotoManager.Instance.count);
        checkImage.enabled = !checkImage.enabled;
    }
}
