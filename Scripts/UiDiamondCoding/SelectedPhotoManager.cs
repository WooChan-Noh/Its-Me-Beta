using UnityEngine.UI;
using UnityEngine;

public class SelectedPhotoManager : MonoBehaviour
{
    public static SelectedPhotoManager Instance;

    public Sprite[] selectedPhoto = new Sprite[3];
    [HideInInspector] public int count = 0;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}

