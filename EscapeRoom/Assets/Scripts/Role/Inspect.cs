using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;
using DG.Tweening;

//角色交互
public class Inspect : MonoBehaviour
{
    public GameObject currentObject;//当前物体，即射线检测到的tag为Inspectable的物体
    [Header("物体显示面板")]
    public Canvas canvas;
    public Image objectPanel;//显示物体信息的面板ObjectPanel
    public TextMeshProUGUI objectName;//显示物体名称objectName
    public Image KeyIcon;//显示物体是否为钥匙
    [Header("检测距离")]
    public float maxDistance;
    [Header("检视位置")]
    public Transform InspectPosition;
    [Header("是否正在检视")]
    public bool isInspect=false;

    [SerializeField] private float scaleFactor; // 检视缩放系数 1.5f
    Vector3 startPosition;//物体检视前的起始位置，当退出检视时将物品放回原位
    Quaternion startRotation;//物体检视前的起始角度，当退出检视时将物品旋转回原位
    Vector3 startScale;//物体检视前的起始缩放大小，当退出检视时将物品旋转回原位
    Coroutine startInspectCoroutine = null;//开始检视协程后返回Coroutine,用来暂停协程
    RoleController rolecontroller;//角色控制器
    // Start is called before the first frame update
    private void Awake()
    {
        //canvas =GameObject.Find("InspectCanvas").GetComponent<Canvas>();  //获取InspectCanvas,这里直接从场景拖入
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
        objectPanel.gameObject.SetActive(false);//隐藏物体面板
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
                objectPanel.gameObject.SetActive(true);//显示物体信息
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
                InspectObject();
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


    //检视物品
    private void InspectObject()
    {
        if (!isInspect && currentObject.GetComponent<InteractiveObject>().Viewable)//如果现在没有检视,且物品为可检视物体，则在按下E键后开始检视
        {
            //将角色控制脚本禁用，并设置融合树Speed为0，防止在检视时角色仍在播放其他动画
            //将物体平滑移动到指定位置检视
            if (Input.GetKeyDown(KeyCode.E))
            {
                startInspectCoroutine = StartCoroutine(startInspect(rolecontroller, 0.5f));//开启检视协程，传入角色控制器和过渡时间
            }
        } 
    }

    public void stoptInspect()
    {
        Debug.Log("停止检视协程");
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
        Debug.Log("在下一帧开始检视");
        yield return null; //这里等待一帧再执行，防止与stopInspect方法按键冲突;
        isInspect = true;
        startPosition =currentObject.transform.position;//设置起始位置,在停止检视后放回原位
        startRotation=currentObject.transform.rotation;//设置起始角度
        startScale=currentObject.transform.localScale;//设置起始缩放
        float elapsedTime = time;        //过渡时间
        rolecontroller.enabled = false;//禁用角色移动
        Vector3 targetScale = CalculateOptimalScale(currentObject);//计算缩放
        //currentObject.transform.DOScale(targetScale, elapsedTime);//平滑缩放到合适的大小,这里使用DOTween插件时缩放不同步(较快)，因此在下方循环中缩放
        while (time>0)//平滑调整数值
        {
            rolecontroller.animator.SetFloat("Speed", rolecontroller.currentSpeed*time/ elapsedTime);//平滑调整角色动画
            currentObject.transform.position = Vector3.Lerp(startPosition,InspectPosition.position, (elapsedTime- time) / elapsedTime);//平滑调整物体到检视位置
            currentObject.transform.rotation = Quaternion.Lerp(startRotation,Camera.main.transform.rotation, (elapsedTime - time) / elapsedTime);//将物体平滑旋转至相机正前方      物体背对摄像头
            //currentObject.transform.DOLocalRotate(Camera.main.transform.rotation.eulerAngles,elapsedTime,RotateMode.Fast );//将物体平滑旋转至相机正前方      物体背对摄像头
            currentObject.transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime - time) / elapsedTime);//平滑缩放

            time -= Time.deltaTime;
            yield return null;
        }

    }

    //计算最优缩放比例
    private Vector3 CalculateOptimalScale(GameObject targetObject)
    {
        // 获取物体及子物体的包围盒
        Bounds bounds = GetObjectBounds(targetObject);
        // 计算物体大小
        float objectSize = bounds.size.magnitude;

        // 根据摄像机视野计算合适的大小
        float cameraDistance = (Camera.main.transform.position - InspectPosition.position).magnitude;// 检视距离
        float cameraFOV = Camera.main.fieldOfView;//摄像机视野大小
        float optimalSize = Mathf.Tan(cameraFOV * 0.5f * Mathf.Deg2Rad) * cameraDistance;

        // 计算缩放比例
        float scale = (optimalSize / objectSize) * scaleFactor;

        return targetObject.transform.localScale * scale;
    }

    //获取物体及子物体的包围盒并合并
    private Bounds GetObjectBounds(GameObject targetObject)
    {
        Renderer[] renderers = targetObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(targetObject.transform.position, Vector3.one);//未获取到render组件则返回大小为1的包围盒

        Bounds bounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);//合并包围盒
        }
        return bounds;
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
