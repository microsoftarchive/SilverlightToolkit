using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Bloom
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();

            SettingsCombo.ItemsSource = BloomSettings.PresetSettings;
        }


        ContentManager contentManager;
        SpriteBatch spriteBatch;
        Texture2D background;
        Model model;

        BloomComponent bloom = new BloomComponent();

        volatile bool bloomEnabled = true;


        void OnBloomEnabled(object sender, RoutedEventArgs e)
        {
            bloomEnabled = true;
        }


        void OnBloomDisabled(object sender, RoutedEventArgs e)
        {
            bloomEnabled = false;
        }


        void OnSettingsChanged(object sender, SelectionChangedEventArgs e)
        {
            int selection = ((ComboBox)sender).SelectedIndex;

            bloom.Settings = BloomSettings.PresetSettings[selection];
        }


        void OnIntermediateChanged(object sender, SelectionChangedEventArgs e)
        {
            int selection = ((ComboBox)sender).SelectedIndex;

            bloom.ShowBuffer = (BloomComponent.IntermediateBuffer)selection;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            spriteBatch = new SpriteBatch(graphicsDevice);
            background = contentManager.Load<Texture2D>("sunset");
            model = contentManager.Load<Model>("tank");
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            bool isEnabled = bloomEnabled;

            if (isEnabled)
            {
                bloom.BeginDraw(graphicsDevice);
            }

            graphicsDevice.Clear(Color.Black);

            // Draw the background image.
            spriteBatch.Begin(0, BlendState.Opaque);

            spriteBatch.Draw(background,
                             new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height),
                             Color.White);

            spriteBatch.End();

            // Draw the spinning model.
            graphicsDevice.DepthStencilState = DepthStencilState.Default;

            DrawModel(graphicsDevice, e.TotalTime);

            if (isEnabled)
            {
                bloom.EndDraw(graphicsDevice);
            }

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }


        /// <summary>
        /// Helper for drawing the spinning 3D model.
        /// </summary>
        void DrawModel(GraphicsDevice graphicsDevice, TimeSpan totalGameTime)
        {
            float time = (float)totalGameTime.TotalSeconds;

            // Create camera matrices.
            Matrix world = Matrix.CreateRotationY(time * 0.42f);

            Matrix view = Matrix.CreateLookAt(new Vector3(750, 100, 0),
                                              new Vector3(0, 300, 0),
                                              Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, graphicsDevice.Viewport.AspectRatio,
                                                                    1, 10000);

            // Look up the bone transform matrices.
            Matrix[] transforms = new Matrix[model.Bones.Count];

            model.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model.
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = transforms[mesh.ParentBone.Index] * world;
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    // Override the default specular color to make it nice and bright,
                    // so we'll get some decent glints that the bloom can key off.
                    effect.SpecularColor = Vector3.One;
                }

                mesh.Draw();
            }
        }
    }
}
