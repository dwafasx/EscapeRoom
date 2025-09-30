using System;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    [Header("�ƶ�����")]
    public float walkSpeed;//,�����ں���Speed����ʱ��ֵ�����ǽ�ɫ�����ٶ�   2f
    public float runSpeed;//ͬ��   5f
    public float acceleration; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    public float deceleration; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    private float currentSpeed = 0f;
    float horizontalMouseSum=0;    //���ˮƽ�ƶ��ۼ�
    float verticalMouseSum = 0;//��괹ֱ�ƶ��ۼ�
    float minCameraAngle=-60f;//�������С�Ƕ�
    float maxCameraAngle=60f;//��������Ƕ�
    [Header("��Ծ����")]
    public Transform groundCheck;//������Ŀ������λ��
    public float checkRadius =1.3f;//���뾶
    public bool isGrounded=false;
    public LayerMask groundMask;
    public float gravity;
    public float jumpForce;
    private Vector3 velocity=Vector3.zero;
    void Start()
    {
        //������굽��Ļ���Ĳ�����
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
        GroundCheck();//������
        ApplyGravity();//Ӧ������
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
        Debug.Log(velocity.x+"  "+ velocity.y + "  "+velocity.z);
        characterController.Move(velocity*Time.deltaTime);//��ɫ��Ծ
        Move();
        RoleCamera();
    }

    private void Jump()
    {
        velocity.y =  Mathf.Sqrt(2 * gravity * jumpForce);
    }

    //ʹ������
    private void ApplyGravity()
    {
        if (!isGrounded){
            velocity.y -= gravity * Time.deltaTime;
        }
    }

    //������
    private void GroundCheck()
    {
        isGrounded=Physics.CheckSphere(groundCheck.position,checkRadius,groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;//С�����µ���ȷ����������
        }
    }


    //��ɫ�ƶ��ű�
    void Move()
    {
        bool isRunning=false;
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 move = (transform.right * horizontalInput + transform.forward * verticalInput).normalized;   
        if (verticalInput > 0.01)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);//������w��leftShiftʱ���ñ���״̬
        }
        
        float targetSpeed = 0f;//anim������ں���Speed��ֵ
        if (Mathf.Abs(verticalInput) > 0.01f || Mathf.Abs(horizontalInput) > 0.01f) // �����·����ʱ�����ٶ�
        {
            targetSpeed = isRunning ? runSpeed : walkSpeed;
        }

        // ƽ��������ǰ�ٶ�
        if (targetSpeed > currentSpeed)
        {
            currentSpeed = Mathf.Min(targetSpeed, currentSpeed + acceleration * Time.deltaTime);
        }
        else if (targetSpeed < currentSpeed)
        {
            currentSpeed = Mathf.Max(targetSpeed, currentSpeed - deceleration * Time.deltaTime);
        }

        animator.SetFloat("Speed", currentSpeed);//�����ں���Speed
        characterController.Move(move*currentSpeed*Time.deltaTime);

    }

    //���ý�ɫ���������ת
    void RoleCamera()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        horizontalMouseSum += horizontal;//���ˮƽ�ƶ��ۼ�
        verticalMouseSum += vertical; //��괹ֱ�ƶ��ۼ�
        transform.localRotation=Quaternion.Euler(Vector3.up* horizontalMouseSum);//��ɫˮƽ��ת
        //��������Ƕ�
        verticalMouseSum = Mathf.Clamp(verticalMouseSum, minCameraAngle, maxCameraAngle);
        //�����ת
        Camera.main.transform.localRotation=Quaternion.Euler(-verticalMouseSum,0,0);
    }
}