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
    [Header("����������ʾ���")]
    public Canvas canvas;
    public Image ObjectPanel;//��ʾ������Ϣ�����ObjectPanel
    public TextMeshProUGUI objectName;//��ʾ��������objectName
    public Image KeyIcon;//��ʾ�����Ƿ�ΪԿ��
    [Header("������")]
    public float maxDistance;
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
        currentObject = null;//��ʼ����ǰ����
        //���߼��
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //�����⵽����ı�ǩ��Insectable�ɽ�������,����ʾ������Ϣ
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;//���õ�ǰ����

                ClearHighlight();// ȡ��֮ǰ�ĸ���
                HighlightObject(currentObject);  // ����������

                ObjectPanel.gameObject.SetActive(true);
                objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//��ȡ��ǰ����InteractiveObject����е�Name��������ʾ
                if (currentObject.GetComponent<InteractiveObject>().isKey)
                {
                    KeyIcon.enabled = true;
                }
                else
                {
                    KeyIcon.enabled = false;
                }
            }
        }
    }
    void ClearHighlight()
    {

    }
    void HighlightObject(GameObject currentobj)
    {
        

    }
}
