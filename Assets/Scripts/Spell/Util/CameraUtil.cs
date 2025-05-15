using UnityEngine;

public static class CameraUtil
{
    public static Vector3 GetCameraTargetPosition(float maxDistance = 100f)
    {
        Camera cam = Camera.main;
        if (cam == null) return Vector3.zero;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            return hit.point;
        return cam.transform.position + cam.transform.forward * maxDistance;
    }

    public static Vector3 GetCameraForward()
    {
        Camera cam = Camera.main;
        return cam != null ? cam.transform.forward : Vector3.forward;
    }
}
