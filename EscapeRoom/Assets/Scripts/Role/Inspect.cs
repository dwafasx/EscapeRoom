using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

//角色交互
public class Inspect : MonoBehaviour
{
    public GameObject currentObject;//当前物体，即射线检测到的tag为Inspectable的物体
    [Header("物体名称显示面板")]
    public Canvas canvas;
    public Image ObjectPanel;//显示物体信息的面板ObjectPanel
    public TextMeshProUGUI objectName;//显示物体名称objectName
    public Image KeyIcon;//显示物体是否为钥匙
    [Header("检测距离")]
    public float maxDistance;
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //获取InspectCanvas,这里直接从场景拖入
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
        ObjectPanel.gameObject.SetActive(false);//隐藏物体面板
        //射线检测
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //如果检测到物体的标签是Insectable可交互物体,则显示物体信息
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;//设置当前物体
                HighlightObject(currentObject);  // 高亮显示物体
                ObjectPanel.gameObject.SetActive(true);//显示物体信息
                objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//获取当前物体InteractiveObject组件中的Name变量并显示
                //如果物体是钥匙则显示钥匙图标
                if (currentObject.GetComponent<InteractiveObject>().isKey)
                {
                    KeyIcon.enabled = true;
                }
                else
                {
                    KeyIcon.enabled = false;
                }
            }
            else
            {
                ClearHighlight();// 射线检测到物体但tag不等于Inspectable时取消高亮
            }
        }
        else
        {
            ClearHighlight();// 射线未检测到物体时取消高亮
        }
    }

    //取消高亮
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

    //使用AssetStore 中的QuickOutline实现物体轮廓高亮显示
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
