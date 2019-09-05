using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(SkewedImage), true)]
[CanEditMultipleObjects]
public class SkewedImageEditor : ImageEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI(); // まず本来のImage用エディターを表示する
        var targetSkewedImage = this.target as SkewedImage;
        if (targetSkewedImage != null)
        {
            var prevSkew = targetSkewedImage.Skew; // 編集前のスキュー
            var newSkew = EditorGUILayout.Vector2Field("Skew", prevSkew); // 編集後のスキュー
            if (newSkew != prevSkew) // 値の編集が行われたならば...
            {
                targetSkewedImage.Skew = newSkew; // スキューを新しい値に更新し...
                targetSkewedImage.SetVerticesDirty(); // メッシュの再生成が必要なことを知らせる
            }
        }
    }
}