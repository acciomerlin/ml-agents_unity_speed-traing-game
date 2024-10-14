using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // 旋转速度
    public float rotationSpeedX = 100f;
    public float rotationSpeedY = 100f;
    public float rotationSpeedZ = 100f;

    // 悬停时更换的材质
    public Material hoverMaterial;
    // 原始材质
    private Material originalMaterial;
    // 渲染器
    private Renderer objectRenderer;

    // 摄像机
    private Camera mainCamera;

    // 游戏管理器
    public GameManager gameManager;

    void Start()
    {
        // 获取渲染器组件
        objectRenderer = GetComponent<Renderer>();
        // 保存原始材质
        originalMaterial = objectRenderer.material;
        // 获取主摄像机
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        // 每帧在三个轴上旋转
        transform.Rotate(new Vector3(rotationSpeedX, rotationSpeedY, rotationSpeedZ) * Time.deltaTime);

        // 检测鼠标光标是否悬停在球体上
        CheckMouseHover();
    }

    void CheckMouseHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                // 悬停时更换材质
                objectRenderer.material = hoverMaterial;
                // 检测鼠标点击
                if (Input.GetMouseButtonDown(0))
                {
                    gameManager.PlayerClick();
                }
            }
            else
            {
                // 恢复原始材质
                objectRenderer.material = originalMaterial;
            }
        }
        else
        {
            // 恢复原始材质
            objectRenderer.material = originalMaterial;
        }
    }
}