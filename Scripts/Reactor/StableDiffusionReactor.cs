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
    public int selectedSampler = 16;//DPM++ SDE Karras임
    [HideInInspector]
    public int selectedModel = 0;//Xl모델

    public int width = 512;
    public int height = 512;
    public int steps = 50;
    public float cfgScale = 7;
    public long seed = -1;
    public long generatedSeed = -1;

    public bool generating = false;//중복 생성 방지 
    #endregion

    #region API Parameter
    string base64Format = "data:image/png;base64,";
    string fileFormat = ".png";
    #endregion

    int printPhotoNum = 15;//사용자가 볼 수 있는 총 사진 수 

    public string sourceImageFolderPath = "";//사용자의 얼굴 사진이 저장된 "폴더" 경로
    string sourceImageName = "";//사용자의 얼굴 사진 이름(용도 : Reactor와 통신) - Generate()에서 설정됨

    public string targetImageFolderPath = "";//타겟 이미지가 전부 저장된 "폴더" 경로
    string targetImagePath = "";//타겟 이미지 최종 경로(용도 : Reactor와 통신)
    string[] targetImageFolderList;//타겟 이미지 이름들이 저장된 배열
    public static int targetImagesFolderLength=0;//타겟 이미지 총 개수 => GenerateAsync()에서 targetImageName 설정에 사용된다.

    float waitHTTPTime = 6f;//웹 응답 대기 시간 (리액터 생성 시간동안 멈추지 않게) -3090에 적당함

    public string resultImageFolderPath = "";//리액터로 합성된 사진이 저장될 "폴더" 경로
    private string reactorFilePath = "";//리액터로 합성된 "사진"의 최종 경로 - 위의 두 string의 합

 
    private void Start()
    {
        selectedSampler = 16;
        sourceImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/Photo/";
        targetImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/TargetImages/";
        resultImageFolderPath = Application.persistentDataPath + "/ItsmeBeta/ResultImages/";

        // 타겟이미지가 굉장히 많아지면 성능에 영향을 미칠 것이다
        targetImageFolderList = Directory.GetFiles(targetImageFolderPath);//타겟 이미지를 받아온다. 몇장이든 상관없음
        targetImagesFolderLength=targetImageFolderList.Length;
    }
    void SetupReactorImageName(int num)
    {
        try
        {
            reactorFilePath = Path.Combine(resultImageFolderPath, "Reactor" + num + fileFormat);//리액터로 합성되어 저장될 사진 경로
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }
    string[] loadTargetImages()//전체 타겟 이미지 중에서 랜덤으로 15개를 선정해 저장함
    {
        int[] usedNumbers = new int[targetImagesFolderLength];
        int randomIndex=0;

        string[] targetImages = new string[printPhotoNum];//합성될 15개의 이미지
 
        for (int i = 0; i < printPhotoNum; i++)
        {
            do
            {
              randomIndex = UnityEngine.Random.Range(0, targetImagesFolderLength);
            } while (usedNumbers[randomIndex] == 1);
            usedNumbers[randomIndex] = 1;
            targetImages[i] = Path.GetFileName(targetImageFolderList[randomIndex]);
          
        }
        Debug.Log("타겟 이미지 로딩 완료");
        return targetImages;

    }
    public void Generate()
    {
        sourceImageName = sourceImageFolderPath+FaceDetectorScene.photoName + fileFormat;//얼굴 사진 이름 설정(용도 : Reactor와 통신)
        if (!generating && !string.IsNullOrEmpty(sourceImageName))//얼굴 사진이 있다면 시작
        {
            StartCoroutine(GenerateAsync());
        }
        else
        {
            Debug.LogError("No Source Image or Target Image! Check Path", this); return;
        }
    }
    IEnumerator GenerateAsync()//비동기가아님. webrequest부분 확인
    {
        generating = true;//중복 생성 방지

        if (sdc == null)//모델 리스트 가져오기
            sdc = FindObjectOfType<StableDiffusionConfiguration>();
        yield return sdc.SetModelAsync(modelsList[selectedModel]);//

        string[] targetImageName = loadTargetImages();//타겟 이미지 이름들 배열에 저장(랜덤으로 뽑힌 15개가 있음)


        for (int i = 0; i < printPhotoNum; i++)//타겟 이미지 개수만큼 하나씩 통신 시작
        {
            SetupReactorImageName(i);//합성 할때마다 이름을 업데이트해줘야함. 안하면 기존 이미지를 덮어씌운다.

            targetImagePath = targetImageFolderPath + targetImageName[i];//타겟 이미지의 최종 "경로"

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

                    byte[] sourceBytes = File.ReadAllBytes(sourceImageName);//얼굴 사진을 바이트 형태로 옮김
                    byte[] targetBytes = File.ReadAllBytes(targetImagePath);//배경 사진을 바이트 형태로 옮김

                    //바이트 형태에서 암호화시킨다(API 요구사항)
                    string sourceBase64String = base64Format + Convert.ToBase64String(sourceBytes);
                    string targetBase64String = base64Format + Convert.ToBase64String(targetBytes);

                    //요청 body에 전달. 이 외의 Reactor에서 업스케일에 필요한 설정과 샘플링 메소드는 Setting.cs에 기본적으로 설정되어있음.
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
               Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();//비동기로 응답을 기다린다

                //result에 접근하는 순간 웹에서 응답이 올때까지 스레드를 대기상태로 만들기 때문에 응답 시간동안 코루틴을 기다리게 한다. 
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
            Debug.Log($"{i}번째 합성 완료");
        }
            generating = false;
            yield return null;    
    }
}
