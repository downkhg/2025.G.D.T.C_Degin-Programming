using UnityEngine;
using System.Collections.Generic; // List�� ����ϱ� ���� �߰�

/// <summary>
/// CCD (Cyclic Coordinate Descent) �˰����� ����� ������ ü�� IK �ֹ��Դϴ�.
/// </summary>
public class IKChain : MonoBehaviour
{
    [Header("IK ����")]
    [Tooltip("IK ü���� �����ϴ� ����(��)���Դϴ�. ��Ʈ���� ������� �Ҵ��ؾ� �մϴ�.")]
    public Transform[] chain;

    [Tooltip("IK ü���� �����ؾ� �� ��ǥ �����Դϴ�.")]
    public Transform target;

    [Header("�ֹ� �Ķ����")]
    [Tooltip("IK ����� �� �� �ݺ����� �����մϴ�. �������� ��Ȯ������ ���ſ����ϴ�.")]
    [Range(1, 20)]
    public int iterations = 10;

    [Tooltip("��ǥ ������ ü���� ���� �� �Ÿ����� ��������� ����� ����ϴ�.")]
    public float epsilon = 0.001f;

    private void Start()
    {
        // ü���� �������� �������� ���� ���, �ڵ����� ���� ������ Ž���Ͽ� �����մϴ�.
        if (chain == null || chain.Length == 0)
        {
            BuildChain();
        }

        if (target == null)
        {
            Debug.LogWarning("��ǥ ������ �������� �ʾҽ��ϴ�. ��ǥ�� �Ҵ����ּ���.");
        }
    }

    /// <summary>
    /// ���� Ʈ���������� �����Ͽ� �ڽ��� �ڽ����� �̾����� ü���� �ڵ����� �����մϴ�.
    /// </summary>
    private void BuildChain()
    {
        List<Transform> chainList = new List<Transform>();
        Transform current = this.transform;

        while (current != null)
        {
            chainList.Add(current);
            // ���� ������ �ڽ��� ������, �� ù ��° �ڽ��� ���� ������ �����մϴ�.
            if (current.childCount > 0)
            {
                current = current.GetChild(0);
            }
            else
            {
                // �ڽ��� �� �̻� ������ ü���� ���̹Ƿ� ������ �����մϴ�.
                break;
            }
        }

        // �ϼ��� ����Ʈ�� �迭�� ��ȯ�Ͽ� chain ������ �Ҵ��մϴ�.
        chain = chainList.ToArray();

        Debug.Log($"IK ü���� �ڵ����� �����Ǿ����ϴ�. �� {chain.Length}���� ������ �߰ߵǾ����ϴ�.");
    }

    void LateUpdate()
    {
        if (target == null || chain.Length == 0)
            return;

        SolveIK();
    }

    private void SolveIK()
    {
        // ü���� ��(End Effector)�� �����ɴϴ�.
        Transform endEffector = chain[chain.Length - 1];

        // ������ Ƚ����ŭ �ݺ� ����մϴ�.
        for (int i = 0; i < iterations; i++)
        {
            // ü���� ���������� ��Ʈ �������� �������� ������ ��ȸ�մϴ�.
            for (int j = chain.Length - 2; j >= 0; j--)
            {
                Transform currentBone = chain[j];
                Vector3 toEndEffector = endEffector.position - currentBone.position;
                Vector3 toTarget = target.position - currentBone.position;
                Quaternion rotation = Quaternion.FromToRotation(toEndEffector, toTarget);
                currentBone.rotation = rotation * currentBone.rotation;
            }

            if (Vector3.Distance(endEffector.position, target.position) < epsilon)
                break;
        }
    }
}