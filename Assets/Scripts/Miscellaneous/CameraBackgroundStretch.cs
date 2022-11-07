using UnityEngine;

namespace Miscellaneous
{
    public class CameraBackgroundStretch : MonoBehaviour
    {
        private void Resize(Camera cam)
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return;

            transform.localScale = new Vector3(1, 1, 1);

            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

            float worldScreenHeight = cam.orthographicSize * 2f;
            float worldScreenWidth = worldScreenHeight * cam.aspect;
            Vector3 position = cam.transform.position + Vector3.forward * 20;
            //position.z = Mathf.Max(position.z + 100, 100);
            transform.position = position;
            transform.localScale =
                new Vector3(worldScreenWidth / spriteSize.x, worldScreenHeight / spriteSize.y, 1);
        }

        public void Update()
        {
            Resize(Camera.main);
        }
        
        protected void OnDrawGizmos()
        {
            Resize(Camera.current);
        }
    }
}