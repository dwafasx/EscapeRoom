using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private Animator animator;
    public float walkSpeed = 2.0f;//,既是融合树Speed行走时数值，又是角色行走速度
    public float runSpeed = 5.0f;//同上

    public float acceleration = 3f; // 融合树Speed变量改变时的加速度
    public float deceleration = 3f; // 融合树Speed变量改变时的减速度
    private float currentSpeed = 0f;

    float herizontalMouseSum=0;    //鼠标水平移动累加
    float verticalMouseSum = 0;//鼠标垂直移动累加
    float minCameraAngle=-60f;
    float maxCameraAngle=60f;
    

    void Start()
    {
        //锁定鼠标到屏幕中心并隐藏
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponent<Animator>();
        
    }

    void Update()
    {
        Move();
        roleCamera();
    }



    void Move()
    {
        bool isRunning=false;
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        if (verticalInput > 0.01)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);
        }
        
        float targetSpeed = 0f;//anim组件中融合树Speed目标数值
        if (Mathf.Abs(verticalInput) > 0.01f || Mathf.Abs(horizontalInput) > 0.01f) // 降低阈值
        {
            targetSpeed = isRunning ? runSpeed : walkSpeed;
        }

        // 平滑调整当前速度
        if (targetSpeed > currentSpeed)
        {
            currentSpeed = Mathf.Min(targetSpeed, currentSpeed + acceleration * Time.deltaTime);
        }
        else if (targetSpeed < currentSpeed)
        {
            currentSpeed = Mathf.Max(targetSpeed, currentSpeed - deceleration * Time.deltaTime);
        }

        animator.SetFloat("Speed", currentSpeed);//设置融合树Speed
        transform.Translate(new Vector3(horizontalInput * targetSpeed * Time.deltaTime, 0, verticalInput * targetSpeed * Time.deltaTime));

    }
    void roleCamera()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        herizontalMouseSum += horizontal;//鼠标水平移动累加
        verticalMouseSum += vertical; //鼠标垂直移动累加
        transform.localRotation=Quaternion.Euler(Vector3.up*herizontalMouseSum);//角色水平旋转
        //限制相机角度
        verticalMouseSum = Mathf.Clamp(verticalMouseSum, minCameraAngle, maxCameraAngle);
        //相机旋转
        Camera.main.transform.localRotation=Quaternion.Euler(-verticalMouseSum,0,0);
    }
}