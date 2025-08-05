// TargetMover.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public Transform target;
    [Header("거리 설정")]
    [Tooltip("타겟과의 최소 거리")]
    public float minDistance = 10f;
    [Tooltip("타겟과의 최대 거리")]
    public float maxDistance = 30f;
    [Tooltip("거리가 변하는 속도")]
    public float distanceChangeSpeed = 1f;

    [Header("회전 설정")]
    [Tooltip("회전 속도")]
    public float rotateSpeed = 30f; // 더 직관적으로 초당 각도로 조절

    // private 변수들
    private Vector3 targetDirection;

    void Awake()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned in TargetMover script.");
            enabled = false; // 스크립트 비활성화
            return;
        }

        // 시작 방향을 transform의 정면(forward)으로 초기화
        targetDirection = transform.forward;
    }

    void Update()
    {
        // 1. 시간에 따라 거리가 부드럽게 변하도록 계산
        // Mathf.Sin(time)은 -1 ~ 1 사이를 왕복
        // (Mathf.Sin(time) + 1) / 2 는 0 ~ 1 사이를 왕복
        float sinValue = (Mathf.Sin(Time.time * distanceChangeSpeed) + 1f) / 2f;

        // Mathf.Lerp를 사용하여 0~1 사이의 값을 minDistance와 maxDistance 사이의 값으로 변환
        float currentDistance = Mathf.Lerp(minDistance, maxDistance, sinValue);

        // 2. 회전 계산
        // AngleAxis를 사용하여 y축(Vector3.up) 기준으로 회전
        Quaternion rotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.up);
        targetDirection = rotation * targetDirection;

        // 3. 최종 타겟 위치 계산 및 적용
        Vector3 targetPosition = transform.position + targetDirection * currentDistance;
        target.position = targetPosition;
    }

    private void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, target.position);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, minDistance);
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}