using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using System.Threading.Tasks;
using OpenCvSharp.Demo;


public class StableDiffusionReactor : StableDiffusionGenerator
{
    # region DefaultPrarameter
    [ReadOnly]
    public string guid = "";
    [SerializeField]
    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }
    [SerializeField]
    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }
    [HideInInspector]
    public int selectedSampler = 16;//DPM++ SDE Karras��
    [HideInInspector]
    public int selectedModel = 0;//Xl��

    public int width = 512;
    public int height = 512;
    public int steps = 50;
    public float cfgScale = 7;
    public long seed = -1;
    public long generatedSeed = -1;

    public bool generating = false;//�ߺ� ���� ���� 
    #endregion

    #region API Parameter
    string base64Format = "data:image/png;base64,";
    string fileFormat = ".png";
    #endregion

    int printPhotoNum = 15;//����ڰ� �� �� �ִ� �� ���� �� 

    public string sourceImageFolderPath = "";//������� �� ������ ����� "����" ���
    string sourceImageName = "";//������� �� ���� �̸�(�뵵 : Reactor�� ���) - Generate()���� ������

    public string targetImageFolderPath = "";//Ÿ�� �̹����� ���� ����� "����" ���
    string targetImagePath = "";//Ÿ�� �̹��� ���� ���(�뵵 : Reactor�� ���)
    string[] targetImageFolderList;//Ÿ�� �̹��� �̸����� ����� �迭
    public static int targetImagesFolderLength=0;//Ÿ�� �̹��� �� ���� => GenerateAsync()���� targetImageName ������ ���ȴ�.

    float waitHTTPTime = 6f;//�� ���� ��� �ð� (������ ���� �ð����� ������ �ʰ�) -3090�� ������

    public string resultImageFolderPath = "";//�����ͷ� �ռ��� ������ ����� "����" ���
    private string reactorFilePath = "";//�����ͷ� �ռ��� "����"�� ���� ��� - ���� �� string�� ��

 
    private void Start()
    {
        selectedSampler = 16;
        sourceImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/Photo/";
        targetImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/TargetImages/";
        resultImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/ResultImages/";

        // Ÿ���̹����� ������ �������� ���ɿ� ������ ��ĥ ���̴�
        targetImageFolderList = Directory.GetFiles(targetImageFolderPath);//Ÿ�� �̹����� �޾ƿ´�. �����̵� �������
        targetImagesFolderLength=targetImageFolderList.Length;
    }
    void SetupReactorImageName(int num)
    {
        try
        {
            reactorFilePath = Path.Combine(resultImageFolderPath, "Reactor" + num + fileFormat);//�����ͷ� �ռ��Ǿ� ����� ���� ���
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }
    string[] loadTargetImages()//��ü Ÿ�� �̹��� �߿��� �������� 15���� ������ ������
    {
        int[] usedNumbers = new int[targetImagesFolderLength];
        int randomIndex=0;

        string[] targetImages = new string[printPhotoNum];//�ռ��� 15���� �̹���
 
        for (int i = 0; i < printPhotoNum; i++)
        {
            do
            {
              randomIndex = UnityEngine.Random.Range(0, targetImagesFolderLength);
            } while (usedNumbers[randomIndex] == 1);
            usedNumbers[randomIndex] = 1;
            targetImages[i] = Path.GetFileName(targetImageFolderList[randomIndex]);
          
        }
        Debug.Log("Ÿ�� �̹��� �ε� �Ϸ�");
        return targetImages;

    }
    public void Generate()
    {
        sourceImageName = sourceImageFolderPath+FaceDetectorScene.photoName + fileFormat;//�� ���� �̸� ����(�뵵 : Reactor�� ���)
        if (!generating && !string.IsNullOrEmpty(sourceImageName))//�� ������ �ִٸ� ����
        {
            StartCoroutine(GenerateAsync());
        }
        else
        {
            Debug.LogError("No Source Image or Target Image! Check Path", this); return;
        }
    }
    IEnumerator GenerateAsync()//�񵿱Ⱑ�ƴ�. webrequest�κ� Ȯ��
    {
        generating = true;//�ߺ� ���� ����

        if (sdc == null)//�� ����Ʈ ��������
            sdc = FindObjectOfType<StableDiffusionConfiguration>();
        yield return sdc.SetModelAsync(modelsList[selectedModel]);//

        string[] targetImageName = loadTargetImages();//Ÿ�� �̹��� �̸��� �迭�� ����(�������� ���� 15���� ����)


        for (int i = 0; i < printPhotoNum; i++)//Ÿ�� �̹��� ������ŭ �ϳ��� ��� ����
        {
            SetupReactorImageName(i);//�ռ� �Ҷ����� �̸��� ������Ʈ�������. ���ϸ� ���� �̹����� ������.

            targetImagePath = targetImageFolderPath + targetImageName[i];//Ÿ�� �̹����� ���� "���"

            // Generate the image
            HttpWebRequest httpWebRequest = null;
            try
            {
                // Make a HTTP POST request to the Stable Diffusion server
                httpWebRequest = (HttpWebRequest)WebRequest.Create(sdc.settings.StableDiffusionServerURL + sdc.settings.ReactorAPI);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {

                    SDparamsInReactor sd = new SDparamsInReactor();

                    byte[] sourceBytes = File.ReadAllBytes(sourceImageName);//�� ������ ����Ʈ ���·� �ű�
                    byte[] targetBytes = File.ReadAllBytes(targetImagePath);//��� ������ ����Ʈ ���·� �ű�

                    //����Ʈ ���¿��� ��ȣȭ��Ų��(API �䱸����)
                    string sourceBase64String = base64Format + Convert.ToBase64String(sourceBytes);
                    string targetBase64String = base64Format + Convert.ToBase64String(targetBytes);

                    //��û body�� ����. �� ���� Reactor���� �������Ͽ� �ʿ��� ������ ���ø� �޼ҵ�� Setting.cs�� �⺻������ �����Ǿ�����.
                    sd.source_image = sourceBase64String;
                    sd.target_image = targetBase64String;

                    // Serialize the input parameters
                    string json = JsonConvert.SerializeObject(sd);

                    // Send to the server
                    streamWriter.Write(json);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message + "\n\n" + e.StackTrace);
            }
       
            // Read the output of generation
            if (httpWebRequest != null)
            {
                // Wait that the generation is complete before procedding
               Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();//�񵿱�� ������ ��ٸ���

                //result�� �����ϴ� ���� ������ ������ �ö����� �����带 �����·� ����� ������ ���� �ð����� �ڷ�ƾ�� ��ٸ��� �Ѵ�. 
                yield return new WaitForSecondsRealtime(waitHTTPTime);
               
                // Stream the result from the server
                var httpResponse = webResponse.Result;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    // Decode the response as a JSON string
                    string result = streamReader.ReadToEnd();

                    // Deserialize the JSON string into a data structure
                    SDResponseReactor json = JsonConvert.DeserializeObject<SDResponseReactor>(result);

                    // Check if the server returned an error
                    if (string.IsNullOrEmpty(json.image))
                    {
                        Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                        generating = false;
                        yield break;
                    }

                    // Decode the image from Base64 string into an array of bytes
                    byte[] imageData = Convert.FromBase64String(json.image);

                    // Write it in the specified project output folder
                    using (FileStream imageFile = new FileStream(reactorFilePath, FileMode.Create))
                    {

                        yield return imageFile.WriteAsync(imageData, 0, imageData.Length);

                    }           
                }
            }
            Debug.Log($"{i}��° �ռ� �Ϸ�");
        }
            generating = false;
            yield return null;    
    }
}
