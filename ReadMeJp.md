# Its-Me-Beta
+ AIを活用して写真を合成するフォトブース「It's me」のベータ版です。
+ UI resources are copyrighted by _Tinygem_
+ ターゲットイメージは仲間のプロンプトエンジニアがAIを活用して作成しました。
  
## Overview
+ 顔が検出されると写真を撮ります。
+ Stable Diffusion API(_Reactor extension_)を使って顔を合成します。
+ 合成された写真を印刷します。
### Test Environment
+ 使用したウェブカメラ : Logitech C170, Logitech BRIO
+ 使用したプリンター : DNP DS-RX1
+ 使用したサーバー : Runpod(3080ti)
+ 使用したローカルPC : 4070ti 12G

### I used this projects
1. OpenCV for Unity : [Original Github](https://github.com/EnoxSoftware/OpenCVForUnity)
2. Stable Diffusion Unity Intergration : [Original Github](https://github.com/dobrado76/Stable-Diffusion-Unity-Integration)
+ 正確にはこのバージョンを使いました : [Original Github](https://github.com/WooChan-Noh/SDReactorUnity)
3. Stable Diffusion WebUI : [Original Github](https://github.com/AUTOMATIC1111/stable-diffusion-webui)
4. Stable Diffusion WebUI Reactor : [Original Github](https://github.com/Gourieff/sd-webui-reactor)
5. Printer Connect Script (**協力会社から提供されました**)
  
## Learn more
+ `Application.persistentDataPath + /ItsmeBeta/`に `Photo`, `ResultImages`, `TargetImages`の空のフォルダが必要です。
+ 合成写真の背景となるターゲットイメージが必要です。 (size : 512x768)
+ ターゲットイメージは全て `TargetImages` sフォルダにある必要があり、最低20個以上必要です(制限なし)。
+ RactoがインストールされているStableDiffusionのURLが必要です。 _Auotomatic1111 webUI Gitを確認してください_
+ ウェブカメラまたはフロントカメラが必要です。 (カメラモデルが変わる場合は`WebCamera.cs`で修正してください)
+ プリンターとプログラムを接続するコードが必要です。 (コードは提供しません)
+  `TargetImages` フォルダから15枚のイメージをランダムに取り込み、顔を合成します。
   + (サーバー環境でテストした場合、3～5分くらいかかりました。)
   + (ローカル環境でテストした時、30秒～1分くらいかかりました。)
+ 15枚のイメージは一つの配列で管理します。
+ 全てのイメージは自分がクリックされたかどうかを検知して、管理します。
+ 合計3つのイメージがチェックされると、別々にテクスチャリングしてプリンターに送信します。 


## Known Issue
1. **エディタモードでのみ**動作します。(理由は不明です。おそらくOpenCVのコードと競合していると思われます。)
2. メモリリーク：ウェブカメラテクスチャが原因だと思われます。
3. 通信が非同期で行われない。 この問題は[SDReactorUnity](https://github.com/WooChan-Noh/SDReactorUnity)でKnown Issueを確認してください。
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
