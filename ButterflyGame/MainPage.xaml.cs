using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButterflyGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Butterfly
        private Butterfly butterfly;

        //Flowers
        private List<Flower> flowers = new List<Flower>();

        //Audio
        private MediaElement mediaElement;

        // Game loop timer
        private DispatcherTimer timer;

        // Which keys are pressed
        private bool UpPressed; // true, false
        private bool RightPressed;
        private bool LeftPressed;  

        public MainPage()
        {
            this.InitializeComponent();

            //try open 800x600 window
            ApplicationView.PreferredLaunchWindowingMode
                = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(800,600);

            //Key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;
            
            
            //Create Objects..
            CreateButterfly();
            StartGame();
            InitAudio();

        }

        //Load audio from assests
        private async void InitAudio()
        {
            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file = await folder.GetFileAsync("tada.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            mediaElement.SetSource(stream, file.ContentType);

        }

        //Flowers
        private void CoreWindow_PointerPressed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            //Create new Flower
            Flower flower = new Flower();
            flower.LocationX = args.CurrentPoint.Position.X - flower.Width / 2;
            flower.LocationY = args.CurrentPoint.Position.Y - flower.Height / 2;
            //Add to Canvas
            MyCanvas.Children.Add(flower);
            flower.SetLocation();
            //Add flower to List
            flowers.Add(flower);
        }

        //KeyDOWN
        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = true;
                    break;
                case VirtualKey.Left:
                    LeftPressed = true;
                    break;
                case VirtualKey.Right:
                    RightPressed = true;
                    break;
                default:
                    break;
            }
        }
        //KeyUP
        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    UpPressed = false;
                    break;
                case VirtualKey.Left:
                    LeftPressed = false;
                    break;
                case VirtualKey.Right:
                    RightPressed = false;
                    break;
                default:
                    break;
            }
        }

        // Create Butterfly
        private void CreateButterfly()
        {
            butterfly = new Butterfly
            {
                LocationX = MyCanvas.Width / 2 - 75,
                LocationY = MyCanvas.Height / 2 - 66
            };
            // Add Butterfly to Canvas
            MyCanvas.Children.Add(butterfly);
            //Show in right location
            butterfly.SetLocation();
        }

        //Start game loop
        private void StartGame()
        {
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,1000/60); //60fps
            timer.Start(); // Stop method similar way timer.Stop();!!
        }

        // Game loop
        private void Timer_Tick(object sender, object e)
        {
            // Move Butterfly
            if (UpPressed) butterfly.Move();
            // Rotate Butterfly
            //Debug.WriteLine(LeftPressed); // DEBUGGAUS
            if (LeftPressed) butterfly.Rotate(-1);
            if (RightPressed) butterfly.Rotate(1);
            // Update Butterfly location
            butterfly.SetLocation();
            // Collision Butterfly with flowers
            CheckCollission();

        }
        // Check collision with flowers and butterfly
        private void CheckCollission()
        {
            foreach(Flower flower in flowers)
            {
                //Get rects
                Rect r1 = new Rect(butterfly.LocationX,butterfly.LocationY, butterfly.ActualWidth,butterfly.ActualHeight); //Butterfly
                Rect r2 = new Rect(flower.LocationX, flower.LocationY, flower.ActualWidth, butterfly.ActualHeight); //Flower
                // Does intersects happend..? 
                r1.Intersect(r2);
                //If not empty
                if (!r1.IsEmpty)
                {
                    //Play audio
                    mediaElement.Play();
                    // Remove flower
                    MyCanvas.Children.Remove(flower);
                    flowers.Remove(flower);
                    break; // end looping..
                }
            }
        }
    }
}
