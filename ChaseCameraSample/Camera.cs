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
    public void MoveToward(GameTime gameTime, Vector2 target, float movePercentage= .02f)
    {
        //figure out which direction to move the camera,
        //by subtracting the current location from the target's
        Vector2 differenceInPosition = target - Center;

        //figure out how far to move in each update
        //based on distance to the target multiplied by how many percent of that distance to move
        differenceInPosition *= movePercentage;

        //get a fraction how much time has passed since last update
        //so the camera moves at a constant speed
        var fractionOfPassedTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 10;

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