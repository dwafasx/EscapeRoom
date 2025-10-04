using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;

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

    [SerializeField] private float scaleFactor; // ��������ϵ�� 1.5f
    Vector3 startPosition;//�������ǰ����ʼλ�ã����˳�����ʱ����Ʒ�Ż�ԭλ
    Quaternion startRotation;//�������ǰ����ʼ�Ƕȣ����˳�����ʱ����Ʒ��ת��ԭλ
    Vector3 startScale;//�������ǰ����ʼ���Ŵ�С�����˳�����ʱ����Ʒ��ת��ԭλ
    Coroutine startInspectCoroutine = null;//��ʼ����Э�̺󷵻�Coroutine,������ͣЭ��
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
        startPosition =currentObject.transform.position;//������ʼλ��,��ֹͣ���Ӻ�Ż�ԭλ
        startRotation=currentObject.transform.rotation;//������ʼ�Ƕ�
        startScale=currentObject.transform.localScale;//������ʼ����
        float elapsedTime = time;        //����ʱ��
        rolecontroller.enabled = false;//���ý�ɫ�ƶ�
        Vector3 targetScale = CalculateOptimalScale(currentObject);//��������
        //currentObject.transform.DOScale(targetScale, elapsedTime);//ƽ�����ŵ����ʵĴ�С,����ʹ��DOTween���ʱ���Ų�ͬ��(�Ͽ�)��������·�ѭ��������
        while (time>0)//ƽ��������ֵ
        {
            rolecontroller.animator.SetFloat("Speed", rolecontroller.currentSpeed*time/ elapsedTime);//ƽ��������ɫ����
            currentObject.transform.position = Vector3.Lerp(startPosition,InspectPosition.position, (elapsedTime- time) / elapsedTime);//ƽ���������嵽����λ��
            currentObject.transform.rotation = Quaternion.Lerp(startRotation,Camera.main.transform.rotation, (elapsedTime - time) / elapsedTime);//������ƽ����ת�������ǰ��      ���屳������ͷ
            //currentObject.transform.DOLocalRotate(Camera.main.transform.rotation.eulerAngles,elapsedTime,RotateMode.Fast );//������ƽ����ת�������ǰ��      ���屳������ͷ
            currentObject.transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime - time) / elapsedTime);//ƽ������

            time -= Time.deltaTime;
            yield return null;
        }

    }

    //�����������ű���
    private Vector3 CalculateOptimalScale(GameObject targetObject)
    {
        // ��ȡ���弰������İ�Χ��
        Bounds bounds = GetObjectBounds(targetObject);
        // ���������С
        float objectSize = bounds.size.magnitude;

        // �����������Ұ������ʵĴ�С
        float cameraDistance = (Camera.main.transform.position - InspectPosition.position).magnitude;// ���Ӿ���
        float cameraFOV = Camera.main.fieldOfView;//�������Ұ��С
        float optimalSize = Mathf.Tan(cameraFOV * 0.5f * Mathf.Deg2Rad) * cameraDistance;

        // �������ű���
        float scale = (optimalSize / objectSize) * scaleFactor;

        return targetObject.transform.localScale * scale;
    }

    //��ȡ���弰������İ�Χ�в��ϲ�
    private Bounds GetObjectBounds(GameObject targetObject)
    {
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(targetObject.transform.position, Vector3.one);//δ��ȡ��render����򷵻ش�СΪ1�İ�Χ��

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);//�ϲ���Χ��
        }
        return bounds;
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
