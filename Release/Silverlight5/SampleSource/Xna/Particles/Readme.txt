This is a Silverlight port of the XNA Particles sample, which introduces 
the concept of a particle system, and shows how to draw particle effects 
using SpriteBatch. Two particle effects are demonstrated: an explosion and 
a rising plume of smoke.

The original XNA version is available from:

    http://create.msdn.com/en-US/education/catalog/sample/particle

This sample uses the XNA Game Studio Content Pipeline to compile textures 
and font data into .xnb binary format. This means you must install XNA 
Game Studio 4.0 (in addition to Silverlight) before you can build the 
Content projects. The resulting .xnb files are embedded 
in the Silverlight .xap package from where they can be loaded and drawn by 
the Silverlight application. Because XNA is only used to build this content 
but not to draw it, Game Studio only needs to be installed on your development PC. 
Users can run the resulting application with the standard Silverlight plugin, 
without having to install XNA Game Studio.

Particle systems are a technique for rendering special effects that are 
typically very fluid and organic. They are common in games, generally 
being used for smoke, fire, sparks, and splashes of water.

A particle system consists of any number of small particles. Each particle 
has its own physical properties, typically including position, velocity, 
and acceleration. More complex particle systems may include even more 
properties. Particles are created and initialized with some initial 
properties determined by the overall particle system, but once the system 
has begun, the particles all act independently of one another. Particles 
are typically drawn as 2D alpha blended sprites. Once many of these 
independently updating particles are drawn on top of one another, the 
particle system has the appearance of a chaotic and natural system.

The MainPage class is the main body of the sample. It demonstrates two 
special effects using particle systems: an explosion and a rising column 
of smoke. The explosion is done using two separate particle systems: one 
for the fiery core, and one for the smoke that remains afterwards. The 
smoke plume is one particle system. To use these effects, MainPage creates 
and maintains three particle systems: ExplosionParticleSystem, 
ExplosionSmokeParticleSystem, and SmokePlumeParticleSystem. The MainPage 
class has a state that corresponds to what effect is being demonstrated, 
and it keeps timers, so that it knows when to start a new effect.

The most interesting class in this sample is ParticleSystem, an abstract 
class that contains the core functionality for a particle system. The 
AddParticles method of ParticleSystem is called to add a new effect to the 
scene. AddParticles uses several constants to initialize the particles. 
The constants, which should all be set by subclasses of ParticleSystem, 
give each particle system its unique look. Subclasses can also override 
several virtual methods in ParticleSystem to get more flexible control 
over particles as they are created.

One key thing to note is that all of the particles are allocated when the 
program starts. Particles are then reused as necessary, and are never 
instantiated during run time. This behavior avoids having any unnecessary 
garbage collections at run time, and keeps your game running smoothly. 
However, it does have one negative side effect: it is possible that when 
AddParticles is called, all particles are currently busy. In this 
situation, AddParticles adds as many as it can, and then stops, which 
results in a desired effect never showing up. This issue is addressed by a 
parameter to the ParticleSystem constructor, an integer that specifies the 
maximum number of effects desired. It is up to the developer who is using 
the ParticleSystem to pick the lowest number possible that will still 
yield good results.
