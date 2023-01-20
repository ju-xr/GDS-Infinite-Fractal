using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json.Bson;
using System.Security.Cryptography;
using UnityEngine.UIElements;


//code reference: https://github.com/sjoerdev/mandelbulb
//[ExecuteInEditMode]
public class ShaderSetup : MonoBehaviour
{
    //Public variables
    public enum MandelSet
    {
        Mandelbrot,
        Mandelbulb
    }

    public MandelSet mandelset;

    #region Mandelbulb Property
    private Material material_Mandelbulb;

    [Header("sphere marching")]
    [Range(1, 400)]
    public float maxDst = 200;
    [Range(0.01f, 0.0001f)]
    public float epsilon = 0.003f;

    [Header("mandelbulb")]
    [Range(0f, 8f)]
    public float MandelbulbPower = 8;
    [Range(2, 100)]
    public int MandelbulbIterations = 50;

    [Header("colors")]
    [Range(0f, 1f)]
    public float Hue = 0.72f;
    public bool NormalsAsColor = false;

    [Header("lighting and shadows")]
    public Transform lightdirection;
    //public Texture SkyboxTexture;

    [Space(20)]

    public bool SoftShadows = true;
    [Range(0.1f, 1f)]
    public float ShadowAccuracy = 1.0f;
    [Range(128, 1024)]
    public float ShadowSharpness = 512;

    [Space(20)]

    public bool AmbientOcclusion = true;

    [Space(20)]

    public bool Reflections = true;
    [Range(0.0f, 1f)]
    public float reflectiveness = 0.5f;

    private bool UpdatePowerOverTime = false;
    float UpdatedValue = 2;
    bool GoingUp = true;

    #endregion

    #region Mandelbrot Property
    [Header("Mandelbrot")]
    private Material material_Mandelbrot;
    public Vector2 position;
    public float scale_Mandelbrot, angle_Mandelbrot;
    public float color_Mandelbrot;
    public float symmetry_Mandelbrot;
    public int iteration_Mandelbrot;
    public float speed_Mandelbrot;

    private Vector2 smoothPos;
    private float smoothScale, smoothAngle;

    #endregion

    private Camera c;
    public float zoomSpeed;
    public float rotateSpeed = 2;

    void Start()
    {
        // cartesian space c0.1oordinates of start screen
        Init();
    }

    void Init()
    {
        // set all public variables in the inspector (and in the UI elements)
        c = this.GetComponent<Camera>();
    }

    private void Update()
    {
        if (mandelset == MandelSet.Mandelbulb)
        {
            MandelbulbControl();
        }
        if (mandelset == MandelSet.Mandelbrot)
        {
            MandelbrotControl();
        }
        cameraControl();
    }

    void cameraControl()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //zoomin
            if (mandelset == MandelSet.Mandelbrot)
            {
                scale_Mandelbrot *= 0.95f;
            }
            if (mandelset == MandelSet.Mandelbulb)
            {
                c.fieldOfView -= Time.deltaTime * zoomSpeed;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (mandelset == MandelSet.Mandelbrot)
            {
                scale_Mandelbrot *= 1.05f;
            }
            if (mandelset == MandelSet.Mandelbulb)
            {
                // zoomout
                c.fieldOfView += Time.deltaTime * zoomSpeed;
            }

        }
        if (Input.GetMouseButton(1))
        {
            gameObject.transform.RotateAround(Vector3.zero, Vector3.up, Input.GetAxis("Mouse X") * rotateSpeed);
            gameObject.transform.RotateAround(Vector3.zero, -gameObject.transform.right, Input.GetAxis("Mouse Y") * rotateSpeed);
        }
    }

    void MandelbulbControl()
    {
        #region fixed set
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            maxDst = 200;
            epsilon = 0.003f;

            MandelbulbPower = 8;
            MandelbulbIterations = 50;

            Hue = 0.72f;
            NormalsAsColor = false;

            SoftShadows = true;
            ShadowAccuracy = 1.0f;
            ShadowSharpness = 512;
            AmbientOcclusion = true;
            Reflections = true;
            reflectiveness = 0.5f;

            UpdatePowerOverTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            maxDst = 400;
            epsilon = 0.001f;

            MandelbulbPower = 3;
            MandelbulbIterations = 50;

            Hue = 0.4f;
            NormalsAsColor = true;

            SoftShadows = false;
            ShadowAccuracy = 1.0f;
            ShadowSharpness = 512;
            AmbientOcclusion = true;
            Reflections = false;
            reflectiveness = 0.5f;

            UpdatePowerOverTime = false;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            maxDst = 400;
            epsilon = 0.0001f;

            MandelbulbPower = 2;
            MandelbulbIterations = 50;

            Hue = 0.25f;
            NormalsAsColor = false;

            SoftShadows = false;
            ShadowAccuracy = 1.0f;
            ShadowSharpness = 512;
            AmbientOcclusion = true;
            Reflections = false;
            reflectiveness = 0.5f;

            UpdatePowerOverTime = true;
        }
        #endregion

        if (UpdatedValue >= 8)
        {
            GoingUp = false;
        }
        if (UpdatedValue <= 2)
        {
            GoingUp = true;
        }

        if (GoingUp)
        {
            UpdatedValue += 0.5f * Time.deltaTime;
        }
        if (!GoingUp)
        {
            UpdatedValue -= 0.5f * Time.deltaTime;
        }

        if (UpdatePowerOverTime)
        {
            MandelbulbPower = UpdatedValue;
        }
        else
        {
            UpdatedValue = 2;
        }
    }

    void MandelbrotControl()
    {

        if (Input.GetKey(KeyCode.E))
            angle_Mandelbrot += 0.01f;
        if (Input.GetKey(KeyCode.Q))
            angle_Mandelbrot -= 0.01f;

        Vector2 dir = new Vector2(0.01f * scale_Mandelbrot, 0);
        float s = Mathf.Sin(angle_Mandelbrot);
        float c = Mathf.Cos(angle_Mandelbrot);
        dir = new Vector2(dir.x * c, dir.x * s);

        if (Input.GetKey(KeyCode.A))
            position -= dir;
        if (Input.GetKey(KeyCode.D))
            position += dir;

        dir = new Vector2(-dir.y, dir.x);
        if (Input.GetKey(KeyCode.S))
            position -= dir;
        if (Input.GetKey(KeyCode.W))
            position += dir;
    }

    void Awake()
    {
        material_Mandelbulb = new Material(Shader.Find("Ju/Mandelbulb_S"));
        material_Mandelbrot = new Material(Shader.Find("Ju/Mandelbrot_S"));
    }

    #region Mandelbrot_UI
    public void SetIterations_Mandelbrot(float n)
    {
        iteration_Mandelbrot = (int)n;
    }

    public void SetColor_Mandelbrot(float n)
    {
        color_Mandelbrot = n;
    }

    public void SetSyemmetry(float n)
    {
        symmetry_Mandelbrot = n;
    }

    public void SetSpeed(float n)
    {
        speed_Mandelbrot = n;
    }
    #endregion

    #region Mandelbulb_UI
    public void SetMaxDist(float n)
    {
        maxDst = n;
    }

    public void SetEpsilon(float n)
    {
        epsilon = n;
    }

    public void SetMandelbulbPower(float n)
    {
        MandelbulbPower = n;
    }

    public void SetIterations_Mandelbulb(float n)
    {
        MandelbulbIterations = (int)n;
    }

    public void SetHue(float n)
    {
        Hue = n;
    }

    #endregion

    void SetMandelbrotParameters()
    {
        //Movement smoothly
        smoothPos = Vector2.Lerp(smoothPos, position, 0.03f);
        smoothScale = Mathf.Lerp(smoothScale, scale_Mandelbrot, 0.03f);
        smoothAngle = Mathf.Lerp(smoothAngle, angle_Mandelbrot, 0.03f);

        //aspect ratio adjustment
        float aspect = (float)Screen.width / (float)Screen.height;
        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1f)
            scaleY /= aspect;
        else
            scaleX *= aspect;

        material_Mandelbrot.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
        material_Mandelbrot.SetFloat("_Angle", smoothAngle);
        material_Mandelbrot.SetFloat("_Color", color_Mandelbrot);
        material_Mandelbrot.SetFloat("_Symmetry", symmetry_Mandelbrot);
        material_Mandelbrot.SetFloat("_Speed", speed_Mandelbrot);
        material_Mandelbrot.SetInt("_MaxIter", iteration_Mandelbrot);
    }

    void SetMandelbulbParameters()
    {
        if (lightdirection == null)
        {
            lightdirection = GameObject.FindGameObjectWithTag("MainLight").transform;
        }

        //material.SetTexture("_SkyboxTexture", SkyboxTexture);
        material_Mandelbulb.SetMatrix("_CTW", Camera.main.cameraToWorldMatrix);
        material_Mandelbulb.SetMatrix("_PMI", Camera.main.projectionMatrix.inverse);
        material_Mandelbulb.SetFloat("maxDst", maxDst);
        material_Mandelbulb.SetFloat("epsilon", epsilon);
        material_Mandelbulb.SetFloat("MandelbulbPower", MandelbulbPower);
        material_Mandelbulb.SetInt("MandelbulbIterations", MandelbulbIterations);
        material_Mandelbulb.SetInt("NormalsAsColor", Convert.ToInt32(NormalsAsColor));
        material_Mandelbrot.SetVector("rgb", new Vector3(Color.HSVToRGB(Hue, 0.2f, 1).g, Color.HSVToRGB(Hue, 0.2f, 1).r, Color.HSVToRGB(Hue, 0.2f, 1).b));
        material_Mandelbulb.SetFloat("skyboxBrightness", 1);
        material_Mandelbulb.SetVector("lightdirection", Quaternion.Euler(lightdirection.rotation.eulerAngles) * Vector3.back);
        material_Mandelbulb.SetFloat("shadowbias", epsilon / ShadowAccuracy);
        material_Mandelbulb.SetFloat("k", ShadowSharpness);
        material_Mandelbulb.SetInt("SoftShadows", Convert.ToInt32(SoftShadows));
        material_Mandelbulb.SetInt("HardShadows", 0);
        material_Mandelbulb.SetInt("AmbientOcclusion", Convert.ToInt32(AmbientOcclusion));
        material_Mandelbulb.SetInt("Reflections", Convert.ToInt32(Reflections));
        material_Mandelbulb.SetFloat("reflectiveness", reflectiveness);
    }

    public void mandelSet(int i)
    {
        if(i == 0)
        {
            mandelset = MandelSet.Mandelbrot;
        }
        if(i == 1)
        {
            mandelset = MandelSet.Mandelbulb;
        }
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        switch (mandelset)
        {
            //case MandelSet.None:
            //    break;
            case MandelSet.Mandelbrot:
                SetMandelbrotParameters();
                Graphics.Blit(source, destination, material_Mandelbrot);
                break;
            case MandelSet.Mandelbulb:
                SetMandelbulbParameters();
                Graphics.Blit(source, destination, material_Mandelbulb);
                break;
        }
    }

}
