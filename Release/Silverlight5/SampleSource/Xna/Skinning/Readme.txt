This is a Silverlight port of the XNA Skinned Model sample, which shows 
how to process and render a skinned character model using the XNA 
Framework Content Pipeline.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/skinned_model

This sample uses the XNA Game Studio Content Pipeline to compile 3D models 
and textures into .xnb binary format. This means you must install XNA Game 
Studio 4.0 (in addition to Silverlight) before you can build the Content projects.
The resulting .xnb files are embedded in the Silverlight .xap package 
from where they can be loaded and drawn by the Silverlight application. Because 
XNA is only used to build this content but not to draw it, Game Studio 
only needs to be installed on your development PC. Users can run the 
resulting application with the standard Silverlight plugin, without having 
to install XNA Game Studio.

Out of the box, the XNA Framework provides only partial support for 
animation. It defines an intermediate object model for storing animation 
data inside the Content Pipeline, and can import data into this object 
model from FBX and X formats. The built-in ModelProcessor also converts 
vertex channels of BoneWeightCollection data into pairs of channels with 
VertexElementUsage BlendIndices and BlendWeight, suitable for skinned 
rendering on the GPU. The Model class, however, does not include any 
run-time animation classes. That functionality is implemented by this 
sample.

There are six projects in the sample:

    SkinnedModel contains the classes used to store and play back 
    animation data. These types are used both during the Content Pipeline 
    build process and at run time by the game, so there must be two 
    versions of this project: an XNA one for use at build time by the 
    Content Pipeline, plus a Silverlight version for use at runtime while 
    the model is being drawn. Both versions of this project share the same 
    code, so there are two .csproj files that link to the same .cs source 
    files.

    SkinnedModelPipeline is an XNA Content Pipeline assembly. It 
    implements a new content processor for building skinned models.

    Skinning is a sample Silverlight application showing how to animate 
    and render a skinned model.

	Content and ContentLibrary are used to compile the animating dude.fbx 
	model into binary .xnb format.

To render a skinned model, we need two things: an effect that will 
implement the mesh deformation, plus some animation data describing how 
the mesh should be deformed. The SkinnedModelProcessor attaches all this 
information to the existing Model type provided by the XNA Framework. The 
SkinnedModelProcessor.DefaultEffect property overload specifies that the 
built model should always use SkinnedEffect rather than the default 
BasicEffect shader. Animation data is converted into a suitable run-time 
format by the SkinnedModelProcessor.ProcessAnimations method, which stores 
a custom SkinningData object in the Tag property of the framework Model 
instance.
