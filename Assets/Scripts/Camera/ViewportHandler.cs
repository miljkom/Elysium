using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent (typeof (Camera))]
public class ViewportHandler : MonoBehaviour
{
    #region FIELDS
    [SerializeField] private SpriteRenderer mainObjectBackground;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private BoxCollider boundaryCollider;
    public static ViewportHandler Instance;
    public new Camera camera;

    private float _width;
    private float _height;
    //*** bottom screen
    private Vector3 _bl;
    private Vector3 _bc;
    private Vector3 _br;
    //*** middle screen
    private Vector3 _ml;
    private Vector3 _mc;
    private Vector3 _mr;
    //*** top screen
    private Vector3 _tl;
    private Vector3 _tc;
    private Vector3 _tr;
    #endregion

    #region PROPERTIES
    public float Width {
        get {
            return _width;
        }
    }
    public float Height {
        get {
            return _height;
        }
    }

    // helper points:
    public Vector3 BottomLeft {
        get {
            return _bl;
        }
    }
    public Vector3 BottomCenter {
        get {
            return _bc;
        }
    }
    public Vector3 BottomRight {
        get {
            return _br;
        }
    }
    public Vector3 MiddleLeft {
        get {
            return _ml;
        }
    }
    public Vector3 MiddleCenter {
        get {
            return _mc;
        }
    }
    public Vector3 MiddleRight {
        get {
            return _mr;
        }
    }
    public Vector3 TopLeft {
        get {
            return _tl;
        }
    }
    public Vector3 TopCenter {
        get {
            return _tc;
        }
    }
    public Vector3 TopRight {
        get {
            return _tr;
        }
    }
    #endregion

    #region METHODS
    private void Awake()
    {
        camera = GetComponent<Camera>();
        Instance = this;
        ComputeResolution();
        SetBoundaryForCamera();
    }

    private void ComputeResolution()
    {
        float leftX, rightX, topY, bottomY;

        var newOrthographicSize = mainObjectBackground.bounds.size.x * Screen.height / Screen.width * 0.5f;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = (newOrthographicSize > 9) ? newOrthographicSize : 9;

        _height = 2f * cinemachineVirtualCamera.m_Lens.OrthographicSize;
        _width = _height * cinemachineVirtualCamera.m_Lens.Aspect;

        float cameraX, cameraY;
        cameraX = camera.transform.position.x;
        cameraY = camera.transform.position.y;

        leftX = cameraX - _width / 2;
        rightX = cameraX + _width / 2;
        topY = cameraY + _height / 2;
        bottomY = cameraY - _height / 2;
        
        //*** bottom
        _bl = new Vector3(leftX, bottomY, 0);
        _bc = new Vector3(cameraX, bottomY, 0);
        _br = new Vector3(rightX, bottomY, 0);
        //*** middle
        _ml = new Vector3(leftX, cameraY, 0);
        _mc = new Vector3(cameraX, cameraY, 0);
        _mr = new Vector3(rightX, cameraY, 0);
        //*** top
        _tl = new Vector3(leftX, topY, 0);
        _tc = new Vector3(cameraX, topY , 0);
        _tr = new Vector3(rightX, topY, 0);           
    }
    private void Update()
    {
        #if UNITY_EDITOR
        ComputeResolution();
        #endif
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.white;

        Matrix4x4 temp = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        if (camera.orthographic) {
            float spread = camera.farClipPlane - camera.nearClipPlane;
            float center = (camera.farClipPlane + camera.nearClipPlane)*0.5f;
            Gizmos.DrawWireCube(new Vector3(0,0,center), new Vector3(camera.orthographicSize*2*camera.aspect, camera.orthographicSize*2, spread));
        } else {
            Gizmos.DrawFrustum(Vector3.zero, camera.fieldOfView, camera.farClipPlane, camera.nearClipPlane, camera.aspect);
        }
        Gizmos.matrix = temp;
    }

    private void SetBoundaryForCamera()
    {
        boundaryCollider.gameObject.SetActive(true);
        var cameraYDistance = Vector3.Distance(BottomCenter, TopCenter);
        boundaryCollider.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y + cameraYDistance, 0);
        boundaryCollider.size = new Vector3(14f, cameraYDistance * 3f, 50f); // 50 da ne bi kamera snepovala poziciju na Z osi
    }
    #endregion

    public enum Constraint { Landscape, Portrait }
}