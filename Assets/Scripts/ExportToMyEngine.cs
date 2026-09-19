using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Globalization;

public class ExportToMyEngine : MonoBehaviour
{
    public static ExportToMyEngine Instance;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] List<GameObject> gos;
    [MenuItem("CONTEXT/ExportToMyEngine/ExportData")]
    public static void ExportData()
    {
        string res = "[";
        List<GameObject> gos = Instance.gos;
        for (int i = 0; i < gos.Count; i++)
        {
            MeshFilter mf = gos[i].GetComponent<MeshFilter>();
            MeshRenderer mr = gos[i].GetComponent<MeshRenderer>();

            Transform tr = gos[i].transform;
            bool isQuad = mf.sharedMesh.name == "Quad";

            Quaternion rotation = isQuad ? tr.rotation * Quaternion.Euler(0, 180, 0) : tr.rotation;
            Vector3 euler = ToRasterizerEuler(rotation);

            res += "new GameObject(";
            res += $"new Vector3({toNiceString(tr.position.x)}, {toNiceString(tr.position.y)}, {toNiceString(tr.position.z)}),";
            res += $"new Vector3({toNiceString(euler.x)}, {toNiceString(euler.y)}, {toNiceString(euler.z)}),";
            res += $"new Vector3({toNiceString(tr.lossyScale.x)}, {toNiceString(tr.lossyScale.y)}, {toNiceString(tr.lossyScale.z)}),";
            res += "new Color(0, 255, 0),";
            if (isQuad)
                res += "quadMesh,";
            else
                res += "cubeMesh,";

            if (mr.sharedMaterial.name.Contains("WALL"))
                res += "wallTexture";
            else
                res += "floorTexture";
            res += "),\n";
        }
        res += "]";
        System.IO.File.WriteAllText("exported.txt", res);
    }

    // Unity's eulerAngles cannot be handed to the rasterizer as they are.
    // Quaternion.buildQuaternionEuler over there composes the axes as
    // Rx * Ry * Rz (Unity's Quaternion.Euler is Ry * Rx * Rz) and turns the
    // opposite way around each axis. So instead of copying the angles across,
    // take the finished world rotation and re-decompose it into the angles that
    // rasterizer actually reproduces it from.
    static Vector3 ToRasterizerEuler(Quaternion rotation)
    {
        // Columns of the rotation matrix: where each basis vector ends up.
        Vector3 right = rotation * Vector3.right;
        Vector3 up = rotation * Vector3.up;
        Vector3 forward = rotation * Vector3.forward;

        // Solve rotation == Rx(x) * Ry(y) * Rz(z), then negate to undo the
        // rasterizer's flipped turn direction.
        float x, y, z;
        y = Mathf.Asin(Mathf.Clamp(forward.x, -1f, 1f));
        if (Mathf.Abs(forward.x) < 0.999999f)
        {
            x = Mathf.Atan2(-forward.y, forward.z);
            z = Mathf.Atan2(-up.x, right.x);
        }
        else
        {
            // cos(y) is 0 here, so x and z turn about the same axis and only their
            // sum is defined. Put all of it in x.
            z = 0f;
            x = Mathf.Atan2(Mathf.Sign(forward.x) * right.y, up.y);
        }
        return new Vector3(-x, -y, -z) * Mathf.Rad2Deg;
    }

    static string toNiceString(float value)
    {
        float r = Mathf.Round(value * 10000f) / 10000f;
        if (r == 0f) r = 0f;
        return r.ToString("0.####", CultureInfo.InvariantCulture);
    }
}
