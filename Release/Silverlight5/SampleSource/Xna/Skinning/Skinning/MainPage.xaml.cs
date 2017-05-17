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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SkinnedModel;

namespace Skinning
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }


        ContentManager contentManager;

        Model currentModel;
        AnimationPlayer animationPlayer;

        float cameraArc = 0;
        float cameraRotation = 0;
        float cameraDistance = 100;

        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            contentManager = new ContentManager(null)
            {
                RootDirectory = "Content"
            };

            // Load the model.
            currentModel = contentManager.Load<Model>("dude");

            // Look up our custom skinning information.
            SkinningData skinningData = currentModel.Tag as SkinningData;

            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);

            AnimationClip clip = skinningData.AnimationClips["Take 001"];

            animationPlayer.StartClip(clip);
        }


        private void OnDraw(object sender, DrawEventArgs e)
        {
            GraphicsDevice graphicsDevice = GraphicsDeviceManager.Current.GraphicsDevice;

            // Update the skinned animation player.
            animationPlayer.Update(e.DeltaTime, true, Matrix.Identity);

            Matrix[] bones = animationPlayer.GetSkinTransforms();

            // Compute camera matrices.
            cameraRotation += (float)e.DeltaTime.TotalSeconds * 10;

            Matrix view = Matrix.CreateTranslation(0, -40, 0) *
                          Matrix.CreateRotationY(MathHelper.ToRadians(cameraRotation)) *
                          Matrix.CreateRotationX(MathHelper.ToRadians(cameraArc)) *
                          Matrix.CreateLookAt(new Vector3(0, 0, -cameraDistance),
                                              new Vector3(0, 0, 0), Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    graphicsDevice.Viewport.AspectRatio,
                                                                    1,
                                                                    10000);

            Color cornflowerBlue = new Color(0x64, 0x95, 0xED, 0xFF);

            graphicsDevice.Clear(cornflowerBlue);

            // Render the skinned mesh.
            foreach (ModelMesh mesh in currentModel.Meshes)
            {
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    effect.SetBoneTransforms(bones);

                    effect.View = view;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();

                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }

                mesh.Draw();
            }

            // Force a redraw so this control will continuously animate.
            e.InvalidateSurface();
        }
    }
}
