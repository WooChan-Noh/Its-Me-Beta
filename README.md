[Read Me - English](https://github.com/WooChan-Noh/Its-Me-Beta/blob/main/ReadMeEng.md)
[Read Me - Japanese](https://github.com/WooChan-Noh/Its-Me-Beta/blob/main/ReadMeJp.md)

# Its-Me-Beta
+ AI를 활용히여 사진을 합성하는 포토부스 It's me의 베타 버전 입니다. 
+ UI resources are copyrighted by _Tinygem_
+ 타겟 이미지는 동료 프롬프트 엔지니어가 AI를 활용하여 제작했습니다.
  
## Overview
+ 얼굴이 감지되면 사진을 찍습니다.
+ Stable Diffusion API(_Reactor extension_)을 사용하여 얼굴을 합성합니다
+ 합성된 사진을 인쇄합니다.
### Test Environment
+ 사용한 웹캠 : Logitech C170, Logitech BRIO
+ 시용한 프린터 : DNP DS-RX1
+ 사용한 서버 : Runpod(3080ti)
+ 사용한 로컬PC : 4070ti 12G

### I used this projects
1. OpenCV for Unity : [Original Github](https://github.com/EnoxSoftware/OpenCVForUnity)
2. Stable Diffusion Unity Intergration : [Original Github](https://github.com/dobrado76/Stable-Diffusion-Unity-Integration)
+ 정확하게는 이 버전을 사용했습니다 : [Original Github](https://github.com/WooChan-Noh/SDReactorUnity)
3. Stable Diffusion WebUI : [Original Github](https://github.com/AUTOMATIC1111/stable-diffusion-webui)
4. Stable Diffusion WebUI Reactor : [Original Github](https://github.com/Gourieff/sd-webui-reactor)
5. Printer Connect Script (**협력 업체에서 제공했습니다**)
  
## Learn more
+ `Application.persistentDataPath + /ItsmeBeta/`경로에 `Photo`, `ResultImages`, `TargetImages` 빈 폴더가 필요합니다. 
+ 합성 사진의 배경이 될 타겟 이미지가 필요합니다. (size : 512x768) 
+ 타겟 이미지는 모두 `TargetImages` 폴더에 있어야 하며, 최소 20개 이상이 필요합니다.(제한 없음)
+ Ractor가 설치된 StableDiffusion의 URL이 필요합니다.. _Auotomatic1111 webUI Git을 확인하세요_
+ 웹캠이나 전면 카메라가 필요합니다. (카메라 모델이 바뀌는 경우 WebCamera.cs에서 수정하세요)
+ 프린터와 프로그램을 연결할 코드가 필요합니다. (코드는 제공하지 않습니다.)
+  `TargetImages` 폴더에서 15개의 이미지를 임의로 가져와 얼굴을 합성합니다.
   + (서버 환경에서 테스트 시 3~5분정도 걸럈습니다.)
   + (로컬 환경에서 테스트 시 30초~1분정도 걸렸습니다.)
+ 15개의 이미지는 하나의 배열로 관리합니다.
+ 모든 이미지는 자신이 클릭되었는지를 감지하고, 관리합니다.
+ 총 3개의 이미지가 체크되면, 별도로 텍스처링하여 프린터로 전송한 뒤, 출력합니다.    


## Known Issue
1. **에디터 모드**에서만 작동합니다. _(이유를 밝혀내지 못했습니다. 아마 OpenCV쪽 코드와 충돌이 일어나는 것으로 생각됩니다.)_
2. 메모리 누수 : 웹캠 텍스처로 인해 발생하는 것으로 생각됩니다.
3. 통신이 비동기로 진행되지 않습니다. 이 문제는 [SDReactorUnity](https://github.com/WooChan-Noh/SDReactorUnity)에서 Known Issue를 확인하세요
***
### Source Image
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/d8acef24-995d-4bd3-9a75-509b7a99c903" width="256" height="384"/></br>        
### Target Image    
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/98b9e230-57f5-4b09-9b9e-83c6d8927888" width="768" height="384"/></br>      
#### Result
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/52b7467e-6b2f-4a66-b85b-6044f9a463f0" width="768" height="384"/></br>
#### Photo
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/5f27604b-7db7-4551-a6e8-957d68113ab5" width="640" height="512"/></br>
#### Test(20sec)
![20sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/e6214071-c539-4f0e-8f42-710ecb989a14)
#### (25sec)
![25sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/118ef37c-833c-4203-95c2-2042a3ba8e8c)
#### (30sec)
![30sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/ffafc062-e5f9-45b8-ae36-f64c0f7b336c)


