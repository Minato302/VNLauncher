#pragma warning disable IDE0049

using Newtonsoft.Json.Linq;
using OpenCvSharp.Extensions;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VNLauncher.Controls;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Windows
{
    public partial class MarqueeWindow : System.Windows.Window
    {
        private IntPtr gameWindowHwnd;
        private Game game;
        private System.Timers.Timer windowMonitoringTimer;
        private System.Timers.Timer tWindowTimer;
        private GlobalHook hook;
        private OCR scanner;
        private TransparentWindow tWindow;
        private FileManager fileManager;
        private MainWindow mainWindow;
        private LocalColorAcquirer resource;
        private dynamic settingResponseData;

        private Translator translator;

        private UInt64 scanActionID;
        private MarqueeHidder hidder;
        private Boolean isBoxSelecting;

        public MarqueeWindow(IntPtr gameWindowHwnd, Game game, MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.gameWindowHwnd = gameWindowHwnd;
            this.game = game;
            fileManager = new FileManager();
            resource = new LocalColorAcquirer();
            windowMonitoringTimer = new System.Timers.Timer(1000);
            windowMonitoringTimer.Elapsed += OnTimedEvent;
            windowMonitoringTimer.AutoReset = true;
            windowMonitoringTimer.Enabled = true;
            isBoxSelecting = false;

            tWindowTimer = new System.Timers.Timer(2000);
            tWindowTimer.Elapsed += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    tWindowTimer.Stop();
                    tWindow!.Visibility = Visibility.Hidden;
                });
            };
            windowMonitoringTimer.AutoReset = true;

            tWindow = new TransparentWindow(gameWindowHwnd, game.Loaction);
            tWindow.Show();
            tWindow.Visibility = Visibility.Hidden;
            scanActionID = 0;
            hidder = new MarqueeHidder(this);
            SettingInit();
        }

        private void OCRInit()
        {
            scanner = new LocalOCR((Boolean)settingResponseData.ocr.localOCR.isV4Model, (Boolean)settingResponseData.ocr.localOCR.usingGPU);

        }
        private void HookInit()
        {
            hook = new GlobalHook(new List<(String, Action)>
            {
                ((String)settingResponseData.keyMapping.translateSwitch,TranslateSwitch),
                ((String)settingResponseData.keyMapping.retranslate,Retranslate),
                ((String)settingResponseData.keyMapping.screenShot,ScreenShot),
                ((String)settingResponseData.keyMapping.showMarquee,ShowMarquee),
                ((String)settingResponseData.keyMapping.captureSideUpMove,CaptureSideUpMove),
                ((String)settingResponseData.keyMapping.captureSideDownMove,CaptureSideDownMove),
                ((String)settingResponseData.keyMapping.captureSideLeftMove,CaptureSideLeftMove),
                ((String)settingResponseData.keyMapping.captureSideRightMove,CaptureSideRightMove),
                ((String)settingResponseData.keyMapping.boxSelectAndTranslate,BoxSelectAndTranslate),
                ("鼠标左键",WaitAndScanAndTranslate),
            });
        }
        private void TranslateModePopupInit()
        {
            if (!(Boolean)settingResponseData.ocr.onlineOCR.enabled)
            {
                onlineOCRRadioButton.SetUnenabled();
            }
            if (!(Boolean)settingResponseData.baiduTranslate.enabled)
            {
                baiduTranslateRadioButton.SetUnenabled();
            }
            if (!(Boolean)settingResponseData.localTranslate.enabled)
            {
                localTranslateRadioButton.SetUnenabled();
            }
            if (!(Boolean)settingResponseData.onlineModelTranslate.enabled)
            {
                onlineModelTranslateRadioButton.SetUnenabled();
            }

            Action ocrChangedAction = () =>
            {
                if (onlineOCRRadioButton.IsChecked)
                {
                    scanner = new BaiduOCR((String)settingResponseData.ocr.onlineOCR.apiKey, (String)settingResponseData.ocr.onlineOCR.secretKey);
                }
                else
                {
                    scanner = new LocalOCR((Boolean)settingResponseData.ocr.localOCR.isV4Model, (Boolean)settingResponseData.ocr.localOCR.usingGPU);
                }
            };

            Action translateChangedAction = () =>
            {
                if (localTranslateRadioButton.IsChecked)
                {
                    translator = new LocalTranslator((String)settingResponseData.localTranslate.url, (Int32)settingResponseData.localTranslate.context, (String)settingResponseData.localTranslate.prompt);
                }
                else if (onlineModelTranslateRadioButton.IsChecked)
                {
                    translator = new OnlineModelTranslator((String)settingResponseData.onlineModelTranslate.apiKey, (String)settingResponseData.onlineModelTranslate.url,
                    new OnlineModelTranslator.Prompt((Boolean)settingResponseData.onlineModelTranslate.prompt.hasContext, (Boolean)settingResponseData.onlineModelTranslate.prompt.contextFirst,
                        (String)settingResponseData.onlineModelTranslate.prompt.prompt1, (String)settingResponseData.onlineModelTranslate.prompt.prompt2, (String)settingResponseData.onlineModelTranslate.prompt.prompt3),
                        (Int32)settingResponseData.onlineModelTranslate.context, (String)settingResponseData.onlineModelTranslate.model);
                }
                else if (baiduTranslateRadioButton.IsChecked)
                {
                    translator = new BaiduTranslator((String)settingResponseData.baiduTranslate.apiKey, (String)settingResponseData.baiduTranslate.secretKey);
                }
            };
            MarqueeRadioButton.SetGroup(new List<MarqueeRadioButton> { onlineOCRRadioButton, localOCRRadioButton }, ocrChangedAction, localOCRRadioButton);
            MarqueeRadioButton.SetGroup(new List<MarqueeRadioButton> { localTranslateRadioButton, onlineModelTranslateRadioButton, baiduTranslateRadioButton }, translateChangedAction);

            if (onlineModelTranslateRadioButton.IsEnabled)
            {
                onlineModelTranslateRadioButton.SetChecked();

                translator = new OnlineModelTranslator((String)settingResponseData.onlineModelTranslate.apiKey, (String)settingResponseData.onlineModelTranslate.url,
                new OnlineModelTranslator.Prompt((Boolean)settingResponseData.onlineModelTranslate.prompt.hasContext, (Boolean)settingResponseData.onlineModelTranslate.prompt.contextFirst,
                (String)settingResponseData.onlineModelTranslate.prompt.prompt1, (String)settingResponseData.onlineModelTranslate.prompt.prompt2, (String)settingResponseData.onlineModelTranslate.prompt.prompt3),
                (Int32)settingResponseData.onlineModelTranslate.context, (String)settingResponseData.onlineModelTranslate.model);
            }
            else if (localTranslateRadioButton.IsEnabled)
            {
                localTranslateRadioButton.SetChecked();
                translator = new LocalTranslator((String)settingResponseData.localTranslate.url, (Int32)settingResponseData.localTranslate.context, (String)settingResponseData.localTranslate.prompt);
            }
            else if (baiduTranslateRadioButton.IsEnabled)
            {
                baiduTranslateRadioButton.SetChecked();
                translator = new BaiduTranslator((String)settingResponseData.baiduTranslate.apiKey, (String)settingResponseData.baiduTranslate.secretKey);
            }
            else
            {
                translator = new Translator();
            }
        }
        public void WaitModePopupInit()
        {
            Action waitModeChangedAction = () =>
            {
                if (waitFixTimeRadioButton.IsChecked)
                {
                    waitTimeSlider.IsEnabled = true;
                    waitTimeTextBlock.IsEnabled = true;
                }
                else
                {
                    waitTimeSlider.IsEnabled = false;
                    waitTimeTextBlock.IsEnabled = false;
                }
            };
            MarqueeRadioButton.SetGroup(new List<MarqueeRadioButton> { waitFixTimeRadioButton, autoWaitRadioButton }, waitModeChangedAction, (Boolean)settingResponseData.marquee.isAutoWait ? autoWaitRadioButton : waitFixTimeRadioButton);
            waitTimeSlider.Value = settingResponseData.marquee.waitTime;
        }
        public void MarqueeStylePopupInit()
        {
            MarqueeRadioButton.SetGroup(new List<MarqueeRadioButton> { jpnChsRadioButton, chsOnlyRadioButton }, () => { }, (Boolean)settingResponseData.marquee.bilingual ? jpnChsRadioButton : chsOnlyRadioButton);

            textTransparencySlider.Value = settingResponseData.marquee.textTransparency;
            backgroundTransparencySlider.Value = settingResponseData.marquee.backgroundTransparency;
            fontSizeSlider.Value = settingResponseData.marquee.fontSize;
        }

        public void SettingInit()
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            settingResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            OCRInit();
            HookInit();
            TranslateModePopupInit();
            WaitModePopupInit();
            MarqueeStylePopupInit();
        }


        void MouseDragMove(Object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MarqueeCloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void TranslateButton_Click(Object sender, RoutedEventArgs e)
        {
            TranslateSwitch();
        }
        private void ShowMarquee()
        {
            if (Visibility == Visibility.Visible)
            {
                Visibility = Visibility.Hidden;
            }
            else
            {
                Visibility = Visibility.Visible;
            }
        }
        private Rectangle gameWindowLocation;
        private void OnTimedEvent(Object? sender, System.Timers.ElapsedEventArgs e)
        {
            WindowInfo? window = WindowsHandler.GetWindow(game.WindowTitle, game.WindowClass);
            if (window == null)
            {
                Dispatcher.Invoke(() =>
                {
                    Topmost = true;
                    game.EndGame();
                    tWindow.Close();
                    windowMonitoringTimer.Stop();
                    mainWindow.SetStartButton(false);
                    Close();

                });
            }
            else
            {
                if (gameWindowLocation != ((WindowInfo)window).Bounds)
                {
                    Dispatcher.Invoke(() =>
                    {
                        tWindow.Visibility = Visibility.Visible;
                        gameWindowLocation = ((WindowInfo)window).Bounds;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        tWindow.Visibility = Visibility.Hidden;
                        gameWindowLocation = ((WindowInfo)window).Bounds;
                    });
                }
            }
        }



        private async void WaitAndScanAndTranslate()
        {
            if (translateButton.IsTranslating && !isBoxSelecting)
            {
                OCR.OCRResult result = null;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Waiting);
                });


                if (autoWaitRadioButton.IsChecked)
                {
                    result = await Task.Run(AutoWaitAndScan);
                }
                else
                {
                    result = await Task.Run(WaitFixTimeAndScan);

                }
                String oriJpSentence = "";
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (result.HasContent)
                    {
                        oriJpSentence = TextModifier.Modify(result.ResultText);
                        if (chsOnlyRadioButton.IsChecked)
                        {
                            marqueeTextBlock.Text = "";
                        }
                        else
                        {
                            marqueeTextBlock.Text = oriJpSentence;
                        }
                        stateInfo.ChangeState(MarqueeStateInfo.State.Translating);
                        SerialTranslate(oriJpSentence);
                    }
                    else
                    {
                        marqueeTextBlock.Text = "未识别到内容";
                        stateInfo.ChangeState(MarqueeStateInfo.State.Over);
                    }
                });

            }
        }

        private async Task<OCR.OCRResult> AutoWaitAndScan()
        {
            scanActionID++;
            UInt64 key = scanActionID;
            try
            {
                while (true)
                {
                    if (!game.IsWindowShot)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Visibility = Visibility.Hidden;
                        });
                    }

                    Bitmap lastCapture = GetGameShot(game.Loaction);
                    List<Int32> historySimilarities = new List<Int32>();
                    for (Int32 i = 0; i < 10; i++)
                    {
                        if (scanActionID != key)
                        {
                            throw new OperationCanceledException();
                        }
                        await Task.Delay(200);

                        Bitmap capture = GetGameShot(game.Loaction);
                        Int32 similarity = ImageHandler.CalculateWhiteIntersections(BitmapConverter.ToMat(capture));
                        Int32 sameCount = 0;
                        foreach (Int32 historySimilarity in historySimilarities)
                        {
                            if (similarity != 0)
                            {
                                if (historySimilarity >= similarity)
                                {
                                    sameCount++;
                                }
                            }
                        }
                        if (sameCount < 2)
                        {
                            historySimilarities.Add(similarity);
                            lastCapture = new Bitmap(capture);
                        }
                        else
                        {
                            break;
                        }
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        stateInfo.ChangeState(MarqueeStateInfo.State.Identifying);
                    });

                    if (!game.IsWindowShot)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Visibility = Visibility.Visible;
                        });
                    }

                    OCR.OCRResult result = await scanner.Scan(lastCapture);
                    if (result.HasContent)
                    {
                        return result;
                    }
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Waiting);
                    });
                }
            }
            catch (Exception ex)
            {
                OCR.OCRResult result = new OCR.OCRResult(false);
                return result;
            }
        }
        private async Task<OCR.OCRResult> WaitFixTimeAndScan()
        {
            try
            {
                scanActionID++;
                UInt64 key = scanActionID;

                Int32 ms = 0;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ms = Convert.ToInt32(waitTimeSlider.Value * 100);
                });
                await Task.Delay(ms);
                if (key != scanActionID)
                {
                    throw new OperationCanceledException();
                }


                if (!game.IsWindowShot)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Visibility = Visibility.Hidden;
                    });
                    Thread.Sleep(100);
                }
                Bitmap capture = GetGameShot(game.Loaction);

                if (!game.IsWindowShot)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Visibility = Visibility.Visible;
                    });
                }


                Application.Current.Dispatcher.Invoke(() =>
                {
                    stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Identifying);
                });
                OCR.OCRResult result = await scanner.Scan(capture);
                return result;
            }
            catch (Exception)
            {
                OCR.OCRResult result = new OCR.OCRResult(false);
                return result;
            }

        }
        private async void Retranslate()
        {
            scanActionID++;
            Bitmap capture = GetGameShot(game.Loaction);

            if (!game.IsWindowShot)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Visible;
                });
            }

            OCR.OCRResult result = null;
            await Task.Run(async () =>
            {
                result = await scanner.Scan(capture);
                Application.Current.Dispatcher.Invoke(() =>
                 {
                     stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Identifying);
                 });
            });
            if (result!.HasContent)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Translating);
                    if (chsOnlyRadioButton.IsChecked)
                    {
                        marqueeTextBlock.Text = "";
                    }
                    else
                    {
                        marqueeTextBlock.Text = result.ResultText;
                    }
                    translator.RemoveLast();
                    SerialTranslate(result.ResultText);
                });
            }
            else
            {
                stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Over);
                marqueeTextBlock.Text = "未识别到内容";
            }

        }
        private void ScreenShot()
        {
            String soundPath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\ScreenshotSound.mp3";
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(soundPath));
            mediaPlayer.Play();

            String time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            Bitmap capture = new Bitmap(1, 1);
            if (!game.IsWindowShot)
            {
                capture = GetGameShot();
            }
            else
            {
                capture = GetGameShot();
            }

            fileManager.SaveCapture(game.Name, capture, time);
            ScreenShotWindow sWindow = new ScreenShotWindow(ImageHandler.ConvertBitmapToBitmapSource(capture));
            sWindow.Show();
        }
        private Bitmap GetGameShot()
        {
            Bitmap capture;
            if (!game.IsWindowShot)
            {
                hidder.Hide();
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.FullScreenCut);
                hidder.Restore();
            }
            else
            {
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.Window);
            }
            return capture;
        }
        private Bitmap GetGameShot(Game.CaptionLocation location)
        {
            Bitmap capture;
            if (!game.IsWindowShot)
            {
                hidder.Hide();
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.FullScreenCut, location);
                hidder.Restore();
            }
            else
            {
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.Window, location);
            }
            return capture;
        }
        private async void SerialTranslate(String jpContent)
        {
            String cnContent = await translator.SerialTranslate(jpContent);
            if (jpnChsRadioButton.IsChecked)
            {
                marqueeTextBlock.Text += '\n';
            }
            marqueeTextBlock.Text += cnContent;
            stateInfo.ChangeState(MarqueeStateInfo.State.Over);
        }
        private async void Translate(String jpContent)
        {
            String cnContent = await translator.Translate(jpContent);
            if (jpnChsRadioButton.IsChecked)
            {
                marqueeTextBlock.Text += '\n';
            }
            marqueeTextBlock.Text += cnContent;
            stateInfo.ChangeState(MarqueeStateInfo.State.Over);
        }

        private void TranslateSwitch()
        {
            translateButton.Turn();
            if (translateButton.IsTranslating)
            {
                adjustSideButton.IsEnabled = false;
                marqueeStyleButton.IsEnabled = false;
                selectTranslateModeButton.IsEnabled = false;
                windowOperatorButton.IsEnabled = false;
                waitModeButton.IsEnabled = false;
                stateInfo.ChangeState(MarqueeStateInfo.State.Over);
            }
            else
            {
                adjustSideButton.IsEnabled = true;
                marqueeStyleButton.IsEnabled = true;
                selectTranslateModeButton.IsEnabled = true;
                windowOperatorButton.IsEnabled = true;
                waitModeButton.IsEnabled = true;
                stateInfo.ChangeState(MarqueeStateInfo.State.Closed);
            }
        }

        public void CaptureSideLeftMove()
        {
            if (adjustSideButton.Side == MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.LeftSideLeftMove();
                tWindow.LeftSideLeftMove();
            }
            else
            {
                game.Loaction.RightSideLeftMove();
                tWindow.RightSideLeftMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void CaptureSideRightMove()
        {
            if (adjustSideButton.Side == MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.LeftSideRightMove();
                tWindow.LeftSideRightMove();
            }
            else
            {
                game.Loaction.RightSideRightMove();
                tWindow.RightSideRightMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void CaptureSideUpMove()
        {
            if (adjustSideButton.Side == MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.UpSideUpMove();
                tWindow.UpSideUpMove();
            }
            else
            {
                game.Loaction.DownSideUpMove();
                tWindow.DownSideUpMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void CaptureSideDownMove()
        {
            if (adjustSideButton.Side == MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.UpSideDownMove();
                tWindow.UpSideDownMove();
            }
            else
            {
                game.Loaction.DownSideDownMove();
                tWindow.DownSideDownMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }

        private void Window_Unloaded(Object sender, RoutedEventArgs e)
        {
            JObject marquee = new JObject
            {
                ["bilingual"] = jpnChsRadioButton.IsChecked,
                ["isAutoWait"] = autoWaitRadioButton.IsChecked,
                ["backgroundTransparency"] = (Int32)backgroundTransparencySlider.Value,
                ["textTransparency"] = (Int32)textTransparencySlider.Value,
                ["fontSize"] = (Int32)fontSizeSlider.Value,
                ["waitTime"] = (Int32)waitTimeSlider.Value,

            };
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.marquee = marquee;
            File.WriteAllText(fileManager.UserDataJsonPath, responseData.ToString());

            if (MainWindowGameButton.SelectedGameButton?.Game == game)
            {
                mainWindow.UpdateGameInfo(game);
            }

            hook.UninstallHook();
        }

        private void MarqueeStyleButton_Click(Object sender, RoutedEventArgs e)
        {
            if (marqueeStylePopup.IsOpen)
            {
                marqueeStylePopup.IsOpen = false;
            }
            else
            {
                marqueeStylePopup.IsOpen = true;
                selectTranslateModePopup.IsOpen = false;
                windowOperatorPopup.IsOpen = false;
                waitModePopup.IsOpen = false;
            }
        }

        private void SelectTranslateModeButton_Click(Object sender, RoutedEventArgs e)
        {
            if (selectTranslateModePopup.IsOpen)
            {
                selectTranslateModePopup.IsOpen = false;
            }
            else
            {
                selectTranslateModePopup.IsOpen = true;
                marqueeStylePopup.IsOpen = false;
                windowOperatorPopup.IsOpen = false;
                waitModePopup.IsOpen = false;
            }
        }
        private void WindowOperatorButton_Click(Object sender, RoutedEventArgs e)
        {
            if (windowOperatorPopup.IsOpen)
            {
                windowOperatorPopup.IsOpen = false;
            }
            else
            {
                windowOperatorPopup.IsOpen = true;
                marqueeStylePopup.IsOpen = false;
                selectTranslateModePopup.IsOpen = false;
                waitModePopup.IsOpen = false;
            }
        }

        private void MainBorder_MouseRightButtonDown(Object sender, MouseButtonEventArgs e)
        {
            marqueeStylePopup.IsOpen = false;
            selectTranslateModePopup.IsOpen = false;
            windowOperatorPopup.IsOpen = false;
            waitModePopup.IsOpen = false;
        }
        private void WaitModeButton_Click(Object sender, RoutedEventArgs e)
        {
            if (waitModePopup.IsOpen)
            {
                waitModePopup.IsOpen = false;
            }
            else
            {
                waitModePopup.IsOpen = true;
                marqueeStylePopup.IsOpen = false;
                selectTranslateModePopup.IsOpen = false;
                windowOperatorPopup.IsOpen = false;
            }
        }
        private void RemoveUIButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowsHandler.RemoveUI(gameWindowHwnd);
        }

        private void RestoreUIButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowsHandler.RestoreUI(gameWindowHwnd);
        }

        private void RestoreToNormalWindow_Click(Object sender, RoutedEventArgs e)
        {
            WindowsHandler.RestoreToNormalWindow(gameWindowHwnd);
        }

        private void MaximizeAndFullscreenButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowsHandler.MaximizeAndFullscreen(gameWindowHwnd);
        }

        private void WaitTimeSlider_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            if (waitTimeTextBlock != null)
            {
                waitTimeTextBlock.Text = (e.NewValue / 10).ToString("0.0") + "s";
            }
        }

        private void BackgroundTransparencySlider_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            mainBorder.Background = resource.GetColor("marqueeBackgroundColor", Convert.ToByte(e.NewValue));
        }

        private void TextTransparencySlider_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            marqueeTextBlock.Foreground = resource.GetColor("marqueeTextColor", Convert.ToByte(e.NewValue));
        }

        private void FontSizeSlider_ValueChanged(Object sender, RoutedPropertyChangedEventArgs<Double> e)
        {
            marqueeTextBlock.FontSize = e.NewValue;
        }
        private void BoxSelectAndTranslate()
        {
            isBoxSelecting = true;
            hidder.Hide();
            SemitransparentWindow stWindow = new SemitransparentWindow(this);
            stWindow.Show();

        }
        public async void BoxTranslate(System.Drawing.Point leftUp, System.Drawing.Point rightDown)
        {
            Bitmap bitmap = WindowCapture.CaptureScreen();
            Bitmap crop = ImageHandler.CropToBox(bitmap, leftUp, rightDown);
            OCR.OCRResult result = await scanner.Scan(crop);
            hidder.Restore();
            if (result.HasContent)
            {
                if (chsOnlyRadioButton.IsChecked)
                {
                    marqueeTextBlock.Text = "";
                }
                else 
                {
                    marqueeTextBlock.Text = result.ResultText;
                }
                stateInfo.ChangeState(MarqueeStateInfo.State.Translating);
                Translate(result.ResultText);
                stateInfo.ChangeState(MarqueeStateInfo.State.Over);
            }
            else
            {
                marqueeTextBlock.Text = "未识别到内容";
            }
            isBoxSelecting = false;
        }
    }
    public class MarqueeHidder
    {
        private Double minWidth;
        private Double minHeight;
        private Double nowWidth;
        private Double nowHeight;
        private MarqueeWindow marquee;
        public MarqueeHidder(MarqueeWindow marquee)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                minWidth = marquee.MinWidth;
                minHeight = marquee.MinHeight;
                nowWidth = marquee.Width;
                nowHeight = marquee.Height;
                this.marquee = marquee;
            });
        }
        public void Hide()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                marquee.MinHeight = 0;
                marquee.MinWidth = 0;
                marquee.Height = 0;
                marquee.Width = 0;
            });
        }
        public void Restore()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                marquee.MinHeight = minHeight;
                marquee.MinWidth = minWidth;
                marquee.Height = nowHeight;
                marquee.Width = nowWidth;
            });
        }
    }
}
