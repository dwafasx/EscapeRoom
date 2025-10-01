using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

//角色交互
public class Inspect : MonoBehaviour
{
    public GameObject currentObject;
    [Header("物体名称显示面板")]
    public Canvas canvas;//InspectCanvas
    [Header("检测距离")]
    public float maxDistance;
    [Header("物品名称")]
    public TextMeshProUGUI objectName;
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //获取InspectCanvas,这里直接从场景拖入
    }
    void Start()
    {
        //获取canvas下的ObjectName中的TextMeshProUGUI组件,用来显示物体名称
        objectName = canvas.transform.Find("ObjectName").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRaycast();
    }

    public void  HandleRaycast()
    {
        objectName.enabled = false;//隐藏物体名称
        currentObject = null;//初始化当前物体
        //射线检测
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            //如果检测到物体的标签是Insectable可交互物体,则显示物体名称
            if (hit.collider.CompareTag("Inspectable"))
            {
                currentObject = hit.collider.gameObject;
                if (objectName != null)
                {
                    objectName.enabled = true;
                    objectName.text = currentObject.GetComponent<InteractiveObject>().Name;//获取当前物体InteractiveObject组件中的Name变量并显示
                }
                else
                {
                    Debug.Log("objectName/TMPUI组件未获取");
                }
            }
        }
    }
}
