using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Primitives3D
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        // Store a list of primitive models, plus which one is currently selected.
        List<GeometricPrimitive> primitives = new List<GeometricPrimitive>();


        // Store a list of selectable colors.
        static Color[] colors =
        {
            new Color(255, 0, 0, 255),
            new Color(0, 255, 0, 255),
            new Color(0, 0, 255, 255),
            new Color(255, 255, 255, 255),
            new Color(0, 0, 0, 255),
        };


        // Custom rasterizer state for rendering in wireframe mode.
        static RasterizerState wireFrameState = new RasterizerState()
        {
            FillMode = FillMode.WireFrame,
            CullMode = CullMode.None,
        };


        // Store the current UI selections.
        int currentPrimitiveIndex;
        int currentColorIndex;
        bool isWireframe;


        private void OnPrimitiveChanged(object sender, SelectionChangedEventArgs e)
        {
            currentPrimitiveIndex = ((ComboBox)sender).SelectedIndex;
        }


        private void OnColorChanged(object sender, SelectionChangedEventArgs e)
        {
            currentColorIndex = ((ComboBox)sender).SelectedIndex;
        }


        private void OnWireframeChanged(object sender, RoutedEventArgs e)
        {
            isWireframe = ((CheckBox)sender).IsChecked == true;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            primitives.Add(new CubePrimitive(graphicsDevice));
            primitives.Add(new SpherePrimitive(graphicsDevice));
            primitives.Add(new CylinderPrimitive(graphicsDevice));
            primitives.Add(new TorusPrimitive(graphicsDevice));
            primitives.Add(new TeapotPrimitive(graphicsDevice));
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            Color cornflowerBlue = new Color(0x64, 0x95, 0xED, 0xFF);

            graphicsDevice.Clear(cornflowerBlue);

            if (isWireframe)
            {
                graphicsDevice.RasterizerState = wireFrameState;
            }
            else
            {
                graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            }

            // Create camera matrices, making the object spin.
            float time = (float)e.TotalTime.TotalSeconds;

            float yaw = time * 0.4f;
            float pitch = time * 0.7f;
            float roll = time * 1.1f;

            Vector3 cameraPosition = new Vector3(0, 0, 2.5f);

            float aspect = graphicsDevice.Viewport.AspectRatio;

            Matrix world = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            Matrix view = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(1, aspect, 1, 10);

            // Draw the current primitive.
            GeometricPrimitive currentPrimitive = primitives[currentPrimitiveIndex];
            Color currentColor = colors[currentColorIndex];

            currentPrimitive.Draw(world, view, projection, currentColor);

            // Reset the fill mode renderstate.
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }
    }
}
