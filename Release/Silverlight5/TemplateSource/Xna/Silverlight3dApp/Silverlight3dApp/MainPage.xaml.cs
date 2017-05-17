using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows;

namespace Silverlight3dApp
{
    public partial class MainPage
    {
        Scene scene;

        public MainPage()
        {
            InitializeComponent();
        }

        private void myDrawingSurface_Draw(object sender, DrawEventArgs e)
        {
            // Render scene
            scene.Draw();

            // Let's go for another turn!
            e.InvalidateSurface();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Check if GPU is on
            if (GraphicsDeviceManager.Current.RenderMode != RenderMode.Hardware)
            {
                MessageBox.Show("Please activate enableGPUAcceleration=true on your Silverlight plugin page.", "Warning", MessageBoxButton.OK);
            }

            // Create the scene
            scene = new Scene(myDrawingSurface);
        }
    }
}
