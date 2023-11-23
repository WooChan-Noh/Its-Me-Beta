namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;
	using OpenCvSharp;
	public abstract class WebCamera: MonoBehaviour
	{
	
		string macTestWebcam = "Webcam C170";//맥북에서 웹캠을 연결해서 테스트할때만 사용함(맥북 전면 캠 사용 불가능)
		string myWebCam = "Logitech BRIO";//사용하는 웹캠 이름

		//app에 나오는 해상도를 결정하는 변수
		[HideInInspector]
		public int myDeviceWidthRatio;
		[HideInInspector]
		public int myDeviceHeightRatio;
		//<서피스 프로7 기준> app 해상도 계산 변수
		int myDeviceScreenWidth;
		int myDeviceScreenHeight;

        #region Default Properties
        /// <summary>
        /// Target surface to render WebCam stream
        /// </summary>
        public GameObject Surface;

		private Nullable<WebCamDevice> webCamDevice = null;
		private WebCamTexture webCamTexture = null;
		private Texture2D renderedTexture = null;

		/// <summary>
		/// A kind of workaround for macOS issue: MacBook doesn't state it's webcam as frontal
		/// </summary>
		protected bool forceFrontalCamera = false;

		/// <summary>
		/// WebCam texture parameters to compensate rotations, flips etc.
		/// </summary>
		protected Unity.TextureConversionParams TextureParameters { get; private set; }

        /// <summary>
        /// Camera device name, full list can be taken from WebCamTextures.devices enumerator
        /// </summary>
        #endregion
   
        #region Default Methods
        public string DeviceName
        {
            get
            {
                return (webCamDevice != null) ? webCamDevice.Value.name : null;
            }
            set
            {
                // quick test
                if (value == DeviceName)
                    return;

                if (null != webCamTexture && webCamTexture.isPlaying)
                    webCamTexture.Stop();

                // get device index
                int cameraIndex = -1;
                for (int i = 0; i < WebCamTexture.devices.Length && -1 == cameraIndex; i++)
                {
                    if (WebCamTexture.devices[i].name == value)
                        cameraIndex = i;
                }

                // set device up
                if (-1 != cameraIndex)
                {
                    webCamDevice = WebCamTexture.devices[cameraIndex];
                    webCamTexture = new WebCamTexture(webCamDevice.Value.name);

                    // read device params and make conversion map
                    ReadTextureConversionParameters();

                    webCamTexture.Play();
                }
                else
                {
                    throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
                }
            }
        }
        /// <summary>
        /// This method scans source device params (flip, rotation, front-camera status etc.) and
        /// prepares TextureConversionParameters that will compensate all that stuff for OpenCV
        /// </summary>
        private void ReadTextureConversionParameters()
        {
            Unity.TextureConversionParams parameters = new Unity.TextureConversionParams();

            // frontal camera - we must flip around Y axis to make it mirror-like
            parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.Value.isFrontFacing;

            // TODO:
            // actually, code below should work, however, on our devices tests every device except iPad
            // returned "false", iPad said "true" but the texture wasn't actually flipped

            // compensate vertical flip
            //parameters.FlipVertically = webCamTexture.videoVerticallyMirrored;

            // deal with rotation
            if (0 != webCamTexture.videoRotationAngle)
                parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw

            // apply
            TextureParameters = parameters;

            //UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
        }
        /// <summary>
        /// Default initializer for MonoBehavior sub-classes
        /// </summary>
        void OnDestroy()
        {
            if (webCamTexture != null)
            {
                if (webCamTexture.isPlaying)
                {
                    webCamTexture.Stop();
                }
                webCamTexture = null;
            }

            if (webCamDevice != null)
            {
                webCamDevice = null;
            }
        }
        /// <summary>
        /// Updates web camera texture
        /// </summary>
        /// <summary>
		/// Processes current texture
		/// This function is intended to be overridden by sub-classes
		/// </summary>
		/// <param name="input">Input WebCamTexture object</param>
		/// <param name="output">Output Texture2D object</param>
		/// <returns>True if anything has been processed, false if output didn't change</returns>
		protected abstract bool ProcessTexture(WebCamTexture input, ref Texture2D output);
        private void Update()
        {
            if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
            {
                // this must be called continuously
                ReadTextureConversionParameters();

                // process texture with whatever method sub-class might have in mind

                if (ProcessTexture(webCamTexture, ref renderedTexture))
                {
                    RenderFrame();
                }
            }
        }//촬영시작

        #endregion

        protected virtual void Awake()// WebCam 연결
        {

            if (WebCamTexture.devices[WebCamTexture.devices.Length-1].name == macTestWebcam)
                DeviceName = macTestWebcam;//맥북 테스트 - 맥북의 전면 카메라는 동작하지 않는다
            else if(WebCamTexture.devices[WebCamTexture.devices.Length-1].name == myWebCam)
                DeviceName = myWebCam;//사용 중인 웹캠
            else
				DeviceName = WebCamTexture.devices[0].name;//전면 카메라	
       
        }	
		/// <summary>
		/// Renders frame onto the surface
		/// </summary>
		private void RenderFrame()//화면 비율 수정 -> 시간 없어서 하드코딩함
        {
			if (renderedTexture != null)
			{
				// apply
				Surface.GetComponent<RawImage>().texture = renderedTexture;

                // Adjust image ration according to the texture sizes 
//                Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(renderedTexture.width,renderedTexture.height);

                myDeviceScreenWidth = 1920;
                myDeviceScreenHeight = 1080;
                if(myDeviceScreenWidth==1920)

                myDeviceWidthRatio = myDeviceScreenHeight;
                myDeviceHeightRatio = myDeviceScreenHeight * renderedTexture.width / renderedTexture.height;


                Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(2560, 1920);
                Surface.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);



            }
        }

    }
}