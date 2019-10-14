using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (RandomEnvironmentGenerator))]
public class RandomEnvironmentGeneratorEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        //元のInspector部分を表示
        base.OnInspectorGUI ();

        //targetを変換して対象を取得
        RandomEnvironmentGenerator script = target as RandomEnvironmentGenerator;

        if (GUILayout.Button ("Objects generate"))
        {
            script.Generate ();
        }

        if (GUILayout.Button ("Stop generate"))
        {
            script.StopGenerate ();
        }

        if (GUILayout.Button ("Hidden children"))
        {
            script.HiddenChildren ();
        }

        if (GUILayout.Button ("Visible children"))
        {
            script.VisibleChildren ();
        }

        if (GUILayout.Button ("Destroy children"))
        {
            script.DestroyChildren ();
        }
    }
}
