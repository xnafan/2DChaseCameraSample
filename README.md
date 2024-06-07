# 2D ChaseCamera Code Sample
This project shows how to make a very simple camera which follows the player around.


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
