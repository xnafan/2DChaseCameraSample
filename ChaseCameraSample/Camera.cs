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

    public Camera(GraphicsDeviceManager graphicsDeviceManager)
    {
        _quarterScreen =  new Vector2 (graphicsDeviceManager.PreferredBackBufferWidth/2, graphicsDeviceManager.PreferredBackBufferHeight/2);
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