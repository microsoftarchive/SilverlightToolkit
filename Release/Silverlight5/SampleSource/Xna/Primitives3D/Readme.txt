This is a Silverlight port of the XNA Primitives 3D sample, which provides 
easily reusable code for drawing basic geometric primitives.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/primitives_3d

This sample provides the following geometric primitive classes:

    CubePrimitive
    SpherePrimitive
    CylinderPrimitive
    TorusPrimitive
    TeapotPrimitive

These classes are particularly useful when debugging 3D games. For 
example, you could use them to display your collision bounding boxes or 
bounding spheres as translucent or wireframe 3D models.

To reuse the code in a game of your own, just copy the .cs file for 
whichever primitive types you want to use, along with the 
GeometricPrimitive base class and VertexPositionNormal helper class. If 
you are using the TeapotPrimitive, you will also need to copy the 
BezierPrimitive base class.

To create a primitive model, add this code to your init method:

    teapot = new TeapotPrimitive(graphicsDevice);

All the primitive classes have two constructor overloads. The basic 
version (shown above) constructs a primitive with sensible default 
options, and a size of one unit across. The second constructor overload 
has more parameters, letting you specify the size of the object and (for 
everything except the cube) how many triangles to tessellate it into. This 
lets you pick your own balance between many triangles (which will create a 
nice smooth looking object but may be expensive to draw) or fewer 
triangles (which will be more efficient but not look as good).

If you are going to be creating and destroying many different primitives 
as your game progresses, you should also call the Dispose method to clean 
up the primitive model when you have finished using it.

To display a primitive model, the Draw method has two overloads. The basic 
version is shown here:

    teapot.Draw(world, view, projection, color);

The basic overload draws the primitive using a BasicEffect shader with 
default lighting, tinted to whatever color you specify. This method sets 
all the important renderstates before it draws, making sure the depth 
buffer is enabled, culling is set to the right direction, and so on, so it 
works robustly even if you aren't sure what other renderstates might be 
set at the time. If you specify a color with a fractional alpha value, 
this enables alpha blending, but if your color has solid alpha, blending 
is disabled.

The other Draw overload takes a custom effect, so you can replace the 
BasicEffect shader with whatever other rendering or lighting techniques 
you prefer. Unlike the first version, this overload does not set any 
renderstates before drawing, so you must take care to set all the states 
you need yourself, or to define these states as part of your custom effect.
