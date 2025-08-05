using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

/// <summary>
/// CCD (Cyclic Coordinate Descent) 알고리즘을 사용한 간단한 체인 IK 솔버입니다.
/// </summary>
public class IKChain : MonoBehaviour
{
    [Header("IK 설정")]
    [Tooltip("IK 체인을 구성하는 관절(뼈)들입니다. 루트부터 순서대로 할당해야 합니다.")]
    public Transform[] chain;

    [Tooltip("IK 체인이 도달해야 할 목표 지점입니다.")]
    public Transform target;

    [Header("솔버 파라미터")]
    [Tooltip("IK 계산을 몇 번 반복할지 결정합니다. 높을수록 정확하지만 무거워집니다.")]
    [Range(1, 20)]
    public int iterations = 10;

    [Tooltip("목표 지점과 체인의 끝이 이 거리보다 가까워지면 계산을 멈춥니다.")]
    public float epsilon = 0.001f;

    private void Start()
    {
        // 체인이 수동으로 설정되지 않은 경우, 자동으로 계층 구조를 탐색하여 설정합니다.
        if (chain == null || chain.Length == 0)
        {
            BuildChain();
        }

        if (target == null)
        {
            Debug.LogWarning("목표 지점이 설정되지 않았습니다. 목표를 할당해주세요.");
        }
    }

    /// <summary>
    /// 현재 트랜스폼부터 시작하여 자식의 자식으로 이어지는 체인을 자동으로 구성합니다.
    /// </summary>
    private void BuildChain()
    {
        List<Transform> chainList = new List<Transform>();
        Transform current = this.transform;

        while (current != null)
        {
            chainList.Add(current);
            // 현재 관절에 자식이 있으면, 그 첫 번째 자식을 다음 관절로 지정합니다.
            if (current.childCount > 0)
            {
                current = current.GetChild(0);
            }
            else
            {
                // 자식이 더 이상 없으면 체인의 끝이므로 루프를 종료합니다.
                break;
            }
        }

        // 완성된 리스트를 배열로 변환하여 chain 변수에 할당합니다.
        chain = chainList.ToArray();

        Debug.Log($"IK 체인이 자동으로 설정되었습니다. 총 {chain.Length}개의 관절이 발견되었습니다.");
    }

    void LateUpdate()
    {
        if (target == null || chain.Length == 0)
            return;

        SolveIK();
    }

    private void SolveIK()
    {
        // 체인의 끝(End Effector)을 가져옵니다.
        Transform endEffector = chain[chain.Length - 1];

        // 지정된 횟수만큼 반복 계산합니다.
        for (int i = 0; i < iterations; i++)
        {
            // 체인의 끝에서부터 루트 방향으로 역순으로 관절을 순회합니다.
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