using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateNumber : MonoBehaviour
{

    private TMP_Text textComponent;

    private void Start()
    {
        textComponent = GetComponentInChildren<TMP_Text>();
    }

    private void Update()
    {
        if (textComponent != null && SelectedPhotoManager.Instance != null)
        {
            textComponent.text = SelectedPhotoManager.Instance.count.ToString()+"/3";
        }
    }
}
