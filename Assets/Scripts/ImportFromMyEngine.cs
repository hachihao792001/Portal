using UnityEngine;
using UnityEditor;
using System.Globalization;
using System.Text.RegularExpressions;

public class ImportFromMyEngine : MonoBehaviour
{
    public static ImportFromMyEngine Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] Material wallMaterial;
    [SerializeField] Material floorMaterial;

    static readonly Regex entry = new Regex(
        @"new\s+GameObject\(\s*" +
        @"new\s+Vector3\(([^)]*)\)\s*,\s*" +
        @"new\s+Vector3\(([^)]*)\)\s*,\s*" +
        @"new\s+Vector3\(([^)]*)\)\s*,\s*" +
        @"new\s+Color\([^)]*\)\s*,\s*" +
        @"(\w+)\s*,\s*" +
        @"(\w+)\s*\)",
        RegexOptions.Compiled);

    [MenuItem("CONTEXT/ImportFromMyEngine/ImportData")]
    public static void ImportData()
    {
        ImportFromMyEngine inst = Instance;
        Transform root = new GameObject("Imported").transform;
        root.SetParent(inst.transform, false);

        string data = System.IO.File.ReadAllText("input.txt");

        int count = 0;
        foreach (Match m in entry.Matches(data))
        {
            Vector3 position = ParseVector3(m.Groups[1].Value);
            Vector3 euler = ParseVector3(m.Groups[2].Value);
            Vector3 scale = ParseVector3(m.Groups[3].Value);
            bool isQuad = m.Groups[4].Value == "quadMesh";
            bool isWall = m.Groups[5].Value == "wallTexture";

            GameObject go = GameObject.CreatePrimitive(isQuad ? PrimitiveType.Quad : PrimitiveType.Cube);
            go.name = (isQuad ? "Quad" : "Cube") + "_" + count;
            go.transform.SetParent(root, false);

            Quaternion rotation = FromRasterizerEuler(euler);
            // Export turned quads around before writing them out; undo that here.
            rotation = isQuad ? rotation * Quaternion.Euler(0, -180, 0) : rotation;

            // Composing the quaternions leaves float noise behind (89.99999 in
            // place of 90, 3.35E-13 in place of 0), so clean the angles up before
            // handing them to the transform.
            go.transform.rotation = Quaternion.Euler(Round(rotation.eulerAngles));
            go.transform.position = Round(position);
            go.transform.localScale = Round(scale);

            Material mat = isWall ? inst.wallMaterial : inst.floorMaterial;
            if (mat != null)
                go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            count++;
        }

        Undo.RegisterCreatedObjectUndo(root.gameObject, "Import Data");
        Debug.Log($"Imported {count} objects.");
    }

    // Inverse of Export.ToRasterizerEuler: the rasterizer composes its axes as
    // Rx * Ry * Rz and turns the opposite way around each one, so negate the
    // angles and rebuild the world rotation in that order.
    static Quaternion FromRasterizerEuler(Vector3 euler)
    {
        return Quaternion.Euler(-euler.x, 0, 0)
             * Quaternion.Euler(0, -euler.y, 0)
             * Quaternion.Euler(0, 0, -euler.z);
    }

    static Vector3 Round(Vector3 v)
    {
        return new Vector3(Round(v.x), Round(v.y), Round(v.z));
    }

    static float Round(float v)
    {
        float r = Mathf.Round(v * 10000f) / 10000f;
        if (r == 0f) r = 0f; // collapse -0
        return r;
    }

    static Vector3 ParseVector3(string s)
    {
        string[] parts = s.Split(',');
        return new Vector3(ParseFloat(parts[0]), ParseFloat(parts[1]), ParseFloat(parts[2]));
    }

    static float ParseFloat(string s)
    {
        return float.Parse(s.Trim(), CultureInfo.InvariantCulture);
    }
}
