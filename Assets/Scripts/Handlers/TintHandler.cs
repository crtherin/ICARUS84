using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TintHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite tintMap;
    
    [Header ("Settings")]
    [SerializeField] private float[] tints = new float[12];

    private MaterialPropertyBlock block;
    private Sprite prevTintMap;
    
    private static readonly int TintMapID = Shader.PropertyToID("_TintMap");

    private static readonly int[] TintIDs = {
        Shader.PropertyToID("_TintRA"),
        Shader.PropertyToID("_TintGA"),
        Shader.PropertyToID("_TintBA"),
        Shader.PropertyToID("_TintRB"),
        Shader.PropertyToID("_TintGB"),
        Shader.PropertyToID("_TintBB"),
        Shader.PropertyToID("_TintRC"),
        Shader.PropertyToID("_TintGC"),
        Shader.PropertyToID("_TintBC"),
        Shader.PropertyToID("_TintRD"),
        Shader.PropertyToID("_TintGD"),
        Shader.PropertyToID("_TintBD")
    };

#if UNITY_EDITOR    
    protected void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected void OnValidate()
    {
        OnDidApplyAnimationProperties();
        
        if (block == null)
            block = new MaterialPropertyBlock();

        spriteRenderer.GetPropertyBlock(block);

        for (int i = 0; i < 12; i++)
            block.SetFloat(TintIDs[i], tints[i]);

        block.SetTexture(TintMapID, tintMap ? tintMap.texture : Texture2D.blackTexture);
            
        spriteRenderer.SetPropertyBlock(block);
    }
#endif

    public void OnDidApplyAnimationProperties()
    {
        if (prevTintMap != tintMap)
        {
            prevTintMap = tintMap;

            if (block == null)
                block = new MaterialPropertyBlock();

            spriteRenderer.GetPropertyBlock(block);
            
            block.SetTexture(TintMapID, tintMap ? tintMap.texture : Texture2D.blackTexture);
            
            spriteRenderer.SetPropertyBlock(block);
        }
    }

    public void SetTintMap(Sprite newTintMap)
    {
        tintMap = newTintMap;
        OnDidApplyAnimationProperties();
    }

    public void SetTint(int i, float tint)
    {
        if (block == null)
            block = new MaterialPropertyBlock();
        
        spriteRenderer.GetPropertyBlock(block);
        block.SetFloat(TintIDs[i], tints[i] = tint);
        spriteRenderer.SetPropertyBlock(block);
    }
}