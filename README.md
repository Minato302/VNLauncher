<h1>简介</h1>
<font size=4>VNLauncher是一款GalGame管理、翻译器。支持各类GalGame的游玩中翻译、游玩时长记录、CG截屏和管理等功能。在支持传统的百度翻译的同时，也支持在线与本地大模型翻译，结果更精确。</font>
<img src="https://s2.loli.net/2024/07/17/8lvLxZAsjyJ2ViS.gif" width="600" >
<h3>特点</h3>
<ul>
<li><font size=4>支持各类大模型翻译，带有上下文翻译更准确</font></li>
<li><font size=4>以PaddleOCR作为本地OCR识别，可使用GPU，速度快准确性高</font></li>
<li><font size=4>图形学算法自动识别OCR时机，可随意设置出字速度，不会漏截或者出字完成后仍需等待的情况</font></li>
<li><font size=4>功能多样，除自动翻译外还有截屏、框选翻译、游戏窗口控制等功能</font></li>
</ul>
<h1>上手指南</h1>


<font size=4>可以点击<a href="https://space.bilibili.com/23411401">这里</a>查看我的B站个人空间，里面有详细的教程。下面是一些简单的介绍。</font>
<h3>运行</h3>
<font size=4>这是一个WPF项目，推荐运行在Win10系统上。在下载时，Windows可能会提示您下载.NET6.0以及配套文件，在下载完后需要重启电脑再运行本软件，否则可能会有一些功能无法生效。</font>
<h3>主页面</h3>
<font size=4>在主页面处可以管理所有游戏、添加或删除游戏、查看游戏截图和启动游戏等等。</font>
<img src="https://s2.loli.net/2024/07/17/O47MQf5hsPZKHn8.png" width="600" >
<h3>OCR</h3>
<font size=4>本应用采用OCR获取原文。支持本地OCR与在线OCR。本地OCR集成在应用中，直接可以使用，是开源项目PaddleOCR的.NET版本<a href="https://github.com/sdcb/PaddleSharp">PaddleSharp</a>，准确率和效率均很高。其中对日文有V3和V4两个模型。V3模型的正确率在97%左右，V4模型基本不会出错，但需要GPU才能以较快的速度运行。在线OCR支持百度OCR，需要自己填入API Key等信息，且识别率低于V4模型，硬件条件支持的话不推荐。</font>
<h3>翻译</h3>
<font size=4>支持三种翻译方式，分别为传统的百度翻译和在线模型、本地模型翻译。在线模型支持OpenAI的各种模型和各种OpenAI的国内转发，以及其他所有与OpenAI API兼容的模型，例如DeepSeek。您可以根据自己的情况自定义Prompt和上下文句数等信息。</font>
<img src="https://s2.loli.net/2024/07/17/NXqloSfTYsDH5hL.png" width="600">

<font size=4>本地模型需要在<a href="https://github.com/oobabooga/text-generation-webui">text-generation</a>中加载<a href="https://github.com/SakuraLLM/SakuraLLM">Sakura翻译模型</a>，硬件条件支持可以得到不逊色于各类大语言模型的效果。可以看B站UP主的这两个视频：<a href="https://www.bilibili.com/video/BV1Te411U7me">text-generation的下载安装</a>和<a href="https://www.bilibili.com/video/BV18J4m1Y7Sa/">Sakura翻译模型的部署</a>，或者直接看我的介绍视频。</font>
<h3>添加游戏</h3>
添加游戏时需要进行初始设置，包括窗口捕获、封面截取和字幕位置框选三个步骤，系统将会引导您进行这些步骤。为了支持部分需要转区运行的游戏，系统支持两种游戏打开方式，一是点击开始游戏时系统直接打开并启动翻译字幕。然而有部分游戏需要转区才能运行，系统无法直接打开，这时可以选择手动启动，系统将在扫描到游戏窗口后自动启动翻译字幕。


<h1>使用注意</h1>
<ul>
<li><font size=4>本软件完全开源，您可以自由地使用、修改和分发本软件，但是需要遵守GPL协议，同时<strong>禁止一切商业用途</strong>。</font></li>
<li><font size=4><strong>在使用软件翻译游戏时，请确保您拥有游戏的使用权</strong>。</font></li>
<li>这是个人项目，且我的能力有限，因此可能会存在一些Bug，如果你遇到了欢迎提issue或者通过邮件（Yoshino0302@outlook.com）联系我。</li>
<li>如果您觉得这个项目对您有帮助，可以给一个star或者在B站关注我。 </li>
</ul>

<h1>引用项目</h1>

<ul>
<li><font size=4><a href="https://github.com/sdcb/PaddleSharp">PaddleSharp</a></font></li>
<li><font size=4><a href="https://github.com/MartinTopfstedt/FontAwesome6">FontAwesome</a>
</font></li>
<li><font size=4><a href="https://github.com/shimat/opencvsharp">OpenCVSharp4</a></font></li>
<li><font size=4><a href="https://github.com/JamesNK/Newtonsoft.Json">Newtonsoft Json</a></font></li>

</ul>
