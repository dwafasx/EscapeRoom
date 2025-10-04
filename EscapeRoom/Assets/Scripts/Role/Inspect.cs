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
    public Image objectPanel;//��ʾ������Ϣ�����ObjectPanel
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
    Vector3 startScale;//�������ǰ����ʼ���Ŵ�С�����˳�����ʱ����Ʒ��ת��ԭλ
    Coroutine startInspectCoroutine = null;//��ʼ����Э�̺󷵻�Coroutine
    RoleController rolecontroller;//��ɫ������
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //��ȡInspectCanvas,����ֱ�Ӵӳ�������
    }
    void Start()
    {
        rolecontroller = GetComponent<RoleController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
        if (isInspect && Input.GetKeyDown(KeyCode.E))
        {
              stoptInspect();
        }
    }

    public void  HandleRaycast()
    {
        objectPanel.gameObject.SetActive(false);//�����������
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
                objectPanel.gameObject.SetActive(true);//��ʾ������Ϣ
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
        if (!isInspect && currentObject.GetComponent<InteractiveObject>().Viewable)//�������û�м���,����ƷΪ�ɼ������壬���ڰ���E����ʼ����
        {
            //����ɫ���ƽű����ã��������ں���SpeedΪ0����ֹ�ڼ���ʱ��ɫ���ڲ�����������
            //������ƽ���ƶ���ָ��λ�ü���
            if (Input.GetKeyDown(KeyCode.E))
            {
                startInspectCoroutine = StartCoroutine(startInspect(rolecontroller, 0.5f));//��������Э�̣������ɫ�������͹���ʱ��
            }
        } 
    }

    public void stoptInspect()
    {
        Debug.Log("ֹͣ����Э��");
        rolecontroller.enabled = true;
        if (startInspectCoroutine != null)
        {
            StopCoroutine(startInspectCoroutine);
        }
        isInspect = false;
    }

    IEnumerator startInspect(RoleController rolecontroller, float time)
    {
        if (isInspect) yield break;
        Debug.Log("����һ֡��ʼ����");
        yield return null; //����ȴ�һ֡��ִ�У���ֹ��stopInspect����������ͻ;
        isInspect = true;
        startPosition =currentObject.transform.position;//������ʼλ��
        startRotation=currentObject.transform.rotation;//������ʼ�Ƕ�
        startScale=currentObject.transform.localScale;//������ʼ����
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
