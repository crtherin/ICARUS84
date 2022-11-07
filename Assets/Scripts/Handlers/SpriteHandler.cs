using UnityEngine;

namespace Handlers
{
    public class SpriteHandler : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Rect buttonPos1;
        private Rect buttonPos2;

#if UNITY_EDITOR
        
        protected void Reset()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            //spriteRenderer.material.set
        }

        protected void OnDrawGizmos()
        {
            Sprite sprite = spriteRenderer.sprite;

            ushort[] triangles = sprite.triangles;
            Vector2[] vertices = sprite.vertices;

            // draw the triangles using grabbed vertices
            for (int i = 0; i < triangles.Length; i = i + 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                Gizmos.color = Color.red;
                Gizmos.DrawLine(vertices[a], vertices[b]);
                Gizmos.DrawLine(vertices[b], vertices[c]);
                Gizmos.DrawLine(vertices[c], vertices[a]);
            }
        }
        
#endif

        /*protected void OnGUI()
        {
            //Press this Button to edit the vertices obtained from the Sprite
            if (GUI.Button(buttonPos2, "Perform OverrideGeometry"))
                ChangeSprite();
        }

        // Show the sprite triangles

        // Edit the vertices obtained from the sprite.  Use OverrideGeometry to
        // submit the changes.
        protected void ChangeSprite()
        {
            //Fetch the Sprite and vertices from the SpriteRenderer
            Sprite sprite = spriteRenderer.sprite;
            Vector2[] spriteVertices = sprite.vertices;

            for (int i = 0; i < spriteVertices.Length; i++)
            {
                spriteVertices[i].x = Mathf.Clamp(
                    (sprite.vertices[i].x - sprite.bounds.center.x -
                        (sprite.textureRectOffset.x / sprite.texture.width) + sprite.bounds.extents.x) /
                    (2.0f * sprite.bounds.extents.x) * sprite.rect.width,
                    0.0f, sprite.rect.width);

                spriteVertices[i].y = Mathf.Clamp(
                    (sprite.vertices[i].y - sprite.bounds.center.y -
                        (sprite.textureRectOffset.y / sprite.texture.height) + sprite.bounds.extents.y) /
                    (2.0f * sprite.bounds.extents.y) * sprite.rect.height,
                    0.0f, sprite.rect.height);

                // Make a small change to the second vertex
                if (i == 2)
                {
                    //Make sure the vertices stay inside the Sprite rectangle
                    if (spriteVertices[i].x < sprite.rect.size.x - 5)
                    {
                        spriteVertices[i].x = spriteVertices[i].x + 5;
                    }
                    else spriteVertices[i].x = 0;
                }
            }

            //Override the geometry with the new vertices
            sprite.OverrideGeometry(spriteVertices, sprite.triangles);
        }*/
    }
}