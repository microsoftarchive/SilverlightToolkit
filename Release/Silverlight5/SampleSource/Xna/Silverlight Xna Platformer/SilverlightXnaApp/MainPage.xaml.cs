using System.Windows.Controls;
using System.Windows.Graphics;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace SilverlightXnaApp
{
    public partial class MainPage
    {
        PlatformerGame platformerGame;

        public MainPage()
        {
            InitializeComponent();

            Mouse.RootControl = this;
            Keyboard.RootControl = this;
        }

        private void myDrawingSurface_Draw(object sender, DrawEventArgs e)
        {
            // Simulate Xna render loop
            platformerGame.Update(e); // We make a copy of keys to prevent external updates of the collection
            platformerGame.Draw(e);

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

            // Compute good tile size
            Tile.Width = (int) (myDrawingSurface.Width / Tile.XCount);
            Tile.Height = (int) (myDrawingSurface.Height / Tile.YCount);
            Tile.Size = new Vector2(Tile.Width, Tile.Height);

            // Create game
            platformerGame = new PlatformerGame();
        }
    }
}
