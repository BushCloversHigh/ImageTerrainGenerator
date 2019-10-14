using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ImageMapTerrain))]
public class ImageMapTerrainEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI ();

        //targetを変換して対象を取得
        ImageMapTerrain script = target as ImageMapTerrain;

        //PublicMethodを実行する用のボタン
        if (GUILayout.Button ("Terrain generate"))
        {
            script.Generate ();
        }
    }
}
