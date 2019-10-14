using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class ImageMapTerrain : GeneratorUtility
{
    //高さ情報を持った画像
    [SerializeField] private Texture2D heightMap;
    //高さの倍率
    [SerializeField] private float heightMultiplier = 1f;
    //サイズの倍率
    [SerializeField] private float sizeMultiplier = 1f;
    //何ピクセルごとに取得するか
    [SerializeField] private int pixelSampleRate = 1;
    //頂点座標をずらす強さ
    [SerializeField] private float noise = 1f;
    //貼り付けるテクスチャ画像がついたマテリアル
    [SerializeField] private Material stageMaterial;

    public void Generate ()
    {
        //縦方向のピクセル数
        int size = Mathf.CeilToInt ((float)heightMap.width / pixelSampleRate);
        //pixelSampleRateごとにheightMapのピクセルの色を取得
        Color[] pixels = new Color[size * size];
        int p = 0;
        for (int z = 0 ; z < size * pixelSampleRate ; z += pixelSampleRate)
        {
            for (int x = 0 ; x < size * pixelSampleRate ; x += pixelSampleRate)
            {
                pixels[p] = heightMap.GetPixel (x, z);
                p++;
            }
        }
        //ピクセルの色から頂点の座標とUV座標を設定
        Vector3[] vertices = new Vector3[size * size];
        Vector2[] uvs = new Vector2[size * size];
        p = 0;
        for (int z = 0 ; z < size ; z++)
        {
            for (int x = 0 ; x < size ; x++)
            {
                ColorHSV colorHSV = ColorUtility.GetHSVByRGB (pixels[p]);
                float y = colorHSV.v * heightMultiplier;
                float random = Random.Range (-noise, noise);
                vertices[z * size + x] = new Vector3 (x * sizeMultiplier * pixelSampleRate, y, z * sizeMultiplier * pixelSampleRate) + new Vector3 (1f, 0, 1f) * random;
                uvs[z * size + x] = new Vector2 (vertices[z * size + x].x / size, vertices[z * size + x].z / size) / (pixelSampleRate * sizeMultiplier);
                p++;
            }
        }

        //頂点インデックスの設定
        /*
             a   ←   b

             ↓　 ↗︎↙︎  ↑

             c   →   d

        */
        int triangleIndex = 0;
        int[] triangles = new int[(size - 1) * (size - 1) * 6];
        for (int z = 0 ; z < size - 1 ; z++)
        {
            for (int x = 0 ; x < size - 1 ; x++)
            {
                int a = z * size + x;
                int b = a + 1;
                int c = a + size;
                int d = c + 1;

                triangles[triangleIndex] = a;
                triangles[triangleIndex + 1] = c;
                triangles[triangleIndex + 2] = b;

                triangles[triangleIndex + 3] = c;
                triangles[triangleIndex + 4] = d;
                triangles[triangleIndex + 5] = b;

                triangleIndex += 6;
            }
        }

        //頂点座標、UV座標、頂点インデックスから、メッシュを生成
        Mesh mesh = new Mesh ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals ();

        //メッシュを描画するやつ
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter> ();
        if (!meshFilter) meshFilter = gameObject.AddComponent<MeshFilter> ();
        //メッシュの見え方のやつ
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer> ();
        if (!meshRenderer) meshRenderer = gameObject.AddComponent<MeshRenderer> ();
        //メッシュコライダー
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider> ();
        if (!meshCollider) meshCollider = gameObject.AddComponent<MeshCollider> ();
        //それぞれ適用
        meshFilter.sharedMesh = mesh;
        meshRenderer.sharedMaterial = stageMaterial;
        meshCollider.sharedMesh = mesh;

        //フラットなメッシュに変換
        MeshFilter mf = GetComponent<MeshFilter> ();
        mesh = mf.sharedMesh;
        mf.sharedMesh = mesh;
        Vector3[] oldVerts = mesh.vertices;
        Vector2[] oldUvs = mesh.uv;
        triangles = mesh.triangles;
        vertices = new Vector3[triangles.Length];
        uvs = new Vector2[triangles.Length];
        for (int i = 0 ; i < triangles.Length ; i++)
        {
            vertices[i] = oldVerts[triangles[i]];
            uvs[i] = oldUvs[triangles[i]];
            triangles[i] = i;
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals ();

        //スタティックにする
        gameObject.isStatic = true;

    }
}

