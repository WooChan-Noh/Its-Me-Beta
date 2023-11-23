# Its-Me-Beta
+ It is AI image photo booth program **Beta** version    
+ UI resources are copyrighted by _Tinygem_
+ Target image was created by my fellow prompt engineers
  
## Overview
If detected faces, take a photo.    
Use the Stable Diffusion API(_Reactor extension_) to swap faces.    
Print a face-swapped image
### Test Environment
+ Used Cam : Logitech C170, Logitech BRIO
+ Used Printer : DNP DS-RX1
+ Used Server(SD) : Runpod(3080ti)
+ Used Local Computer : 4070ti 12G

### I used this projects
1. OpenCV for Unity : https://github.com/EnoxSoftware/OpenCVForUnity
2. Stable Diffusion Unity Intergration : https://github.com/dobrado76/Stable-Diffusion-Unity-Integration
+ To be precise, used this Version : https://github.com/WooChan-Noh/SDReactorUnity
3. Stable Diffusion WebUI : https://github.com/AUTOMATIC1111/stable-diffusion-webui
4. Stable Diffusion WebUI Reactor : https://github.com/Gourieff/sd-webui-reactor
5. Printer Connect Script (**Provided by a partner company**)
  
## Learn more
+ Under the path `Application.persistentDataPath + /ItsmeBeta/`, we need **folders** `Photo`, `ResultImages`, and `TargetImages` 
+ We need the target image, which is the background for compositing the face. (size : 512x768) 
+ The target images should all be in the `TargetImages` folder, and we need at least 20 of them (there is no maximum).
+ You will need **your own Stable Diffusion URL** where Reactor is installed and API communication is possible (local or server). _Check Auotomatic1111 project!_
+ You will need a **front-facing webcam** or **another webcam** (In this case, you need to modify the set webcam code in WebCamera.cs.)
+ You need printer and the code to connect it (Code can't be provided)
+ Randomly fetch 15 target images in "TargetImages" folder to swap faces.
  + (3-5 minutes for communication on the test environment server)
  + (30 sec ~ 1 min on local environment)
+ The 15 images are in one array
+ All images clicks are detected, clicked images are checked separately
+ and the checked images are textured separately and sent to the printer.(3 images)    


## Known Issue
1. Only works in **Editor Mode** _(I don't know why. Probably frequent calls to OpenCV are causing the build program to freeze)_
2. Memory leak : The webcam texture continues to exist. I didn't write any allocation code after freeing the memory, so it will **probably** cause memory issues after a long time.
3. Communication : Communication is **NOT** async. this project has same problem as SDReactorUnity (https://github.com/WooChan-Noh/SDReactorUnity)     _- Check Known Issue Part!_
***
### Source Image
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/d8acef24-995d-4bd3-9a75-509b7a99c903" width="256" height="384"/></br>        
### Target Image    
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/98b9e230-57f5-4b09-9b9e-83c6d8927888" width="768" height="384"/></br>      
#### Result
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/52b7467e-6b2f-4a66-b85b-6044f9a463f0" width="768" height="384"/></br>
#### Photo
<img src="https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/5f27604b-7db7-4551-a6e8-957d68113ab5" width="512" height="512"/></br>
#### Test(20sec)
![20sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/e6214071-c539-4f0e-8f42-710ecb989a14)
#### (25sec)
![25sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/118ef37c-833c-4203-95c2-2042a3ba8e8c)
#### (30sec)
![30sec](https://github.com/WooChan-Noh/Its-Me-Beta/assets/103042258/ffafc062-e5f9-45b8-ae36-f64c0f7b336c)


