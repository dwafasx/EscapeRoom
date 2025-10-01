using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//��ɫ����
public class Inspect : MonoBehaviour
{
    public GameObject currentObject;
    [Header("����������ʾ���")]
    public Canvas canvas;//InspectCanvas
    [Header("������")]
    public float maxDistance;
    [Header("��Ʒ����")]
    public TextMeshProUGUI objectName;
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //��ȡInspectCanvas,����ֱ�Ӵӳ�������
    }
    void Start()
    {
        //��ȡcanvas�µ�ObjectName�е�TextMeshProUGUI���,������ʾ��������
        objectName = canvas.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
    }

    public void  HandleRaycast()
    {
        objectName.enabled = false;//������������
        currentObject = null;//��ʼ����ǰ����
        //���߼��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //�����⵽����ı�ǩ��Insectable�ɽ�������,����ʾ��������
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;
                if (objectName != null)
                {
                    objectName.enabled = true;
                    objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//��ȡ��ǰ����InteractiveObject����е�Name��������ʾ
                }
                else
                {
                    Debug.Log("objectName/TMPUI���δ��ȡ");
                }
            }
        }
    }
}
