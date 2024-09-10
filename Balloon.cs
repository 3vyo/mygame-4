using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace mygame
{
    internal class Balloon
    {
        private static readonly float SCALE = 4f;
        public Texture2D texture;
        public Vector2 position;
        public List<Vector2> path;
        private int currentWaypoint = 0;
        private float velocity ;  // Velocity (speed) for the balloon

        public Rectangle Rect
        {
            get
            {
                return new Rectangle(
                    (int)position.X,
                    (int)position.Y,
                    texture.Width * (int)SCALE,
                    texture.Height * (int)SCALE
                );
            }
        }

        // Constructor updated to include velocity
        public Balloon(Texture2D texture, Vector2 position, List<Vector2> path, float velocity)
        {
            this.texture = texture;
            this.position = position;
            this.path = path;
            this.velocity = velocity;  // Assign the velocity parameter
        }

        public virtual void Update(GameTime gameTime)
        {
            if (path == null || path.Count == 0) return;

            Vector2 target = path[currentWaypoint];
            Vector2 difference = target - position;

            if (difference.Length() > 0)
            {
                Vector2 direction = Vector2.Normalize(difference);

                if (!float.IsNaN(direction.X) && !float.IsNaN(direction.Y))
                {
                    position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            // Check if the balloon reached the current waypoint
            if (Vector2.Distance(position, target) < 10f) // Adjust threshold if necessary
            {
                currentWaypoint++;
                if (currentWaypoint >= path.Count)
                {
                    currentWaypoint = 0; // Loop back to the first waypoint
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, null, Color.White, 0, Vector2.Zero, SCALE, SpriteEffects.None, 0);
        }
    }
}
