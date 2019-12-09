using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using LookingGlass;

public class QuiltSelector : MonoBehaviour
{
#if !WITHOUT_HOLOPLAY
    public Holoplay holoplay;
#endif

    [System.Serializable]
    public struct TextureConfig
    {
        [SerializeField]
        public Texture2D texture;
        [SerializeField]
        public int num;
    }

    [SerializeField]
    TextureConfig[] textures;

    private RenderTexture renderTexture;

    public int tileX = 5;
    public int tileY = 9;
    public float scaleP = 0.0f;

    public bool isOutputQuilt = false;

    private struct CalcRenderTexSize
    {
        public int tileSizeX;
        public int tileSizeY;
        public int paddingX;
        public int paddingY;

        public CalcRenderTexSize(int renderTargetW, int renderTargetH, int tileX, int tileY)
        {
            tileSizeX = (int)renderTargetW / tileX;
            tileSizeY = (int)renderTargetH / tileY;
            paddingX = (int)renderTargetW - tileX * tileSizeX;
            paddingY = (int)renderTargetH - tileY * tileSizeY;
        }
    }

    public int num_0 = 45;
    public int num_1 = 45;
    public int num_2 = 45;

    private void UpdateAnimation()
    {
        textures[0].num = num_0;
        textures[1].num = num_1;
        textures[2].num = num_2;
    }




    // Start is called before the first frame update
    void Start()
    {
        renderTexture = new RenderTexture(textures[0].texture.width, textures[0].texture.height, 0);
#if !WITHOUT_HOLOPLAY
        holoplay.overrideQuilt = renderTexture;
#endif
        Application.targetFrameRate = 30;
        Time.captureFramerate = 30;

        Application.targetFrameRate = 30;
        Time.captureFramerate = 30;

        if (isOutputQuilt)
        {
            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.mainTexture = renderTexture;
#if !WITHOUT_HOLOPLAY
            holoplay.lightfieldMat = mat;
#endif
        }

    }
    private void OnDestroy()
    {
#if !WITHOUT_HOLOPLAY
        holoplay.quiltRT = null;
#endif
        RenderTexture.Destroy(renderTexture);
    }

    // Update is called once per frame
    void Update()
    {
        this.UpdateAnimation();


        CommandBuffer commandBuffer = new CommandBuffer();
        commandBuffer.SetRenderTarget(renderTexture);
        commandBuffer.ClearRenderTarget(true, true, Color.black);

        CalcRenderTexSize calcRenderTex = new CalcRenderTexSize(renderTexture.width, renderTexture.height, tileX, tileY);
        int currentIdx = 0;
        int currentNum = 0;
        Rect r = new Rect(0, 0, 1, 1);
        for( int i = 0; i < tileX * tileY; ++i)
        {
            for(int j = currentIdx; j < textures.Length - 1; ++j)
            {
                if(textures[currentIdx].num <= currentNum)
                {
                    currentNum = 0;
                    ++currentIdx;
                }
            }
            if( currentIdx >= textures.Length)
            {
                currentIdx = textures.Length - 1;
            }
            AddCommandBuffer(commandBuffer, textures[currentIdx].texture, i, calcRenderTex,r);
            currentNum++;
        }
        if(this.scaleP > 0.0f)
        {
            ExecuteNextBlock(commandBuffer, calcRenderTex);
        }

        Graphics.ExecuteCommandBuffer(commandBuffer);
        commandBuffer.Clear();
    }

    private void ExecuteNextBlock(CommandBuffer commandBuffer, CalcRenderTexSize calcRenderTex)
    {
        int currentIdx = 0;
        int currentNum = 0;

        Rect right = new Rect(0.9f-scaleP*0.5f, 0.6f - scaleP*0.5f, scaleP, scaleP);
        Rect left = new Rect(0.1f -scaleP*0.5f, right.y, right.width, right.height);
        for (int i = 0; i < tileX * tileY; ++i)
        {
            for (int j = currentIdx; j < textures.Length - 1; ++j)
            {
                if (textures[currentIdx].num <= currentNum)
                {
                    currentNum = 0;
                    ++currentIdx;
                }
            }
            if (currentIdx >= textures.Length)
            {
                currentIdx = textures.Length - 1;
            }
            if (currentIdx > 0)
            {
                AddCommandBuffer(commandBuffer, textures[currentIdx - 1].texture, i, calcRenderTex, left);
            }
            if (currentIdx < textures.Length - 1)
            {
                AddCommandBuffer(commandBuffer, textures[currentIdx + 1].texture, i, calcRenderTex, right);
            }
            currentNum++;
        }

    }

    private void AddCommandBuffer(CommandBuffer commandBuffer, Texture src,int idx, CalcRenderTexSize calcRenderTex,Rect drawRect)
    {
        Vector2 scale = new Vector2(calcRenderTex.tileSizeX, calcRenderTex.tileSizeY);
        Vector2 offset = new Vector2((idx % tileX) * calcRenderTex.tileSizeX, (idx / tileX) * calcRenderTex.tileSizeY + calcRenderTex.paddingY);

        scale.x /= (renderTexture.width );
        scale.y /= (renderTexture.height );
        offset.x /= (renderTexture.width );
        offset.y /= (renderTexture.height );

        // offset
        Matrix4x4 matrix = Matrix4x4.identity;

        var mesh = CreateQuadMesh(scale, offset,drawRect);
        var material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = src;
        commandBuffer.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
        commandBuffer.DrawMesh(mesh, matrix, material);
    }
    
    Mesh CreateQuadMesh(Vector2 scale , Vector2 offset,Rect drawRect)
    {
        Mesh mesh = new Mesh();
        var uv = new Vector2[] {
            new Vector2 (offset.x, offset.y),
            new Vector2 (offset.x, offset.y + scale.y),
            new Vector2 (offset.x + scale.x, offset.y),
            new Vector2 (offset.x + scale.x, offset.y+scale.y),
        };

        offset.x += drawRect.x * scale.x;
        offset.y += drawRect.y * scale.y;
        scale.x *= drawRect.width;
        scale.y *= drawRect.height;
        var vertcies = new Vector3[] {
            new Vector3 (offset.x, offset.y, 0),
            new Vector3 (offset.x,  offset.y + scale.y, 0),
            new Vector3 (offset.x + scale.x , offset.y, 0),
            new Vector3 (offset.x + scale.x ,  offset.y + scale.y, 0),
        };
        for(int i = 0; i < vertcies.Length; ++i)
        {
            vertcies[i] = vertcies[i] * 2.0f - new Vector3(1.0f,1.0f,0.0f);
        }

        mesh.vertices = vertcies;


        mesh.uv = uv;
        mesh.triangles = new int[] {
            0, 1, 2,
            1, 3, 2,
        };
        return mesh;
    }
}
