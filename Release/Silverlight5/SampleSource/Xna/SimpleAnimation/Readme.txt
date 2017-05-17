This is a Silverlight port of the XNA Simple Animation sample, which shows 
how to how to apply program controlled rigid body animation to a 3D model.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/simple_animation

This sample uses the XNA Game Studio Content Pipeline to compile 3D models 
and textures into .xnb binary format. This means you must install XNA Game 
Studio 4.0 (in addition to Silverlight) before you can build the Content projects.
The resulting .xnb files are embedded in the Silverlight .xap 
package from where they can be loaded and drawn by the Silverlight application. 
Because XNA is only used to build this content but not to draw it, Game 
Studio only needs to be installed on your development PC. Users can run 
the resulting application with the standard Silverlight plugin, without 
having to install XNA Game Studio.

This sample begins by loading a tank model. The tank is built of a 
hierarchy of separate meshes. The wheels, the turret, the hatch, and so 
on, are all separate meshes. Inside the 3D modeling package, each mesh has 
a descriptive name. Each is attached to the appropriate parent mesh with a 
suitable pivot point. For example, the cannon mesh can tilt up and down. 
It is attached to the turret mesh, which can also swivel from left to 
right, and is itself attached to the body of the tank. Because of this 
hierarchy, when you rotate the turret, the cannon rotates along with it.
 
In the XNA Framework, the Model class represents the whole model. The 
Model contains a ModelMesh for each separate mesh in the model. Each 
ModelMesh contains a ParentBone (the pivot point mentioned earlier), which 
controls the mesh's position and orientation relative to the model. The 
Model has a Root bone, which determines the model's position and 
orientation. Every ModelBone can have one parent and many children. The 
Root bone on the Model object is the ultimate parent; its children are 
bones on ModelMesh objects, which might have other ModelMesh bones as 
their children, and so on. In the example above, the body of the tank 
would have a ParentBone, with a child bone for the turret, which itself 
has a child bone for the cannon. In any given family of bones, rotating 
the parent bone also rotates the children, and their children, and so on.

Every bone has a transformation matrix (called Transform) that defines its 
position and rotation relative to the position of the parent bone. This 
rotation and translation applies to all the vertices in the ModelMesh (for 
example, all the vertices that connect to that bone). To animate a bone, 
you multiply the default bone transform by a new matrix. When you draw the 
ModelMesh, you then base your world matrix on the bone's transform.

The easiest way to incorporate transformed bones into drawing is to use 
the CopyAbsoluteBoneTransformsTo method. This method takes the bone 
transforms, which are relative to each other, and iterates over them to 
make them relative to the Root bone of the Model. It then returns a copy 
of these transforms. When you draw each ModelMesh, you can use the 
absolute bone transform as the first part of your world matrix. This way 
you won't have to worry about parent bones and their relationships.

The Tank class is a wrapper around the XNA Framework Model type. It 
exposes properties for each value that can be used to animate the tank. In 
the Load method, it uses the ContentManager to load the underlying Model 
data. Then it saves shortcut references to the bones that are going to be 
animated, and stores their original transform matrices. In the Draw 
method, it computes new rotation matrices based on new rotation values 
from the game. These new matrices are assigned to the Transform properties 
of the animated bones, after which the model is rendered in the usual way 
by using CopyAbsoluteBoneTransformsTo.

The MainPage.xaml.cs OnDraw method uses the current time value to update 
the animation properties on the Tank object. The wheels are rotated, while 
the other values are moved back and forth in sine curves of varying 
speeds. In a real game, you would take this data from user inputs or the 
physics system rather than just making everything oscillate. 
