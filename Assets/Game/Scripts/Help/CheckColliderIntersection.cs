using UnityEngine;

public static class CheckColliderIntersection
{
    //public static bool CheckIntersection(Collider collider, LayerMask layerMask)
    //{
    //    if (collider == null) return false;

    //    Collider[] results = null;

    //    if (collider is BoxCollider box)
    //    {
    //        Vector3 worldCenter = box.transform.TransformPoint(box.center);
    //        Vector3 halfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);

    //        results = Physics.OverlapBox(
    //            worldCenter,
    //            halfExtents,
    //            box.transform.rotation,
    //            layerMask
    //        );
    //    }
    //    else if (collider is SphereCollider sphere)
    //    {
    //        results = Physics.OverlapSphere(
    //            sphere.bounds.center,
    //            sphere.radius * Mathf.Max(
    //                sphere.transform.lossyScale.x,
    //                sphere.transform.lossyScale.y,
    //                sphere.transform.lossyScale.z),
    //            layerMask);
    //    }

    //    foreach (var hit in results)
    //    {
    //        if (hit != collider) 
    //            return true;
    //    }

    //    return false;
    //}
}