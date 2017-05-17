using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Windows.Graphics;

namespace SimpleAnimation
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        ContentManager contentManager;

        Tank tank;


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            tank = new Tank();

            tank.Load(contentManager);
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            float time = (float)e.TotalTime.TotalSeconds;

            // Update the animation properties on the tank object. In a real game
            // you would probably take this data from user inputs or the physics
            // system, rather than just making everything rotate like this!

            tank.WheelRotation = time * 5;
            tank.SteerRotation = (float)Math.Sin(time * 0.75f) * 0.5f;
            tank.TurretRotation = (float)Math.Sin(time * 0.333f) * 1.25f;
            tank.CannonRotation = (float)Math.Sin(time * 0.25f) * 0.333f - 0.333f;
            tank.HatchRotation = MathHelper.Clamp((float)Math.Sin(time * 2) * 2, -1, 0);

            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            Color darkGray = new Color(0xA9, 0xA9, 0xA9, 0xFF);

            graphicsDevice.Clear(darkGray);

            // Calculate the camera matrices.
            Matrix rotation = Matrix.CreateRotationY(time * 0.1f);

            Matrix view = Matrix.CreateLookAt(new Vector3(1000, 500, 0),
                                              new Vector3(0, 150, 0),
                                              Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    graphicsDevice.Viewport.AspectRatio,
                                                                    10,
                                                                    10000);

            // Draw the tank model.
            tank.Draw(rotation, view, projection);

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }
    }
}
