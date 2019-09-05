using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class SkewedImage : Image
{
    public Vector2 Skew; // 水平・垂直スキューのタンジェント...たとえば(0, 1)とすると、X成分の増加に傾き1で比例してYも増加する→X軸が45°傾く

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        // 本来のImageの頂点を作成させる
        base.OnPopulateMesh(toFill);

        // Imageが作成した頂点を(むりやり)取得
        var positionsInfo = typeof(VertexHelper).GetField(
            "m_Positions",
            BindingFlags.Instance | BindingFlags.NonPublic);
        var positions = positionsInfo.GetValue(toFill) as List<Vector3>;
        var positionCount = positions == null ? 0 : positions.Count;

        // スキュー変換行列を作成
        var skewMatrix = Matrix4x4.identity;
        skewMatrix.m01 = this.Skew.x;
        skewMatrix.m10 = this.Skew.y;

        // 全頂点にスキュー変換を適用
        for (var i = 0; i < positionCount; i++)
        {
            positions[i] = skewMatrix.MultiplyPoint(positions[i]);
        }
    }
}