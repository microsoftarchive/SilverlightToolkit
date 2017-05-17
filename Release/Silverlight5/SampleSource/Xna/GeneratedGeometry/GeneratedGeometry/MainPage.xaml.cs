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

namespace GeneratedGeometry
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        ContentManager contentManager;
        Model terrain;
        Sky sky;

        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            terrain = contentManager.Load<Model>("terrain");
            sky = contentManager.Load<Sky>("sky");
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            graphicsDevice.Clear(Color.Black);

            // Calculate the projection matrix.
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    graphicsDevice.Viewport.AspectRatio,
                                                                    1, 10000);

            // Calculate a view matrix, moving the camera around a circle.
            float time = (float)e.TotalTime.TotalSeconds * 0.333f;

            float cameraX = (float)Math.Cos(time);
            float cameraY = (float)Math.Sin(time);

            Vector3 cameraPosition = new Vector3(cameraX, 0, cameraY) * 64;
            Vector3 cameraFront = new Vector3(-cameraY, 0, cameraX);

            Matrix view = Matrix.CreateLookAt(cameraPosition,
                                              cameraPosition + cameraFront,
                                              Vector3.Up);

            // Draw the terrain first, then the sky. This is faster than
            // drawing the sky first, because the depth buffer can skip
            // bothering to draw sky pixels that are covered up by the
            // terrain. This trick works because the code used to draw
            // the sky forces all the sky vertices to be as far away as
            // possible, and turns depth testing on but depth writes off.

            DrawTerrain(view, projection);

            sky.Draw(graphicsDevice, view, projection);

            // If there was any alpha blended translucent geometry in
            // the scene, that would be drawn here, after the sky.

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }


        /// <summary>
        /// Helper for drawing the terrain model.
        /// </summary>
        void DrawTerrain(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in terrain.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    // Set the specular lighting to match the sky color.
                    effect.SpecularColor = new Vector3(0.6f, 0.4f, 0.2f);
                    effect.SpecularPower = 8;

                    // Set the fog to match the distant mountains
                    // that are drawn into the sky texture.
                    effect.FogEnabled = true;
                    effect.FogColor = new Vector3(0.15f);
                    effect.FogStart = 100;
                    effect.FogEnd = 320;
                }

                mesh.Draw();
            }
        }
    }
}
