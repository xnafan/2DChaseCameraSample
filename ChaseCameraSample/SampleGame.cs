using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace ChaseCameraSample;
public class SampleGame : Game
{
    private SpriteBatch _spriteBatch;
    private KeyboardState _currentKeyboardState;
    private Texture2D _texture, _smallTexture;
    private Camera _camera;
    private Vector2 _playerPosition;
    private float _moveSpeed = 2.5f;
    private GraphicsDeviceManager graphics;
    private SpriteFont _font;

    public SampleGame()
    {
        graphics = new GraphicsDeviceManager(this);
    }
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        //create two white textures so we don't need an image in the Content file
        _texture = CreateTexture(GraphicsDevice, 16, 16, Color.White);
        _smallTexture = CreateTexture(GraphicsDevice, 8, 8, Color.White);
        Window.Title = "Sample Chase Camera - arrow keys to move..";
        _font = Content.Load<SpriteFont>("Content/Default");
        
        //set the desired window size
        graphics.PreferredBackBufferWidth = 1024;
        graphics.PreferredBackBufferHeight = 768;
        
        //apply the size changes
        graphics.ApplyChanges();

        //instantiates the camera with the graphics device manager,
        //so the camera can get the screen size
        _camera = new Camera(graphics);
    }

    protected override void Update(GameTime gameTime)
    {
        GetKeyboardStateAndExitIfEscapePressed();
        MoveCameraTowardsPlayer();
        MovePlayerBasedOnKeyboardInput();
    }

    private void GetKeyboardStateAndExitIfEscapePressed()
    {
        _currentKeyboardState = Keyboard.GetState();
        if (_currentKeyboardState.IsKeyDown(Keys.Escape)) { Exit(); }
    }

    private void MoveCameraTowardsPlayer()
    {
        _camera.MoveToward(_playerPosition);
    }

    private void MovePlayerBasedOnKeyboardInput()
    {
        if (_currentKeyboardState.IsKeyDown(Keys.Up)) { _playerPosition += Vector2.UnitY * -1 * _moveSpeed; }
        if (_currentKeyboardState.IsKeyDown(Keys.Down)) { _playerPosition += Vector2.UnitY * 1 * _moveSpeed; }
        if (_currentKeyboardState.IsKeyDown(Keys.Left)) { _playerPosition += Vector2.UnitX * -1 * _moveSpeed; }
        if (_currentKeyboardState.IsKeyDown(Keys.Right)) { _playerPosition += Vector2.UnitX * 1 * _moveSpeed; }
    }

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

    private void DrawWorldAndCameraCoordinates()
    {
        _spriteBatch.Draw(_smallTexture, Vector2.Zero - Vector2.One * 4, Color.White);
        _spriteBatch.DrawString(_font, "World: (0,0)", new Vector2(15, -12), Color.White);
        _spriteBatch.Draw(_smallTexture, _camera.Center - Vector2.One * 4, Color.Red);
        _spriteBatch.DrawString(_font, $"Camera center: ({_camera.Center.X:0.},{_camera.Center.Y:0.})", _camera.Center + Vector2.One * 20, Color.Red);

    }

    private void DrawPlayerWithPosition()
    {
        _spriteBatch.Draw(_texture, _playerPosition - Vector2.One * 8, Color.Yellow);
        _spriteBatch.DrawString(_font, $"Player location ({_playerPosition.X:0.},{_playerPosition.Y:0.})", _playerPosition + Vector2.One * -60, Color.Yellow);
    }

    //draws a grid for reference with 100 pixel spacing
    //21 by 21 squares with the center one at (0,0)
    private void DrawGridForReference()
    {
        float spacing = 100;
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                _spriteBatch.Draw(_texture, new Vector2(x * spacing + spacing / 2, y * spacing + spacing / 2), Color.Blue);
            }
        }
    }

    //Helpermethod for creating a texture of a specified size and color
    private static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Color color)
    {
        //initialize a texture
        Texture2D texture = new Texture2D(device, width, height);

        //the array holds the color for each pixel in the texture
        Color[] data = new Color[width * height];
        for (int pixel = 0; pixel < data.Count(); pixel++)
        {
            //the function applies the color according to the specified pixel
            data[pixel] = color;
        }

        //set the color
        texture.SetData(data);

        return texture;
    }
}