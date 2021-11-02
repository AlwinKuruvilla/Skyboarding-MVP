using UnityEngine;
using System.Collections;
 
public static class TransformExtensions
{
    // from here https://answers.unity.com/questions/1238142/version-of-transformtransformpoint-which-is-unaffe.html
    
    public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
    {
        var localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        return localToWorldMatrix.MultiplyPoint3x4(position);
    }
 
    public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
    {
        var worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        return worldToLocalMatrix.MultiplyPoint3x4(position);
    }
}