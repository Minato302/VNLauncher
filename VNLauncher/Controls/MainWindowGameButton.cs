#pragma warning disable IDE0049
#pragma warning disable CS8618


using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowGameButton : Button
    {

        static MainWindowGameButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowGameButton), new FrameworkPropertyMetadata(typeof(MainWindowGameButton)));
        }
        private FileManager fileManager;
        private LocalColorAcquirer resource;
        private Image iconImage;
        private TextBlock gameNameTextBlock;

        private Game game;
        public Game Game => game;

        private static MainWindowGameButton? selectedGameButton;
        public static MainWindowGameButton? SelectedGameButton => selectedGameButton;

        private Border mainBorder;
        private Action action;

        public MainWindowGameButton(String gameName)
        {
            
            fileManager = new FileManager();
            game = new Game(gameName, fileManager);

            resource = new LocalColorAcquirer();
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                BeingSelected();
            };
            MouseEnter += (sender, e) =>
            {
                if (this != selectedGameButton)
                {
                    mainBorder!.Background = resource.GetColor("mainWindowGameButtonColor_MouseEnter");
                }
            };
            MouseLeave += (sender, e) =>
            {
                if (this != selectedGameButton)
                {
                    mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
            };

        }
        public void BeingSelected()
        {
            ApplyTemplate();

            selectedGameButton = this;
            action?.Invoke();
            mainBorder!.Background = resource.GetColor("mainWindowGameButtonColor_Selected");
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            iconImage = (Template.FindName("iconImage", this) as Image)!;
            gameNameTextBlock = (Template.FindName("gameNameTextBlock", this) as TextBlock)!;
            iconImage.Source = ImageHandler.GetImage((fileManager.GetGameIconPath(game.Name)!));         
         
            gameNameTextBlock.Text = game.Name ;
        }
        public void SetSelectedAction(Action action)
        {
            ApplyTemplate();
            this.action = action;
        }


        public void RelieveSelected()
        {
            ApplyTemplate();
            mainBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
    }
}
