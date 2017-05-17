This is a Silverlight port of the XNA Custom Model Effect sample, which 
shows how custom effects can be applied to a model using the XNA Framework 
Content Pipeline.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/custom_model_effect

This sample uses the XNA Game Studio Content Pipeline to compile 3D models 
and textures into .xnb binary format. This means you must install XNA Game 
Studio 4.0 (in addition to Silverlight) before you can build the Content projects. 
The resulting .xnb files are embedded in the Silverlight .xap 
package from where they can be loaded and drawn by the Silverlight application. 
Because XNA is only used to build this content but not to draw it, Game 
Studio only needs to be installed on your development PC. Users can run 
the resulting application with the standard Silverlight plugin, without 
having to install XNA Game Studio.

This sample demonstrates how to use EnvironmentMapEffect to render a model 
with a static environment cube map, creating a shiny, reflective surface. 
The sample uses two custom content pipeline processors. The first 
processor applies the environment mapping effect to the model during the 
content build process. The run-time code does not then have to do anything 
special to render using the custom effect because the model will be loaded 
automatically with the new effect set up and ready to use. The second 
custom content pipeline processor converts a regular 2D photograph into a 
cube map that can be used for the environment mapping.

The EnvironmentMappedModelProcessor derives from the built-in 
ModelProcessor, and overrides the ConvertMaterial method to pass all the 
materials on the model through to the custom 
EnvironmentMappedMaterialProcessor.

EnvironmentMappedMaterialProcessor derives from the built-in 
MaterialProcessor, and overrides the Process and BuildTexture methods. 
Inside the Process method, it creates a new instance of 
EnvironmentMapMaterialContent, then sets this to reference our chosen 
environment map texture. The base texture setting is copied across from 
the original material, before it chains to the base 
MaterialProcessor.Process.

The overload of BuildTexture checks which texture is being built. The 
environment map texture is passed to the custom CubemapProcessor, while 
all other textures go to the default base class implementation.

The CubemapProcessor converts arbitrary 2D photographs into static 
reflection cube maps. It does this by mirroring the image to make it tile 
seamlessly from left to right, wrapping the middle portion of it around 
the side of the cube, folding up the top and bottom to fill the top and 
bottom cube map faces, and then applying a blur to cover up the 
discontinuities at the very top and bottom. This technique provides a 
quick and easy way to turn any image into an environment map. For example, 
you could take a screen shot of your game and use it to reflect "typical" 
game surroundings in your shiny objects.
