using UnityEngine;

public class Fire : MonoBehaviour
{
    /*陀螺仪部分*/
    private Quaternion gyroscopeQua;    //陀螺仪四元数
    private Quaternion quatMult = new Quaternion(0, 0, 1, 0);   // 沿着 Z 周旋转的四元数 因子
    private float speedH = 0.2f;    //差值


    /*火焰部分，记录初始参数值，方便还原*/
    ParticleSystem particleSystem;
    ParticleSystem.ForceOverLifetimeModule forceMode;
    float ChangeTime = 5f;//火苗随机的间隔
    int FireFlag_x = 0;//随机火焰方向
    int FireFlag_y = 0;
    float FireForce_x = 0;//风吹力度
    float FireForce_y = 0;
    ParticleSystem.MinMaxCurve origin_x;
    ParticleSystem.MinMaxCurve origin_y;
    float origin_stratSize;
    float origin_startLifetime;
    float Fire_z = 0f;//火焰角度
    // Use this for initialization
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        forceMode = particleSystem.forceOverLifetime;
        //记录初始参数值，方便还原
        ParticleSystem.MinMaxCurve origin_x = forceMode.x;
        ParticleSystem.MinMaxCurve origin_y = forceMode.y;
        origin_stratSize = particleSystem.startSize;
        origin_startLifetime = particleSystem.startLifetime;

        /*
        Debug.Log(forceMode.x.constantMax);
        Debug.Log(forceMode.y.constantMax);
        Debug.Log(particleSystem.startSize);
        Debug.Log(particleSystem.startLifetime);
        Debug.Log(origin_stratSize);
        Debug.Log(origin_startLifetime);
        */


        //激活陀螺仪
        Input.gyro.enabled = true;

        //因安卓设备的陀螺仪四元数水平值为[0,0,0,0]水平向下，所以将相机初始位置修改与其对齐
        transform.eulerAngles = new Vector3(90, 90, 0);


    }

    // Update is called once per frame

    void Update()
    {
        GyroModifyCamera();

        ChangeTime = ChangeTime - Time.deltaTime; //倒计时

        //Debug.Log("ChangeTime = " + ChangeTime.ToString());
        Shelter();
        if (ChangeTime <= 0)
        {
            ChangeTime = Random.Range(6, 8);//随机一个新的时间
            FireFlag_x = Random.Range(3, 5);//前后
            //FireFlag_x = 3;
            FireForce_x = Random.Range(0.5f, 16f);//力度
            FireFlag_y = Random.Range(1, 3);//左右
            //FireFlag_y = 1;
            FireForce_y = Random.Range(0.5f, 16f);
            //Debug.Log("FireFlag = " + FireFlag.ToString());
            ChangeFire(0);
            ChangeFire(FireFlag_x, FireForce_x);
            ChangeFire(FireFlag_y, FireForce_y);
        }



    }

    /*void OnGUI()
    {
        if (GUI.Button(new Rect(10, 200, 80, 50), "left"))
        {
            ParticleSystem.MinMaxCurve temp = forceMode.x;
            temp.constantMax = temp.constantMax - 2.0f;
            forceMode.x = temp;
        }

        if (GUI.Button(new Rect(10, 260, 80, 50), "right"))
        {
            ParticleSystem.MinMaxCurve temp = forceMode.x;
            temp.constantMax = temp.constantMax + 2.0f;
            forceMode.x = temp;
        }

        if (GUI.Button(new Rect(10, 320, 80, 50), "big"))
        {
            particleSystem.startSize = particleSystem.startSize * 1.5f;
            particleSystem.startLifetime = particleSystem.startLifetime * 1.5f;
        }

        if (GUI.Button(new Rect(10, 380, 80, 50), "small"))
        {
            particleSystem.startSize = particleSystem.startSize * 0.5f;
            particleSystem.startLifetime = particleSystem.startLifetime * 0.5f;
        }
    }*/

    void ChangeFire(int FireFlag, float FireForce = 12.0f, float FireSize = 1f)//控制火焰方向和大小
    {
        if (FireFlag == 0)//复原
        {
            RestoreFire();
        }
        else if (FireFlag == 1)
        {
            ParticleSystem.MinMaxCurve temp = forceMode.x;
            temp.constantMax = temp.constantMax + FireForce;
            forceMode.x = temp;
        }
        else if (FireFlag == 2)
        {
            ParticleSystem.MinMaxCurve temp = forceMode.x;
            temp.constantMax = temp.constantMax - FireForce;
            forceMode.x = temp;
        }
        else if (FireFlag == 3)
        {
            ParticleSystem.MinMaxCurve temp = forceMode.y;
            temp.constantMax = temp.constantMax - FireForce;
            forceMode.y = temp;
        }
        else if (FireFlag == 4)
        {
            ParticleSystem.MinMaxCurve temp = forceMode.y;
            temp.constantMax = temp.constantMax + FireForce;
            forceMode.y = temp;
        }
        else if (FireFlag == 5)//调整火焰大小
        {
            particleSystem.startSize = particleSystem.startSize * FireSize;
            particleSystem.startLifetime = particleSystem.startLifetime * FireSize;
        }

    }

    void RestoreFire()//将火焰恢复原状
    {
        forceMode.x = origin_x;
        forceMode.y = origin_y;
        particleSystem.startSize = origin_stratSize;
        particleSystem.startLifetime = origin_startLifetime;
    }

    void Shelter()//挡风
    {
        Fire_z = Mathf.Atan2(FireForce_y * (FireFlag_y - 1.5f), FireForce_x * (FireFlag_x - 3.5f)) * 180f / 3.1416f + 180;
        float delta_z = Mathf.Abs(Fire_z - Input.gyro.attitude.eulerAngles.z);
        if (delta_z < 10 || delta_z > 350)
        {
            RestoreFire();
        }
    }


    protected void GyroModifyCamera()//计算相机角度
    {
        gyroscopeQua = Input.gyro.attitude * quatMult;      //为球面运算做准备

        Camera.main.transform.localRotation = Quaternion.Slerp(

            Camera.main.transform.localRotation, gyroscopeQua, speedH);

    }

    protected void OnGUI()//显示参数
    {

        GUI.skin.label.fontSize = Screen.width / 40;

        GUILayout.Label("Orientation: " + Screen.orientation);

        GUILayout.Label("input.gyro.eulerAngles: " + Input.gyro.attitude.eulerAngles);

        GUILayout.Label("now eulerAngles: " + Camera.main.transform.eulerAngles);

        GUILayout.Label("iphone width/font: " + Screen.width + " : " + GUI.skin.label.fontSize);

        GUILayout.Label("FireFlag_x: " + FireFlag_x);

        GUILayout.Label("FireFlag_y: " + FireFlag_y);

        GUILayout.Label("FireForce_x: " + FireForce_x);

        GUILayout.Label("FireForce_y: " + FireForce_y);

        GUILayout.Label("Fire_z: " + Fire_z);

    }

}
