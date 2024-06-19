# 2D ChaseCamera Code Sample
This project shows how to make a very simple camera class in Monogame which follows the player around.
![Sample of chase camera](https://github.com/xnafan/2DChaseCameraSample/blob/master/SampleChaseCamera.gif)

The magic happens in the Game.Draw method where the camera's offset is used to create a translation matrix to use in the SpriteBatch object:  

```C#
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.Navy);
    //Use the offset of the camera's top left corner
    //relative to the center of the camera (where it is pointing)
    //to create a transformation matrix in 2D
    Matrix transform = Matrix.CreateTranslation(-_camera.GetTopLeft().X, -_camera.GetTopLeft().Y, 0);
    _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, transform);
    base.Draw(gameTime);
    DrawGridForReference();
    DrawPlayer();
    _spriteBatch.End();
}
```  

The Camera class is basically a wrapper around three responsibilities:  
- knowing where the camera is pointing (the center of the camera), stored in the Vector2 property *Center*
- knowing the offset between the center of the camera and the top left corner of the screen (a quarter screen)
- being able to move slightly towards something (in this case the player's location) for every call to MoveToward()

# Camera class
```C# 
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

    public Camera(GraphicsDevice graphicsDevice)
    {
        _quarterScreen =  new Vector2 (graphicsDevice.Viewport.Width/2, graphicsDevice.Viewport.Height/2);
    }
    public void MoveToward (Vector2 target, float movePercentage= .02f)
    {
        //figure out which direction to move the camera,
        //by subtracting the current location from the target's
        Vector2 delta = target - Center;

        //figure out how far to move in each update
        //based on distance to the target
        delta *= movePercentage;

        //move the camera to the new location
        Center += delta;

        //if the camera is very close, just center it on the target
        //in order to avoid "jiggling" if the camera continuously overshoots the target
        if((target - Center).Length() < movePercentage)
        {
            Center = target;
        }
    }
}
```
