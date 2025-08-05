// TargetMover.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public Transform target;
    [Header("�Ÿ� ����")]
    [Tooltip("Ÿ�ٰ��� �ּ� �Ÿ�")]
    public float minDistance = 10f;
    [Tooltip("Ÿ�ٰ��� �ִ� �Ÿ�")]
    public float maxDistance = 30f;
    [Tooltip("�Ÿ��� ���ϴ� �ӵ�")]
    public float distanceChangeSpeed = 1f;

    [Header("ȸ�� ����")]
    [Tooltip("ȸ�� �ӵ�")]
    public float rotateSpeed = 30f; // �� ���������� �ʴ� ������ ����

    // private ������
    private Vector3 targetDirection;

    void Awake()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned in TargetMover script.");
            enabled = false; // ��ũ��Ʈ ��Ȱ��ȭ
            return;
        }

        // ���� ������ transform�� ����(forward)���� �ʱ�ȭ
        targetDirection = transform.forward;
    }

    void Update()
    {
        // 1. �ð��� ���� �Ÿ��� �ε巴�� ���ϵ��� ���
        // Mathf.Sin(time)�� -1 ~ 1 ���̸� �պ�
        // (Mathf.Sin(time) + 1) / 2 �� 0 ~ 1 ���̸� �պ�
        float sinValue = (Mathf.Sin(Time.time * distanceChangeSpeed) + 1f) / 2f;

        // Mathf.Lerp�� ����Ͽ� 0~1 ������ ���� minDistance�� maxDistance ������ ������ ��ȯ
        float currentDistance = Mathf.Lerp(minDistance, maxDistance, sinValue);

        // 2. ȸ�� ���
        // AngleAxis�� ����Ͽ� y��(Vector3.up) �������� ȸ��
        Quaternion rotation = Quaternion.AngleAxis(rotateSpeed * Time.deltaTime, Vector3.up);
        targetDirection = rotation * targetDirection;

        // 3. ���� Ÿ�� ��ġ ��� �� ����
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