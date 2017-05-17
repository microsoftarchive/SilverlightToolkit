using System;
using System.Windows;
using System.Windows.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Content;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace SilverlightXnaApp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerGame
    {
        readonly SpriteBatch spriteBatch;
        readonly ContentManager contentManager;

        // Global content.
        private readonly SpriteFont hudFont;

        private readonly Texture2D winOverlay;
        private readonly Texture2D loseOverlay;
        private readonly Texture2D diedOverlay;

        // Meta-level game state.
        private int levelIndex = -1;
        private Level level;
        
        // When the time remaining is less than the warning time, it blinks on the hud
        private static readonly TimeSpan WarningTime = TimeSpan.FromSeconds(30);

        // Music
        readonly SoundEffect backgroundMusic;
        readonly SoundEffectInstance backgroundMusicInstance;

        public PlatformerGame()
        {
            contentManager = new ContentManager(null, "Content");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDeviceManager.Current.GraphicsDevice);

            // Load fonts
            hudFont = contentManager.Load<SpriteFont>("Fonts/Hud");

            // Load overlay textures
            winOverlay = contentManager.Load<Texture2D>("Overlays/you_win");
            loseOverlay = contentManager.Load<Texture2D>("Overlays/you_lose");
            diedOverlay = contentManager.Load<Texture2D>("Overlays/you_died");

            LoadNextLevel();

            // Music
            backgroundMusic = contentManager.Load<SoundEffect>("Sounds/Music");
            backgroundMusicInstance = backgroundMusic.CreateInstance();
            backgroundMusicInstance.IsLooped = true;
            backgroundMusicInstance.Play();
        }

        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Levels/{0}.txt", ++levelIndex);

                bool fileFound;
                try
                {
                    using (new StreamReader(Application.GetResourceStream(new Uri(levelPath, UriKind.Relative)).Stream))
                    {
                        
                    }
                    fileFound = true;
                }
                catch
                {
                    fileFound = false;
                }

                if (fileFound)
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");

                // Whenever we can't find a level, start over again at 0.
                levelIndex = -1;
            }

            // Unloads the content for the current level before loading the next one.
            if (level != null)
                level.Dispose();

            // Load the level.
            level = new Level(levelPath);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }


        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        public void Update(DrawEventArgs drawEventArgs)
        {
            HandleInput();
            level.Update(drawEventArgs);
        }

        void HandleInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            level.Player.HandleInput(keyboardState);

            if (keyboardState.IsKeyDown(Key.Space) || keyboardState.IsKeyDown(Key.Up))
            {
                if (!level.Player.IsAlive)
                {
                    level.StartNewLife();
                }
                else if (level.TimeRemaining == TimeSpan.Zero)
                {
                    if (level.ReachedExit)
                        LoadNextLevel();
                    else
                        ReloadCurrentLevel();
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        public void Draw(DrawEventArgs drawEventArgs)
        {
            GraphicsDeviceManager.Current.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            level.Draw(drawEventArgs, spriteBatch);

            DrawHud();

            spriteBatch.End();
        }

        private void DrawHud()
        {
            Rectangle titleSafeArea = GraphicsDeviceManager.Current.GraphicsDevice.Viewport.Bounds;
            Vector2 hudLocation = new Vector2(titleSafeArea.X, titleSafeArea.Y);
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f,
                                         titleSafeArea.Y + titleSafeArea.Height / 2.0f);

            // Draw time remaining. Uses modulo division to cause blinking when the
            // player is running out of time.
            string timeString = "TIME: " + level.TimeRemaining.Minutes.ToString("00") + ":" + level.TimeRemaining.Seconds.ToString("00");
            Color timeColor;
            if (level.TimeRemaining > WarningTime ||
                level.ReachedExit ||
                (int)level.TimeRemaining.TotalSeconds % 2 == 0)
            {
                timeColor = new Color(1.0f, 1.0f, 0);
            }
            else
            {
                timeColor = new Color(1.0f, 0, 0);
            }
            DrawShadowedString(hudFont, timeString, hudLocation, timeColor);

            // Draw score
            float timeHeight = hudFont.MeasureString(timeString).Y;
            DrawShadowedString(hudFont, "SCORE: " + level.Score.ToString(), hudLocation + new Vector2(0.0f, timeHeight * 1.2f), new Color(1.0f, 1.0f, 0));

            // Determine the status overlay message to show.
            Texture2D status = null;
            if (level.TimeRemaining == TimeSpan.Zero)
            {
                status = level.ReachedExit ? winOverlay : loseOverlay;
            }
            else if (!level.Player.IsAlive)
            {
                status = diedOverlay;
            }

            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
        }

        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
