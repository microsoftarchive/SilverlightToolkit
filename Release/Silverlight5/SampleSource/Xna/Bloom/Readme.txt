This is a Silverlight port of the XNA Bloom Postprocess sample, which 
shows how to use bloom post-processing filters to add a glow effect over 
the top of an existing scene.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/bloom

This sample uses the XNA Game Studio Content Pipeline to compile 3D models 
and textures into .xnb binary format. This means you must install XNA Game 
Studio 4.0 (in addition to Silverlight) before you can build the Content projects. 
The resulting .xnb files are embedded in the Silverlight .xap package 
from where they can be loaded and drawn by the 
Silverlight application. Because XNA is only used to build this content 
but not to draw it, Game Studio only needs to be installed on your development PC. 
Users can run the resulting application with the standard Silverlight plugin, 
without having to install XNA Game Studio.

Bloom post-processing emulates the visual effect of bright lights and 
glowing objects. It does this by extracting the brightest parts of an 
image to a custom render target, blurring these bright areas, and then 
adding the blurred result back into the original image.

Because bloom is implemented entirely as a post-process, it can easily be 
used over the top of any other 2D or 3D rendering techniques. In addition 
to the more extreme glowing effects, when used subtly it provides a useful 
softening that can make computer graphics look more organic and help to 
hide artifacts from elsewhere in your rendering. Particle systems, for 
example, often look better with a subtle bloom applied over the top.

There are three stages to applying a bloom post-process:

Pass 1
    Extract the brightest parts of the scene. This uses the 
    BloomExtract.fx pixel shader, which removes any areas darker than the 
    specified threshold parameter value. Modifying the threshold changes 
    the look of the bloom: higher values produce smaller and better 
    defined glows, while smaller ones produce a softer end result. To see 
    just the result of this first pass, change the "Show final result" 
    setting to "Extracted bloom source".

Passes 2 and 3
    Blur the bright areas. This is done by using the GaussianBlur.fx pixel 
    shader, first to blur the image horizontally, and then again to blur 
    it vertically. The shader does multiple lookups into different parts 
    of the source texture, and then averages the results using a Gaussian 
    curve to weight the importance of each sample. The work is split over 
    two passes because Shader Model 2.0 is not capable of doing enough 
    texture lookups to give a high-quality blur along both the horizontal 
    and vertical axis in a single pass. To see the result of these blur 
    passes, change the "Show final result" setting to "Blurred 
    horizontally" or "Blurred both ways".

Pass 4
    Combine the blurred result with the original image. This uses the 
    BloomCombine.fx pixel shader, which has parameters to control how much 
    bloom to include, how much of the original image to retain, and for 
    adjusting the color saturation of both the bloom and original base 
    images. Tweaking these settings can produce a wide range of visual 
    effects.
