using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;

public class LoadPhoto : MonoBehaviour
{

    private string resultImageFolder;
    public Image[] resultImages;


    private void Start()
    {
        resultImageFolder = Application.persistentDataPath + "/ItsmeBeta/ResultImages/";
    }
    public void images()
    {


        string[] files = Directory.GetFiles(resultImageFolder, "*.png");

        for (int i = 0; i < resultImages.Length && i < files.Length; i++)
        {
          
            Texture2D tex = LoadImage(files[i]);
           
            resultImages[i].sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }

    }

    Texture2D LoadImage(string filePath)
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        return tex;
    }



    public void SaveImage()
    {
        int index = 0;
        for (int i = 0; i < 15; i++)
        {

            if (resultImages != null)
            {
                Image checkImage = resultImages[i].transform.Find("CheckImage").GetComponent<Image>();

                if (checkImage != null && checkImage.enabled)
                {
                    SelectedPhotoManager.Instance.selectedPhoto[index] = resultImages[i].GetComponent<Image>().sprite;
                    resultImages[i].transform.Find("CheckImage").GetComponent<Image>().enabled = false;
                    Debug.Log(resultImages[i].name);
                    index++;
                }
            }
        }

    }


}
