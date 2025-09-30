using System;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    [Header("移动设置")]
    public float walkSpeed = 2.0f;//,既是融合树Speed行走时数值，又是角色行走速度
    public float runSpeed = 5.0f;//同上
    public float acceleration; // 融合树Speed变量改变时的加速度
    public float deceleration; // 融合树Speed变量改变时的减速度
    private float currentSpeed = 0f;
    float herizontalMouseSum=0;    //鼠标水平移动累加
    float verticalMouseSum = 0;//鼠标垂直移动累加
    float minCameraAngle=-60f;//摄像机最小角度
    float maxCameraAngle=60f;//摄像机最大角度
    [Header("跳跃设置")]
    public Transform groundCheck;//地面检测的空物体的位置
    public float checkRadius =1.3f;//检测半径
    public bool isGrounded=false;
    public LayerMask groundMask;
    public float gravity;
    public float jumpForce;
    private Vector3 velocity=Vector3.zero;
    void Start()
    {
        //锁定鼠标到屏幕中心并隐藏
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        animator = GetComponent<Animator>();
        characterController=GetComponent<CharacterController>();
        if(groundCheck == null)
        {
            groundCheck = transform;
        }
    }

    void Update()
    {
        GroundCheck();//地面检测
        ApplyGravity();//应用重力
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
            Debug.Log(velocity.x+"  "+ velocity.y + "  "+velocity.z);
        characterController.Move(velocity*0.01f);//角色跳跃
        Move();
        RoleCamera();
    }

    private void Jump()
    {
        velocity.y =  Mathf.Sqrt(2 * gravity * jumpForce);
    }
    private void ApplyGravity()
    {
        if (!isGrounded){
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        //isGrounded = characterController.isGrounded;
        isGrounded=Physics.CheckSphere(groundCheck.position,checkRadius,groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
    }

    void Move()
    {
        bool isRunning=false;
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 move = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;   
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
        //transform.Translate(new Vector3(horizontalInput * targetSpeed * Time.deltaTime, 0, verticalInput * targetSpeed * Time.deltaTime));
        characterController.Move(move*currentSpeed*Time.deltaTime);

    }
    void RoleCamera()
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