using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;
using Apos.Shapes;

namespace mygame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private List<Rectangle> textureStore;
        private Texture2D textureAtlas;
        private Texture2D hitboxTexture;
        private Dictionary<Vector2, int> mg;
        private Dictionary<Vector2, int> collisions;
        List<Balloon> bloon;
        private List<Vector2> path;  // Path waypoints for balloons

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 768;
            _graphics.PreferredBackBufferHeight = 896;
            _graphics.ApplyChanges();
            textureStore = new List<Rectangle>
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(0, 0, 32, 32)
            };

            mg = LoadMap("../../../Data/map_mg.csv");
            collisions = LoadMap("../../../Data/map_collisions.csv");
        }

        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new Dictionary<Vector2, int>();
            using (StreamReader reader = new StreamReader(filepath))
            {
                int y = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] items = line.Split(',');

                    for (int x = 0; x < items.Length; x++)
                    {
                        if (int.TryParse(items[x], out int value))
                        {
                            if (value > -1)
                            {
                                result[new Vector2(x, y)] = value;
                            }
                        }
                    }
                    y++;
                }
            }
            return result;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            bloon = new();

            // Load the texture atlas and balloon texture
            textureAtlas = Content.Load<Texture2D>("Atlas");
            Texture2D bloonTexture = Content.Load<Texture2D>("enemy");
            hitboxTexture = Content.Load<Texture2D>("hitboxes");

            // Define the path waypoints for the balloons to follow
            path = new List<Vector2>
            {
                new Vector2(1, 310),
                new Vector2(1, 200),
                new Vector2(100, 200)
            };

            // Set the velocity for the balloons
            float balloonVelocity = 100f;

            // Initialize balloons with path-following logic and velocity
            bloon.Add(new Balloon(bloonTexture, new Vector2(1, 310), path, balloonVelocity));

            _graphics.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y;

            // Update each balloon to follow the path
            foreach (var Bloon in bloon)
            {
                Bloon.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int display_tilesize = 64;
            int num_tiles_per_row = textureAtlas.Width / 32;
            int pixel_tilesize = 32;

            // Draw map grid and collisions
            foreach (var item in mg)
            {
                Rectangle drect = new Rectangle((int)item.Key.X * display_tilesize, (int)item.Key.Y * display_tilesize, display_tilesize, display_tilesize);
                int X = item.Value % num_tiles_per_row;
                int Y = item.Value / num_tiles_per_row;
                Rectangle src = new Rectangle(X * pixel_tilesize, Y * pixel_tilesize, pixel_tilesize, pixel_tilesize);
                _spriteBatch.Draw(textureAtlas, drect, src, Color.White);
            }

            foreach (var item in collisions)
            {
                Rectangle drect = new Rectangle((int)item.Key.X * display_tilesize, (int)item.Key.Y * display_tilesize, display_tilesize, display_tilesize);
                int X = item.Value % num_tiles_per_row;
                int Y = item.Value / num_tiles_per_row;
                Rectangle src = new Rectangle(X * pixel_tilesize, Y * pixel_tilesize, pixel_tilesize, pixel_tilesize);
                _spriteBatch.Draw(hitboxTexture, drect, src, Color.White);
            }

            // Draw each balloon following its path
            foreach (var Bloon in bloon)
            {
                Bloon.Draw(_spriteBatch);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
