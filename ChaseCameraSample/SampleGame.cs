using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace ChaseCameraSample;
public class SampleGame : Game
{
    private SpriteBatch _spriteBatch;
    private KeyboardState _currentKeyboardState;
    private Texture2D _texture;
    private Camera _camera;
    private Vector2 _playerPosition;
    private float _moveSpeed = 2.5f;

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        //create a white texture so we don't need a Content file
        _texture = CreateTexture(GraphicsDevice, 16, 16, Color.White);
        _camera = new Camera(GraphicsDevice);
        Window.Title = "Sample Chase Camera - arrow keys to move..";
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
        GraphicsDevice.Clear(Color.Navy);
        Matrix transform = Matrix.CreateTranslation(-_camera.GetTopLeft().X, -_camera.GetTopLeft().Y, 0);
        _spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, transform);
        base.Draw(gameTime);
        DrawGridForReference();
        DrawPlayer();
        _spriteBatch.End();
    }

    private void DrawPlayer()
    {
        _spriteBatch.Draw(_texture, _playerPosition, Color.Yellow);
    }

    private void DrawGridForReference()
    {
        float spacing = 100;
        for (int x = -10; x <= 10; x++)
        {
            for (int y = -10; y <= 10; y++)
            {
                _spriteBatch.Draw(_texture, new Vector2(x * spacing + spacing/2, y * spacing + spacing / 2), Color.Blue);
            }

        }
    }

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