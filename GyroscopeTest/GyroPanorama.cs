using UnityEngine;

public class GyroPanorama : MonoBehaviour
{
    private Quaternion gyroscopeQua;    //陀螺仪四元数

    private Quaternion quatMult = new Quaternion(0, 0, 1, 0);   // 沿着 Z 周旋转的四元数 因子

    private float speedH = 0.2f;    //差值

    protected void Start()

    {

        //激活陀螺仪

        Input.gyro.enabled = true;

        //因安卓设备的陀螺仪四元数水平值为[0,0,0,0]水平向下，所以将相机初始位置修改与其对齐

        transform.eulerAngles = new Vector3(90, 90, 0);

    }

    protected void Update()

    {

        GyroModifyCamera();

    }

    /// <summary>

    /// 检测陀螺仪运动的函数

    /// </summary>

    protected void GyroModifyCamera()

    {

        gyroscopeQua = Input.gyro.attitude * quatMult;      //为球面运算做准备

        Camera.main.transform.localRotation = Quaternion.Slerp(

            Camera.main.transform.localRotation, gyroscopeQua, speedH);

    }

    /// <summary>

    /// Gyro信息打印，真正运行时可以去掉

    /// </summary>

    /* protected void OnGUI()

     {

         GUI.skin.label.fontSize = Screen.width / 40;

         GUILayout.Label("Orientation: " + Screen.orientation);

         GUILayout.Label("input.gyro.eulerAngles: " + Input.gyro.attitude.eulerAngles);

         GUILayout.Label("now eulerAngles: " + Camera.main.transform.eulerAngles);

         GUILayout.Label("iphone width/font: " + Screen.width + " : " + GUI.skin.label.fontSize);

     }*/
}
