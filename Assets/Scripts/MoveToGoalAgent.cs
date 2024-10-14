using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MoveToGoalAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;

    public bool isGameActive = false;
    private float moveSpeed = 10f; // 默认速度

    public GameManager gameManager;

    public override void OnEpisodeBegin()
    {
        if (!isGameActive)
        {
            // 游戏结束时将代理重置到初始位置并保持静止
            transform.localPosition = new Vector3(0f, 0.994f, 0f);
            return;
        }

        // 生成代理的随机位置
        transform.localPosition = new Vector3(Random.Range(-13.71f, 1f), 0.994f, Random.Range(-9.03f, 10f));

        // 生成目标的随机位置，确保距离代理至少2米
        Vector3 targetPosition;
        float minDistance = 2f;
        do
        {
            targetPosition = new Vector3(Random.Range(-14.9f, +13.14f), 2f, Random.Range(-11.09f, 10.74f));
        } while (Vector3.Distance(transform.localPosition, targetPosition) < minDistance);

        targetTransform.localPosition = targetPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (!isGameActive)
            return;

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (!isGameActive)
            return;

        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        float turnSpeed = 300f;

        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;
        if (moveDirection != Vector3.zero)
        {
            // 计算目标旋转角度
            Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            // 逐渐旋转模型朝向目标角度
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, turnSpeed * Time.deltaTime);
        }

        transform.localPosition += moveDirection * Time.deltaTime * moveSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            SetReward(+1f);
            gameManager.AgentTouch();
            EndEpisode();
        }
        if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public void StopAgent()
    {
        isGameActive = false;
        EndEpisode();
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }
}