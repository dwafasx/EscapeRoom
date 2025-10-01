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
        currentObject = null;//初始化当前物体
        //射线检测
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //如果检测到物体的标签是Insectable可交互物体,则显示物体信息
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;//设置当前物体

                ClearHighlight();// 取消之前的高亮
                HighlightObject(currentObject);  // 高亮新物体

                ObjectPanel.gameObject.SetActive(true);
                objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//获取当前物体InteractiveObject组件中的Name变量并显示
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
