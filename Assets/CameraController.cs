using UnityEngine;

public class CameraController : MonoBehaviour
{
    // ���� ����� Inspector â���� ������ �� �ֵ��� public���� ����
    public Transform player;

    // ī�޶��� �ε巯�� �̵� �ӵ�
    public float smoothSpeed = 0.125f;

    // �÷��̾�κ��� ������ ���� �Ÿ� (ī�޶� ��ġ ������)
    public Vector3 offset;

    // LateUpdate�� ��� Update �Լ��� ȣ��� �Ŀ� ����˴ϴ�.
    // �÷��̾ ������ �Ŀ� ī�޶� ���󰡾� ���̳� ������ ���� ������ LateUpdate�� ����մϴ�.
    void LateUpdate()
    {
        // �÷��̾ �Ҵ���� �ʾҴٸ� �ƹ��͵� ���� ����
        if (player == null)
        {
            return;
        }

        // 1. ��ǥ ��ġ ���: �÷��̾��� ���� ��ġ + ������
        Vector3 desiredPosition = player.position + offset;

        // 2. �ε巯�� �̵� ��� (Lerp ���)
        // ���� ī�޶� ��ġ���� ��ǥ ��ġ���� smoothSpeed�� ���� �ε巴�� �̵��� ��ġ�� ���
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // 3. ���� ��ġ�� ī�޶� �̵�
        transform.position = smoothedPosition;
    }
}