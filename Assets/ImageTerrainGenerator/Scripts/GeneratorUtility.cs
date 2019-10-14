using UnityEngine;

public class GeneratorUtility : MonoBehaviour
{
    //ステージ生成用のスクリプトにつける
    //シーンが再生されたら、このコンポーネントをリムーブ
    private void Start ()
    {
        if (Application.isPlaying)
        {
            Destroy (this);
        }
    }
}
