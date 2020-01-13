using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BlackHoleManager : MonoBehaviour
{
    public Material m_Mat = null;
    [Range(0.01f, 0.2f)] public float m_DarkRange = 0.1f;
    [Range(-2.5f, -1f)] public float m_Distortion = -2f;
    public Transform BlackHolePositionObject;
    Vector3 wtsp;
    Vector2 pos;
    Camera cam;
    int m_ID_Center = 0;
    int m_ID_DarkRange = 0;
    int m_ID_Distortion = 0;
    void OnEnable()
    {
        cam = GetComponent<Camera>();
    }
    void Start()
    {
        if (!SystemInfo.supportsImageEffects)
            enabled = false;
        m_ID_Center = Shader.PropertyToID("_Center");
        m_ID_DarkRange = Shader.PropertyToID("_DarkRange");
        m_ID_Distortion = Shader.PropertyToID("_Distortion");
    }
    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        if (m_Mat && BlackHolePositionObject)
        {
            wtsp = cam.WorldToScreenPoint(BlackHolePositionObject.position);
            pos = new Vector2(wtsp.x / cam.pixelWidth, wtsp.y / cam.pixelHeight);
            m_Mat.SetVector(m_ID_Center, pos);
            m_Mat.SetFloat(m_ID_DarkRange, m_DarkRange);
            m_Mat.SetFloat(m_ID_Distortion, m_Distortion);
            Graphics.Blit(sourceTexture, destTexture, m_Mat);

        }

    }

    void OnDisable()
    {
        if (m_Mat)
            DestroyImmediate(m_Mat);
    }
}