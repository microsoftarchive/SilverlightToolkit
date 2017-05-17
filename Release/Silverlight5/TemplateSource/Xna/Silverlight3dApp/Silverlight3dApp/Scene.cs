using System.Windows.Controls;
using System;
using System.Windows.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Silverlight3dApp
{
    public class Scene : IDisposable
    {
        #region Fields

        readonly DrawingSurface _drawingSurface;
        readonly ContentManager contentManager;
        readonly Cube cube;

        float aspectRatio;
        float rotationAngle;

        #endregion

        #region Properties

        public ContentManager ContentManager
        {
            get
            {
                return contentManager;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return GraphicsDeviceManager.Current.GraphicsDevice;
            }
        }

        #endregion

        #region Creation

        public Scene(DrawingSurface drawingSurface)
        {
            _drawingSurface = drawingSurface;

            // Register for size changed to update the aspect ratio
            _drawingSurface.SizeChanged += _drawingSurface_SizeChanged;

            // Get a content manager to access content pipeline
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            // Initializing variables
            cube = new Cube(this, 1.0f);
        }

        #endregion

        #region Methods

        void _drawingSurface_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            aspectRatio = (float)(_drawingSurface.ActualWidth / _drawingSurface.ActualHeight);
        }

        public void Draw()
        {
            // Clear surface
            GraphicsDeviceManager.Current.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, new Color(0.2f, 0.2f, 0.2f, 1.0f), 1.0f, 0);

            // Compute matrices
            Matrix world = Matrix.CreateRotationX(rotationAngle) * Matrix.CreateRotationY(rotationAngle);
            Matrix view = Matrix.CreateLookAt(new Vector3(0, 0, -5.0f), Vector3.Zero, Vector3.UnitY);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(0.85f, aspectRatio, 0.01f, 1000.0f);

            // Update per frame parameter values
            cube.World = world;
            cube.WorldViewProjection = world * view * projection;            
            
            // Drawing the cube
            cube.Draw();            

            // Animate rotation
            rotationAngle += 0.05f;
        }

        public void Dispose()
        {
            _drawingSurface.SizeChanged -= _drawingSurface_SizeChanged;
        }


        #endregion
    }
}
