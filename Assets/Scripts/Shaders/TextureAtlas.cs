using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Shaders
{
    [CreateAssetMenu(menuName = "Texture Atlas")]
    public class TextureAtlas : ScriptableObject
    {
        [SerializeField] private Texture2D[] textures;
        [SerializeField] private int cellSize;
        [SerializeField] private int cellOffsetX;
        [SerializeField] private int cellOffsetY;

        [SerializeField, HideInInspector] private Texture2D atlas;
        [SerializeField, HideInInspector] private Vector2 bounds;
        [SerializeField, HideInInspector] private int wCount;
        [SerializeField, HideInInspector] private int hCount;

#if UNITY_EDITOR

        [ContextMenu("Generate")]
        public void Generate()
        {
            float l = textures.Length;
            wCount = Mathf.CeilToInt(Mathf.Sqrt(l));
            hCount = Mathf.CeilToInt(l / wCount);

            int width = FindPow2(wCount * cellSize);
            int height = FindPow2(hCount * cellSize);

            bounds = Vector2.one;
            bounds.x *= wCount * cellSize;
            bounds.x /= width;
            bounds.y *= hCount * cellSize;
            bounds.y /= height;

            if (atlas == null)
            {
                atlas = new Texture2D(width, height, TextureFormat.RGBA32, false);
                AssetDatabase.AddObjectToAsset(atlas, this);
            } else if (atlas.width != width || atlas.height != height)
            {
                atlas.Resize(width, height);
            }

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    atlas.SetPixel(x, y, Color.clear);
                }
            }

            for (int y = 0; y < hCount; y++)
            {
                for (int x = 0; x < wCount; x++)
                {
                    int i = x + y * wCount;

                    if (i >= textures.Length)
                        break;

                    Texture2D texture = textures[i];

                    if (texture == null)
                        continue;
                    
                    Color[] sprite = texture.GetPixels(cellOffsetX, cellOffsetY, cellSize, cellSize);

                    for (int j = 0; j < sprite.Length; j++)
                    {
                        if (sprite[j].a < 0.01f)
                            sprite[j] = Color.clear;
                    }
                    
                    atlas.SetPixels(x * cellSize, y * cellSize, cellSize, cellSize, sprite, 0);
                }
            }

            atlas.name = name;
            atlas.Apply();
            
            EditorUtility.SetDirty(this);
        }

        [ContextMenu("Delete")]
        public void Delete()
        {
            if (atlas == null)
                return;
            
            DestroyImmediate(atlas, true);
            
            EditorUtility.SetDirty(this);
        }
        
        
#endif

        private static int FindPow2(int value)
        {
            int o = 2;

            while (o < value)
                o *= 2;

            return o;
        }
        
        public Texture2D Get()
        {
            return atlas;
        }

        public (Vector2, Vector2) GetCell(int i)
        {
            int x = i % wCount;
            int y = i / wCount;
            
            Vector2 uvD = bounds;
            uvD.x /= wCount;
            uvD.y /= hCount;
            
            Vector2 uv0 = uvD;
            uv0.x *= x;
            uv0.y *= y;

            Vector2 uv1 = uv0;
            uv0.x += uvD.x;
            uv0.y += uvD.y;

            return (uv0, uv1);
        }
    }
}
