using UnityEngine;
using UnityEditor;
using System.Collections;

[ExecuteInEditMode]
public class RandomEnvironmentGenerator : GeneratorUtility
{
    //色とオブジェクト
    [SerializeField] private ObjectByColor[] objectByColors;
    private static Texture2D tex;
    private static bool flg = false;
    private static IEnumerator enumerator;


#if UNITY_EDITOR
    static double waitTime = 0;

    void OnEnable ()
    {
        waitTime = EditorApplication.timeSinceStartup;
        EditorApplication.update += EditorUpdate;
    }

    void OnDisable ()
    {
        EditorApplication.update -= EditorUpdate;
    }

    // 更新処理
    void EditorUpdate ()
    {
        // １／６０秒に１回更新
        if ((EditorApplication.timeSinceStartup - waitTime) >= 1f / 60f)
        {
            if (flg)
            {
                enumerator.MoveNext ();
            }
            SceneView.RepaintAll (); // シーンビュー更新
            waitTime = EditorApplication.timeSinceStartup;
        }
    }
#endif

    //生成
    public void Generate ()
    {
        if (flg)
        {
            return;
        }
        //最初に、子にいるオブジェクトを全て削除
        DestroyChildren ();
        //コルーチンを登録
        enumerator = GenerateLoop ();
        flg = true;
    }

    private IEnumerator GenerateLoop ()
    {
        Debug.Log ("オブジェクト生成中");
        //マテリアルのテクスチャを取得
        tex = (Texture2D)GetComponent<MeshRenderer> ().sharedMaterial.mainTexture;
        //それぞれの色のところにオブジェクトを生成
        for (int i = 0 ; i < objectByColors.Length ; i++)
        {
            //このループで使用するやち
            ObjectByColor _objectByColor = objectByColors[i];
            //生成させたい場所の色
            Color originalColor = _objectByColor.locationColor;

            //子にグループとしての空オブジェクトを生成
            GameObject group = new GameObject ("Group" + i);
            group.transform.parent = transform;
            group.isStatic = true;

            //距離毎に上からレイを撃って生成
            int miss = 0;
            int l = 0;
            for (float z = 10f ; ; z += _objectByColor.distanceRate)
            {
                for (float x = 10f ; ; x += _objectByColor.distanceRate)
                {
                    //レイ
                    Ray ray = new Ray (new Vector3 (x, 10000f, z), Vector3.down);
                    RaycastHit hit;
                    //地面があった
                    if (Physics.Raycast (ray, out hit))
                    {
                        miss = 0;
                        //レイが当たったところのテクスチャの座標
                        Vector2 texPos = hit.textureCoord * tex.width;
                        //その座標のピクセルの色を取得
                        Color pixelColor = tex.GetPixel ((int)texPos.x, (int)texPos.y);

                        //そのピクセルの色と設定れた色が似ていたら
                        if (ColorUtility.WithinRange (originalColor, _objectByColor.closenessColor, pixelColor))
                        {
                            //確率で生成
                            if (Random.value <= _objectByColor.probability)
                            {
                                //生成するオブジェクトをランダムで選ぶ
                                GenerateObject generateObject = _objectByColor.generateObjects[Random.Range (0, _objectByColor.generateObjects.Length)];
                                //オブジェクトを生成
                                GameObject go = Instantiate (generateObject.obj);
                                //子になる
                                go.transform.parent = group.transform;
                                //座標と向きを適用
                                go.transform.position = hit.point;
                                go.transform.up = hit.normal;
                                if (generateObject.randomRotate_AxisFree)
                                {
                                    go.transform.Rotate (Random.Range (0f, 360f), Random.Range (0f, 360f), Random.Range (0f, 360f));
                                }
                                if (generateObject.randomRotate_AxisNormal)
                                {
                                    go.transform.Rotate (go.transform.up, Random.Range (0f, 360f));
                                }
                                //大きさをランダムで
                                go.transform.localScale = new Vector3 (
                                    Random.Range (generateObject.minRandomSize.x, generateObject.maxRandomSize.x),
                                    Random.Range (generateObject.minRandomSize.y, generateObject.maxRandomSize.y),
                                    Random.Range (generateObject.minRandomSize.z, generateObject.maxRandomSize.z));
                                //スタティックにする
                                go.isStatic = true;

                                l++;
                                if (l > 2000)
                                {
                                    l = 0;
                                    yield return null;
                                }
                            }
                        }
                    }
                    else
                    {
                        //レイが当たってない
                        miss++;
                        break;
                    }
                }
                //2回連続でレイが当たってなければ、もう地面はない
                if (miss == 2)
                {
                    break;
                }
            }
        }
        flg = false;
        Debug.Log ("生成完了！");
    }

    //子にいるオブジェクトを全部削除
    public void DestroyChildren ()
    {
        //何回か回さないと全部消えてくれない
        for (int j = 0 ; j < 5 ; j++)
        {
            for (int i = 0 ; i < transform.childCount ; i++)
            {
                DestroyImmediate (transform.GetChild (i).gameObject);
            }
        }
        flg = false;
        Debug.Log ("子オブジェクトを全て削除");
    }

    //生成を中止
    public void StopGenerate ()
    {
        flg = false;
        Debug.Log ("生成を中止");
    }

    //子オブジェクトを非表示にする
    public void HiddenChildren ()
    {
        //何回か回さないと全部消えてくれない
        for (int j = 0 ; j < 5 ; j++)
        {
            for (int i = 0 ; i < transform.childCount ; i++)
            {
                transform.GetChild (i).gameObject.SetActive (false);
            }
        }
    }

    //子オブジェクトを表示する
    public void VisibleChildren ()
    {
        //何回か回さないと全部消えてくれない
        for (int j = 0 ; j < 5 ; j++)
        {
            for (int i = 0 ; i < transform.childCount ; i++)
            {
                transform.GetChild (i).gameObject.SetActive (true);
            }
        }
    }
}

//生成させたい場所の色や、生成距離、生成オブジェクト
[System.Serializable]
public class ObjectByColor
{
    //生成させたい場所の色
    public Color locationColor = Color.green;
    //色の許容範囲
    [Range(0f, 1f)]
    public float closenessColor = 0.1f;
    //隣との距離
    [Range(0.1f, 50f)]
    public float distanceRate = 1f;
    //生成される確率
    [Range (0f, 1f)]
    public float probability = 0.5f;
    //生成オブジェクト
    public GenerateObject[] generateObjects;
}

//生成するオブジェクトのプロパティ
[System.Serializable]
public class GenerateObject
{
    //生成するプレハブ
    public GameObject obj;
    //ランダムサイズの最小値、最大値
    public Vector3 minRandomSize = Vector3.one;
    public Vector3 maxRandomSize = Vector3.one * 5f;
    //ランダムで回転させる際の軸の設定
    //x,y,z全軸でそれぞれランダムに回転する
    public bool randomRotate_AxisFree = true;
    //オブジェクトの上方向を軸に回転
    public bool randomRotate_AxisNormal = false;
}