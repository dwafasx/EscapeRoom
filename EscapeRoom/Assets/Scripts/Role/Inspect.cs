using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//��ɫ����
public class Inspect : MonoBehaviour
{
    public GameObject currentObject;//��ǰ���壬�����߼�⵽��tagΪInspectable������
    [Header("������ʾ���")]
    public Canvas canvas;
    public Image ObjectPanel;//��ʾ������Ϣ�����ObjectPanel
    public TextMeshProUGUI objectName;//��ʾ��������objectName
    public Image KeyIcon;//��ʾ�����Ƿ�ΪԿ��
    [Header("������")]
    public float maxDistance;
    [Header("����λ��")]
    public Transform InspectPosition;
    [Header("�Ƿ����ڼ���")]
    public bool isInspect=false;
    Vector3 startPosition;//�������ǰ����ʼλ�ã����˳�����ʱ����Ʒ�Ż�ԭλ
    Quaternion startRotation;//�������ǰ����ʼ�Ƕȣ����˳�����ʱ����Ʒ��ת��ԭλ
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //��ȡInspectCanvas,����ֱ�Ӵӳ�������
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
    }



    public void  HandleRaycast()
    {
        ObjectPanel.gameObject.SetActive(false);//�����������
        //���߼��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //�����⵽����ı�ǩ��Insectable�ɽ�������,����ʾ������Ϣ
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;//���õ�ǰ����
                HighlightObject(currentObject);  // ������ʾ����
                ObjectPanel.gameObject.SetActive(true);//��ʾ������Ϣ
                objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//��ȡ��ǰ����InteractiveObject����е�Name��������ʾ
                //���������Կ������ʾԿ��ͼ��
                if (currentObject.GetComponent<InteractiveObject>().isKey)
                {
                    KeyIcon.enabled = true;
                }
                else
                {
                    KeyIcon.enabled = false;
                }
                InspectObject();
            }
            else
            {
                ClearHighlight();// ���߼�⵽���嵫tag������Inspectableʱȡ������
            }
        }
        else
        {
            ClearHighlight();// ����δ��⵽����ʱȡ������
        }
    }


    //������Ʒ
    private void InspectObject()
    {
        RoleController rolecontroller = GetComponent<RoleController>();
        if (!isInspect)//�������û�м��ӣ����ڰ���E����ʼ����
        {
            //����ɫ���ƽű����ã��������ں���SpeedΪ0����ֹ�ڼ���ʱ��ɫ���ڲ�����������
            //������ƽ���ƶ���ָ��λ�ü���
            if (Input.GetKeyDown(KeyCode.E))
            {
                isInspect = true;
                StartCoroutine(startInspect(rolecontroller, 0.5f));//����Э�̣������ɫ�������͹���ʱ��
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isInspect = false;
                rolecontroller.enabled=true;
                //StopCoroutine(startInspect(rolecontroller, 1));
                StopAllCoroutines();
            }
        }
        
    }

    IEnumerator startInspect(RoleController rolecontroller, float time)
    {
        startPosition=currentObject.transform.position;//������ʼλ��
        startRotation=currentObject.transform.rotation;//������ʼλ��
        float elapsedTime = time;        //����ʱ��
        rolecontroller.enabled = false;//���ý�ɫ�ƶ�
        while (time>0)//ƽ��������ֵ
        {
            rolecontroller.animator.SetFloat("Speed", rolecontroller.currentSpeed*time/ elapsedTime);//ƽ��������ɫ����
            currentObject.transform.position = Vector3.Lerp(startPosition,InspectPosition.position, (elapsedTime- time) / elapsedTime);//ƽ���������嵽����λ��
            currentObject.transform.rotation = Quaternion.Lerp(startRotation,Camera.main.transform.rotation, (elapsedTime - time) / elapsedTime);//������ƽ����ת�������ǰ��      ���屳������ͷ

            time -= Time.deltaTime;
            yield return null;
        }

    }

    //ȡ������
    void ClearHighlight()
    {
        if(currentObject != null)
        {
            Outline outline= currentObject.GetComponent<Outline>();
            if (outline)
            {
                outline.enabled = false;
            }
        }
        
    }

    //ʹ��AssetStore �е�QuickOutlineʵ����������������ʾ
    void HighlightObject(GameObject currentobj)
    {
        Outline outline= currentobj.GetComponent<Outline>();
        if (outline == null)
        {
            outline=currentObject.AddComponent<Outline>();
        }
        outline.enabled = true;
        outline.OutlineWidth = 10;
    }
}
