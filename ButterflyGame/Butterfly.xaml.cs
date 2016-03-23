using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ButterflyGame
{
    public sealed partial class Butterfly : UserControl
    {
        //Animate Butterfly
        private DispatcherTimer timer;

        //Offset to show
        private int currentFrame = 0;
        private int direction = 1; // 1 is direction down, -1 is up
        private int frameHeight = 132;

        //Location
        public double LocationX { get; set; }
        public double LocationY { get; set; }

        // Speed
        private readonly double MaxSpeed = 10.0;
        private readonly double Accelerate = 0.5;
        private double speed;

        //Angle
        private double Angle = 0;
        private double AngleStep = 5;

        public Butterfly()
        {
            this.InitializeComponent();

            Animate();
        }

        //Animate Buttefly
        private void Animate()
        {
            //Timer_Tick will be called in every 125 ms
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,125);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // Current frame 0->4, 4->0, 0->4
            if (direction == 1) currentFrame++;
            else currentFrame--;
            if (currentFrame == 0 || currentFrame == 4) direction *= -1; // 1 or -1 

            //Offset
            SpriteSheetOffset.Y = currentFrame * -frameHeight;
        }

        // Show Butterfly in Canvas (right location)
        public void SetLocation ()
        {
            SetValue(Canvas.LeftProperty, LocationX);
            SetValue(Canvas.TopProperty, LocationY);
        }

        // Move!!
        public void Move()
        {
            //More speed
            speed += Accelerate; // 0 -> 0.5 -> 1 > 1.5....
            if (speed > MaxSpeed) speed = MaxSpeed; // MaxSpeed 10!
            // Update location (with angle and speed)
            LocationX -= (Math.Cos(Math.PI / 180 * (Angle + 90))) * speed;
            LocationY -= (Math.Sin(Math.PI / 180 * (Angle + 90))) * speed;
        }

        // Rotate!!
        public void Rotate(int angleDirection) // 1 or -1
        {
            Angle += AngleStep * angleDirection; //5 or -5
            ButterflyRotateAngle.Angle = Angle;
        }
    }
}
