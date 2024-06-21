# 2D ChaseCamera Code Sample
This project shows how to make a very simple camera class in Monogame which follows the player around.
![Sample of chase camera](https://github.com/xnafan/2DChaseCameraSample/blob/master/ChaseCameraSample.gif)

The magic happens in the Game.Draw method where the camera's offset is used to create a translation matrix to use in the SpriteBatch object.  
First a complete spritebatch is run using the camera offset, then a new spritebatch is used to write text in an absolute position (without the camera matrix). 
This second spritebatch can be used to add your GUI elements in a static position regardless of the camera's position in your game world.

```C#
protected override void Draw(GameTime gameTime)
{
    //clear the screen with a dark blue color
    GraphicsDevice.Clear(Color.Navy);

    //Use the offset of the camera's top left corner
    //relative to the center of the camera (where it is pointing)
    //to create a transformation matrix in 2D
    Matrix transform = Matrix.CreateTranslation(-_camera.GetTopLeft().X, -_camera.GetTopLeft().Y, 0);
    //send the transformation matrix to the spritebatch, so everything is drawn relative to the camera
    _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, transform);

    //call the super class draw method
    base.Draw(gameTime);

    //draw the grid and the player, world and camera coordinates
    DrawGridForReference();
    DrawPlayerWithPosition();
    DrawWorldAndCameraCoordinates();
    _spriteBatch.End();

    //using a new spritebatch without the transformation matrix
    //draw the UI on top of everything else, with absolute coordinates
    _spriteBatch.Begin();
    _spriteBatch.DrawString(_font, " Arrows to move, ESC to exit", Vector2.One * 10, Color.Cyan);
    _spriteBatch.End();
}
```  

The Camera class is basically a wrapper around three responsibilities:  
- knowing where the camera is pointing (the center of the camera), stored in the Vector2 property *Center*
- knowing the offset between the center of the camera and the top left corner of the screen (a quarter screen)
- being able to move slightly towards something (in this case the player's location) for every call to MoveToward()
The camera calculates the quarter screen by being sent the GraphicsDeviceManager in its constructor and inspecting the PreferredBackBufferHeight and -width.
# Camera class
```C# 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Timers;

namespace ChaseCameraSample;

/// <summary>
/// The Camera class is basically just a location (the center of the screen)
/// which is used to generate a translation matrix for use in the SpriteBatch.Begin method
/// which in turn offsets everything that is drawn to the screen.
/// The offset is a quarter of the screen size (which is the top-left corner of the screen)
/// </summary>
internal class Camera
{
    //where the camera is centered
    public Vector2  Center { get; set; }
    
    //how big a quarter of the screen is, in order to calculate top left corner
    private Vector2 _quarterScreen;
    public Vector2 GetTopLeft() => Center - _quarterScreen;

    public Camera(GraphicsDeviceManager graphicsDeviceManager)
    {
        _quarterScreen =  new Vector2 (graphicsDeviceManager.PreferredBackBufferWidth/2, graphicsDeviceManager.PreferredBackBufferHeight/2);
    }
    public void MoveToward(Vector2 target, float deltaTimeInMs, float movePercentage= .02f)
    {
        //figure out which direction to move the camera,
        //by subtracting the current location from the target's
        Vector2 differenceInPosition = target - Center;

        //figure out how far to move in each update
        //based on distance to the target multiplied by how many percent of that distance to move
        differenceInPosition *= movePercentage;

        //get a fraction how much time has passed since last update
        //so the camera moves at a constant speed
        var fractionOfPassedTime = deltaTimeInMs/ 10;
        //note: dividing by 10 is an arbitrary constant,
        //which works well in this case
        //to make the camera slower than the player

        //move the camera towards the target based on
        //both the desired percentage of distance and the time passed
        Center += differenceInPosition * fractionOfPassedTime;

        //if the camera is very close to the target, just center it on the target
        //in order to avoid "jiggling" if the camera continuously overshoots the target
        if((target - Center).Length() < movePercentage)
        {
            Center = target;
        }
    }
}
```
# Smooth motion as a C# one-liner
[u/Apostolique](https://www.reddit.com/user/Apostolique/) suggested using Lerping to smooth the motion of the camera even more - thanks! 😊👍  
I have chosen to keep the camera as is above, to show you what is going on in the math, step by step.  
To get linear interpolation with his one-liner, you can make the following changes to the camera class:  

Change the Camera.MoveToward method to
```C#
public void MoveToward(Vector2 target, float deltaTimeInMs, float movePercentage= .02f)
{
    Center = ExpDecay(Center, target, movePercentage, deltaTimeInMs/10);
}
```
And add this private helpermethod for computing a percentage of the path to the target, interpolated
```C# 
private static Vector2 ExpDecay(Vector2 start, Vector2 target, float decay, float deltaTime)
{
    return target + (start - target) * MathF.Exp(-decay * deltaTime);
}
```
Here's a great video on LERP'ing (linear interpolation) in games, if you want the full explanation with visuals.  
[Lerp smoothing is broken](https://www.youtube.com/watch?v=LSNQuFEDOyQ)

*A note on rendering large worlds*  
If you have a big game world with many sprites and entities to be drawn, make sure you only draw what's visible in the camera's field of view.  
