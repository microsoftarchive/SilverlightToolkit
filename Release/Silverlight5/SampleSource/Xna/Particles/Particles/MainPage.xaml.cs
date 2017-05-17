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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Particles
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        // The particle systems will all need a SpriteBatch to draw their particles,
        // so let's make one they can share. We'll use this to draw our SpriteFont
        // too.
        SpriteBatch spriteBatch;

        ContentManager contentManager;


        // Used to draw the instructions on the screen.
        SpriteFont font;

        
        // Here's the really fun part of the sample, the particle systems! These are
        // drawable game components, so we can just add them to the components
        // collection. Read more about each particle system in their respective source
        // files.
        ExplosionParticleSystem explosion;
        ExplosionSmokeParticleSystem smoke;
        SmokePlumeParticleSystem smokePlume;


        // State is an enum that represents which effect we're currently demoing.
        enum State
        {
            Explosions,
            SmokePlume
        };

        State currentState = State.Explosions;

        // a timer that will tell us when it's time to trigger another explosion.
        const float TimeBetweenExplosions = 2.0f;
        float timeTillExplosion = 0.0f;

        // keep a timer that will tell us when it's time to add more particles to the
        // smoke plume.
        const float TimeBetweenSmokePlumePuffs = .5f;
        float timeTillPuff = 0.0f;


        private void OnLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            currentState = (currentState == State.Explosions) ? State.SmokePlume : State.Explosions;
        }


        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            spriteBatch = new SpriteBatch(graphicsDevice);

            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            // we should never see more than one explosion at once
            explosion = new ExplosionParticleSystem(contentManager, spriteBatch, 1);

            // but the smoke from the explosion lingers a while.
            smoke = new ExplosionSmokeParticleSystem(contentManager, spriteBatch, 2);

            // we'll see lots of these effects at once; this is ok
            // because they have a fairly small number of particles per effect.
            smokePlume = new SmokePlumeParticleSystem(contentManager, spriteBatch, 9);

            font = contentManager.Load<SpriteFont>("font");
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            // Update the particle animation.
            explosion.Update(e.DeltaTime);
            smoke.Update(e.DeltaTime);
            smokePlume.Update(e.DeltaTime);

            float dt = (float)e.DeltaTime.TotalSeconds;

            switch (currentState)
            {
                // if we should be demoing the explosions effect, check to see if it's
                // time for a new explosion.
                case State.Explosions:
                    UpdateExplosions(dt, graphicsDevice.Viewport);
                    break;

                // if we're showing off the smoke plume, check to see if it's time for a
                // new puff of smoke.
                case State.SmokePlume:
                    UpdateSmokePlume(dt, graphicsDevice.Viewport);
                    break;
            }

            // Draw the particle systems.
            graphicsDevice.Clear(Color.Black);

            smoke.Draw();
            smokePlume.Draw();
            explosion.Draw();

            spriteBatch.Begin();

            // draw some instructions on the screen
            string message = string.Format("Current effect: {0}!\n" +
                "Click the left mouse button to change\n\n" +
                "Free particles:\n" +
                "    ExplosionParticleSystem: {1}\n" +
                "    ExplosionSmokeParticleSystem: {2}\n" +
                "    SmokePlumeParticleSystem: {3}",
                currentState, explosion.FreeParticleCount,
                smoke.FreeParticleCount, smokePlume.FreeParticleCount);

            spriteBatch.DrawString(font, message, new Vector2(50, 50), Color.White);

            spriteBatch.End();

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }


        // this function is called when we want to demo the smoke plume effect. it
        // updates the timeTillPuff timer, and adds more particles to the plume when
        // necessary.
        private void UpdateSmokePlume(float dt, Viewport viewport)
        {
            timeTillPuff -= dt;
            if (timeTillPuff < 0)
            {
                Vector2 where = Vector2.Zero;
                // add more particles at the bottom of the screen, halfway across.
                where.X = viewport.Width / 2;
                where.Y = viewport.Height;
                smokePlume.AddParticles(where);

                // and then reset the timer.
                timeTillPuff = TimeBetweenSmokePlumePuffs;
            }
        }


        // this function is called when we want to demo the explosion effect. it
        // updates the timeTillExplosion timer, and starts another explosion effect
        // when the timer reaches zero.
        private void UpdateExplosions(float dt, Viewport viewport)
        {
            timeTillExplosion -= dt;
            if (timeTillExplosion < 0)
            {
                Vector2 where = Vector2.Zero;
                // create the explosion at some random point on the screen.
                where.X = ParticleSystem.RandomBetween(0, viewport.Width);
                where.Y = ParticleSystem.RandomBetween(0, viewport.Height);

                // the overall explosion effect is actually comprised of two particle
                // systems: the fiery bit, and the smoke behind it. add particles to
                // both of those systems.
                explosion.AddParticles(where);
                smoke.AddParticles(where);

                // reset the timer.
                timeTillExplosion = TimeBetweenExplosions;
            }
        }
    }
}
