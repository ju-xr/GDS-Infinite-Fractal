using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class MandelbrotSet_Control_S : MonoBehaviour
{

    public Material mat;
    public Vector2 position;
    public float scale, angle;

    private Vector2 smoothPos;
    private float smoothScale, smoothAngle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        InputControl();
        //UpdateShader();
        
    }

    private void InputControl()
    {
        if (Input.GetKey(KeyCode.O))
            scale *= 0.99f;
        if (Input.GetKey(KeyCode.P))
            scale *= 1.01f;
        if (Input.GetKey(KeyCode.E))
            angle -= 0.01f;
        if (Input.GetKey(KeyCode.Q))
            angle += 0.01f;

        Vector2 dir = new Vector2(0.01f * scale, 0);
        float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);
        dir = new Vector2(dir.x*c, dir.x*s);

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


    //private void UpdateShader()
    //{
    //    //Movement smoothly
    //    smoothPos = Vector2.Lerp(smoothPos, position, 0.03f);
    //    smoothScale = Mathf.Lerp(smoothScale, scale, 0.03f);
    //    smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.03f);

    //    //aspect ratio adjustment
    //    float aspect = (float)Screen.width / (float)Screen.height;
    //    float scaleX = smoothScale;
    //    float scaleY = smoothScale;

    //    if (aspect > 1f)
    //        scaleY /= aspect;
    //    else
    //        scaleX *= aspect;

    //    mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
    //    mat.SetFloat("_Angle", smoothAngle);
    //}

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        //Movement smoothly
        smoothPos = Vector2.Lerp(smoothPos, position, 0.03f);
        smoothScale = Mathf.Lerp(smoothScale, scale, 0.03f);
        smoothAngle = Mathf.Lerp(smoothAngle, angle, 0.03f);

        //aspect ratio adjustment
        float aspect = (float)Screen.width / (float)Screen.height;
        float scaleX = smoothScale;
        float scaleY = smoothScale;

        if (aspect > 1f)
            scaleY /= aspect;
        else
            scaleX *= aspect;

        mat.SetVector("_Area", new Vector4(smoothPos.x, smoothPos.y, scaleX, scaleY));
        mat.SetFloat("_Angle", smoothAngle);

        Graphics.Blit(src, dst, mat);
    }

}
