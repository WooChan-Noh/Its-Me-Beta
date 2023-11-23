namespace OpenCvSharp.Demo
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using OpenCvSharp;
    using System.Collections;
    using System.IO;
    using TMPro;
    public class FaceDetectorScene : WebCamera
    {
        #region Default parameters
        public TextAsset faces;
        public TextAsset eyes;
        public TextAsset shapes;
        private FaceProcessorLive<WebCamTexture> processor;
        #endregion

        public GameObject printScreen;
        public Image bottomUI;
        public Image preocessDoneCheckUI;//사진 촬영 완료 화면에 나오는 체크 모양 UI
        public Image reactorLoadingUI;//사진 합성 중임을 알림
        public Image faceDetectionAnimation;//사진 촬영 중 나오는 로딩 애니메이션
       
        public Sprite originalBlackSprite;
        public Sprite processDoneBlueSprite;//사진 촬영 완료 화면에 UI들을 파란색으로 바꿔준다.
        public TextMeshProUGUI boxSize;//얼굴 인식 범위 확인용 (사용x)
        public TextMeshProUGUI infoMassage;//bottomUI의 문구
        public Color processColor = new Color(74f / 255f, 160f / 255f, 249f / 255f);
        StableDiffusionReactor stableDiffusionReactor;//통신하기 위해
        LoadPhoto loadPhoto;
        string infoText = "얼굴이 화면에 맞도록 가까이 와주세요";//디폴트 문구
        string processText = "얼굴 확인 진행 중...";
        string errorText = "얼굴이 너무 가깝거나 멀어요";

        static public string photoName = "Facemesh";//실제 얼굴 사진 이름
        string folderPath;//실제 얼굴 사진이 저장될 폴더.

        float waitingTime = 2.0f;//UI 변경에 쓰임
        bool textChangeFlag = false;
        string fileFormat = ".png";

        protected override void Awake()
        {
            base.Awake();
            base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

            byte[] shapeDat = shapes.bytes;
            if (shapeDat.Length == 0)
            {
                string errorMessage =
                    "In order to have Face Landmarks working you must download special pre-trained shape predictor " +
                    "available for free via DLib library website and replace a placeholder file located at " +
                    "\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
                    "Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
                // query user to download the proper shape predictor
                if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
                    Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
            }

            processor = new FaceProcessorLive<WebCamTexture>();
            processor.Initialize(faces.text, eyes.text, shapes.bytes);

            // data stabilizer - affects face rects, face landmarks etc.
            processor.DataStabilizer.Enabled = true;        // enable stabilizer
            processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
            processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

            // performance data - some tricks to make it work faster
            processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
            processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)

        }
        private void Start()
        {
            folderPath = Application.persistentDataPath + "/ItsmeBeta/Photo/";
            stableDiffusionReactor = GetComponent<StableDiffusionReactor>();
            loadPhoto = GetComponent<LoadPhoto>();
           
        }
        void InitVariables()//사진 찍혔을 때 또는 사람이 없을 때 측정값 초기화
        {
            processor.measureFaceTime = 0.0f;
            processor.measureEmptyTime = 0.0f;
            processor.squareSideLength = 0;
        }
        void ActiveUI()
        {
            //topUI.gameObject.SetActive(true);
            bottomUI.gameObject.SetActive(true);
            //maskUI.gameObject.SetActive(true);
        }
        void DeactivateUI(WebCamTexture input)
        {
            //topUI.gameObject.SetActive(false); // 이미지 UI 비활성화
            bottomUI.gameObject.SetActive(false);
            //maskUI.gameObject.SetActive(false);
            faceDetectionAnimation.gameObject.SetActive(false);
           
            processor.ProcessTexture(input, TextureParameters, false);// 얼굴 마커 비활성화 							
            infoMassage.gameObject.SetActive(false);   // 텍스트 비활성화
        }
        void SwitchToProcessDoneUI()//사진 촬영 완료 시 UI 색 변경
        {
            //topUI.sprite = processDoneBlueSprite;
            bottomUI.sprite = processDoneBlueSprite;
        }
        void SwitchToOriginalUI()
        {
            //topUI.sprite = originalBlackSprite;
            bottomUI.sprite = originalBlackSprite;
        }
        void UpdateInfo(bool loadingUIFlag, string text, Color color)//UI 정보 업데이트
        {
            faceDetectionAnimation.gameObject.SetActive(loadingUIFlag);
            infoMassage.text = text;
            infoMassage.color = color;
        }
        bool IsScreenshotCaptured(string filePath)//파일이 생성되었는지 확인
        {
            Debug.Log("얼굴 사진 파일을 확인하러 무한루프에 들어왔음");
            // 스크린샷 파일이 생성되었는지 여부를 확인하는 코드
            while (true)
            {
                if (File.Exists(filePath))
                {
                    Debug.Log("얼굴 사진 파일이 존재합니다.");
                    Debug.Log("무한루프 종료");
                    break;
                }
                else
                {
                    Debug.Log("얼굴 사진 파일이 존재하지 않습니다.");
                    Debug.Log("무한루프로 돌아갑니다.");
                }
            }
            return true;
        }
        IEnumerator TakePhoto()//사진 찍기 / UI 변경 / 합성 끝나면 다음 화면으로
        {
            //UI를 비활성화 시키고  1프레임 기다린 후 스크린샷 캡쳐 시작
            //기다리지 않으면 UI가 같이 찍힌다
            yield return new WaitForEndOfFrame();

            string filePath = Path.Combine(folderPath, photoName + fileFormat);
            ScreenCapture.CaptureScreenshot(filePath);//촬영 

            InitVariables();//찍었으니 파라미터 초기화
            SwitchToProcessDoneUI();//사진 촬영 완료 화면으로 UI 변경해 놓고
            ActiveUI();//UI 활성화
            preocessDoneCheckUI.gameObject.SetActive(true);//완료 화면에만 필요한 추가 UI(체크 모양)

            Time.timeScale = 0; // 프로그램을 일시 중지(얼굴 측정을 중지함)     
            yield return new WaitForSecondsRealtime(waitingTime);// 사진이 저장될 시간을 기다림
            yield return new WaitUntil(() => IsScreenshotCaptured(filePath));//사진이 저장되었는지 확인

    
            stableDiffusionReactor.Generate();//리액터로 합성 시작

            reactorLoadingUI.gameObject.SetActive(true);//로딩 화면 전환
           
            yield return new WaitUntil(()=>stableDiffusionReactor.generating==false);

            SwitchToOriginalUI();//모든 과정이 끝났으므로 기존 UI로 변경 -> 합성 끝나면
            infoMassage.gameObject.SetActive(true);//안내 문구 활성화

            //기존 UI에 포함되지 않는 UI들 다시 비활성화
            preocessDoneCheckUI.gameObject.SetActive(false);
            reactorLoadingUI.gameObject.SetActive(false);
            
            printScreen.SetActive(true);//다음 화면으로
            loadPhoto.images();//합성된 사진 불러오기
        }

        //************프로세스는 여기서 시작한다 - WebCamera.cs의 Update의 IF 조건에서 호출됨(오버라이드)************//
        //************WebCamera.cs에서 반드시 환경에 맞추어 해상도 조절 - 해상도가 맞지 않으면 인식률 저하************//
        protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
        {
            #region UI 텍스트 관련 코드

            ///Red Box 수치 확인 및 해상도 체크 (FaceSize 오브젝트 켜야함)
            ///boxSize.text = "Width&Height : " + processor.squareSideLength.ToString() + "\n" + myDeviceWidthRatio + " " + myDeviceHeightRatio;

            //얼굴 인식 거리가 허용범위가 아닌 경우 UI변경을 위한 if문
            if (processor.faceDistanceCheck == false && Time.timeScale != 0)
            {
                UpdateInfo(false, errorText, Color.white);
                processor.measureFaceTime -= Time.deltaTime * processor.timeControl * 2f;//인식 거리가 허용범위로 들어갔을 때 바로 찍히는 것을 방지하기 위한 누적 시간 조정
                textChangeFlag = true;//텍스트 변경을 위한 flag변수
            }

            //얼굴 인식 거리가 허용범위가 아니였다가 허용범위로 들어온 경우 UI변경을 위한 if문
            if (textChangeFlag == true && processor.faceDistanceCheck == true && Time.timeScale != 0)
            {
                UpdateInfo(true, processText, processColor);
                textChangeFlag = false;//텍스트 변경을 위한 flag변수
            }

            ///측정된 empty의 누적 시간이 초기화 시키는 기준empty시간보다 많으면 초기화	
            /// => 사람이 카메라 앞에 없다는 뜻이니까
            /// => 혹은 1초 이상 사람이 카메라 앞에서 얼굴을 돌렸다는 뜻이니까
            /// => 정면이 아닐 확률이 높으므로 다시 측정해야함
            if (processor.measureEmptyTime >= processor.resetTime)
            {
                processor.faceDistanceCheck = true;//false로 설정하면 안내 문구에 설정에 문제가 생김
                InitVariables();//empty 상태이니 측정하던 값들 초기화
                UpdateInfo(false, infoText, Color.white);//모든 측정 수치가 초기화 되었으므로 기존 안내 문구 출력        
            }
            #endregion

            #region 촬영

            //거리가 허용 범위인 상태에서 측정한 face의 누적 시간이 촬영 기준 시간보다 많으면 촬영 시작 
            if (processor.faceDistanceCheck == true && processor.measureFaceTime >= processor.takePhotoTime)
            {
                DeactivateUI(input);//스크린샷을 찍으므로 모든 요소 비활성화
                StartCoroutine(TakePhoto()); //촬영 시작                                                                                                            
            }
            else//측정하고 있는 face의 누적 시간이 촬영 기준 시간 아래면 측정을 계속한다.
            {
                // detect everything we're interested in
                processor.ProcessTexture(input, TextureParameters);

                //mark detected objects
                processor.MarkDetected();

                #region UI 코드
                if (processor.faceDistanceCheck == true && processor.measureFaceTime != 0.0f)//측정하고 있으면 UI 바꿔주기       
                    UpdateInfo(true, processText, processColor);//촬영중
                else if (processor.faceDistanceCheck == false && processor.measureFaceTime != 0.0f)//얼굴이 허용거리 벗어났으면 알려주기
                    UpdateInfo(false, errorText, Color.white);//예외상황          
                else
                    UpdateInfo(false, infoText, Color.white);//디폴트
                #endregion
            }
            #endregion
            // processor.Image now holds data we'd like to visualize
            output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created
            return true;
        }
    }
}
