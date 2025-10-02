using System;
using UnityEngine;

public class RoleController : MonoBehaviour
{
    public Animator animator;
    public CharacterController characterController;
    [Header("�ƶ�����")]
    public float walkSpeed;//,�����ں���Speed����ʱ��ֵ�����ǽ�ɫ�����ٶ�   2f
    public float runSpeed;//ͬ��   5f
    public float acceleration; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    public float deceleration; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    public float currentSpeed = 0f;
    float horizontalMouseSum=0;    //���ˮƽ�ƶ��ۼ�
    float verticalMouseSum = 0;//��괹ֱ�ƶ��ۼ�
    float minCameraAngle=-90f;//�������С�Ƕ�
    float maxCameraAngle=80f;//��������Ƕ�
    Vector3 move=Vector3.zero;
    [Header("��Ծ����")]
    public Transform groundCheck;//������Ŀ������λ��
    public float checkRadius;//���뾶 0.41f
    public bool isGrounded=false;
    public LayerMask groundMask;//��������Щ��
    public float gravity;//����
    public float jumpForce;//��Ծ�߶�
    float targetSpeed=0;
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
        //Debug.Log(move.x+"  "+ move.y + "  "+ move.z);
        Move();
        RoleCamera();
    }

    private void Jump()
    {
        move.y =  Mathf.Sqrt(2 * gravity * jumpForce);
        animator.SetTrigger("isJump");
    }

    //ʹ������
    private void ApplyGravity()
    {
        if (!isGrounded){
            move.y -= gravity * Time.deltaTime;
        }
    }

    //������
    private void GroundCheck()
    {
        isGrounded=Physics.CheckSphere(groundCheck.position,checkRadius,groundMask);
        if (isGrounded && move.y < 0)
        {
            move.y = -2;//С�����µ���ȷ����������
        }
    }


    //��ɫ�ƶ��ű�
    void Move()
    {
        bool isRunning=false;
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");
        move = (transform.right * horizontalInput +transform.up*move.y+ transform.forward * verticalInput);   
        if (verticalInput > 0.01)
        {
            isRunning = Input.GetKey(KeyCode.LeftShift);//������w��leftShiftʱ���ñ���״̬
        }
        
        targetSpeed = 0f;//anim������ں���Speed��ֵ
        if (move.x !=0 || move.z !=0) // �����·����ʱ�����ٶ�
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
        characterController.Move(new Vector3(move.x ,0, move.z)* currentSpeed * Time.deltaTime);//ˮƽ�ƶ�
        characterController.Move(new Vector3(0 , move.y, 0) * jumpForce * Time.deltaTime);//��Ծ

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