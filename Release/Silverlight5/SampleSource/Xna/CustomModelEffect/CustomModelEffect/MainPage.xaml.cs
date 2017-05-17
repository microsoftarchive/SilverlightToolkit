using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace CustomModelEffect
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        ContentManager contentManager;
        Model model;

        Vector3 specularColor = new Vector3(1, 1, 0);

        float currentEnvMap;
        float currentSpecular;
        float currentFresnel;


        void OnEnvMapChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentEnvMap = (float)e.NewValue;
        }


        void OnSpecularChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentSpecular = (float)e.NewValue;
        }


        void OnFresnelChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            currentFresnel = (float)e.NewValue;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            model = contentManager.Load<Model>("saucer");
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            Color cornflowerBlue = new Color(0x64, 0x95, 0xED, 0xFF);

            graphicsDevice.Clear(cornflowerBlue);

            // Calculate the camera matrices.
            float time = (float)e.TotalTime.TotalSeconds;

            Matrix world = Matrix.CreateRotationX(time * 0.3f) *
                           Matrix.CreateRotationY(time);

            Matrix view = Matrix.CreateLookAt(new Vector3(4000, 0, 0),
                                              Vector3.Zero,
                                              Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    graphicsDevice.Viewport.AspectRatio,
                                                                    10, 10000);

            // Configure effect parameters.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (EnvironmentMapEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.EnvironmentMapAmount = currentEnvMap;
                    effect.EnvironmentMapSpecular = specularColor * currentSpecular;
                    effect.FresnelFactor = currentFresnel;
                }
            }

            // Draw the model.
            model.Draw(world, view, projection);

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }
    }
}
