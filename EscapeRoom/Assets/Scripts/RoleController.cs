using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private Animator animator;
    public float walkSpeed = 2.0f;//,�����ں���Speed����ʱ��ֵ�����ǽ�ɫ�����ٶ�
    public float runSpeed = 5.0f;//ͬ��

    public float acceleration = 3f; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    public float deceleration = 3f; // �ں���Speed�����ı�ʱ�ļ��ٶ�
    private float currentSpeed = 0f;

    float herizontalMouseSum=0;    //���ˮƽ�ƶ��ۼ�
    float verticalMouseSum = 0;//��괹ֱ�ƶ��ۼ�
    float minCameraAngle=-60f;
    float maxCameraAngle=60f;
    

    void Start()
    {
        //������굽��Ļ���Ĳ�����
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
        
        float targetSpeed = 0f;//anim������ں���SpeedĿ����ֵ
        if (Mathf.Abs(verticalInput) > 0.01f || Mathf.Abs(horizontalInput) > 0.01f) // ������ֵ
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
        transform.Translate(new Vector3(horizontalInput * targetSpeed * Time.deltaTime, 0, verticalInput * targetSpeed * Time.deltaTime));

    }
    void roleCamera()
    {
        float horizontal = Input.GetAxis("Mouse X");
        float vertical = Input.GetAxis("Mouse Y");
        herizontalMouseSum += horizontal;//���ˮƽ�ƶ��ۼ�
        verticalMouseSum += vertical; //��괹ֱ�ƶ��ۼ�
        transform.localRotation=Quaternion.Euler(Vector3.up*herizontalMouseSum);//��ɫˮƽ��ת
        //��������Ƕ�
        verticalMouseSum = Mathf.Clamp(verticalMouseSum, minCameraAngle, maxCameraAngle);
        //�����ת
        Camera.main.transform.localRotation=Quaternion.Euler(-verticalMouseSum,0,0);
    }
}