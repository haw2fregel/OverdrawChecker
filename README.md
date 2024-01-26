# OverdrawChecker
オーバードローの確認用\
従来の明るさ表示に加えて、パーセント表示もできるようにします。

https://github.com/haw2fregel/OverdrawChecker/assets/143925343/7b1543eb-aa10-4e68-81fd-fc0fce62bbcf

## セットアップ
### インストール方法
UnityのPackageManagerを開き\
＋ボタンから「Add package from git URL...」を選択\
以下URLを入力してインストールします。\
・https://github.com/haw2fregel/OverdrawChecker.git?path=/Assets/OverdrawChecker

![image](https://github.com/haw2fregel/OverdrawChecker/assets/143925343/2aae7416-cfe1-4795-b3c0-4591a9e84aed)

### Renderer Data の登録
RenderPipelineAssetにRendererDataを2つ登録します\
・OverdrawRenderer\
・OverdrawUIRenderer\
![image](https://github.com/haw2fregel/OverdrawChecker/assets/143925343/6129156d-cb25-45ce-be40-c9b19c3f0d6c)

### Prefabの配置
OverdrawChecker.prefabをシーン上に配置\
![image](https://github.com/haw2fregel/OverdrawChecker/assets/143925343/7d1d69d8-3f70-4de3-94a8-1895300b7afe)

### オーバードローを確認
プレハブを配置した状態で、シーンを再生するとDisplay2に表示されます\
![image](https://github.com/haw2fregel/OverdrawChecker/assets/143925343/175d0077-8261-4570-acf7-147386d657d1)
