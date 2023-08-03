using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SerializeField : �ش� �ʵ��� ����Ƽ�������� �ν�����â�� ������ �����ֱ����� Attribute
public class Horse : MonoBehaviour
{
    public bool doMove;
    [SerializeField] private float speed;
    [SerializeField] private float stability;

    private void FixedUpdate()
    {
        if (doMove)
            Move();
    }


    // �Ÿ�  = �ӷ� * �ð�
    // �������Ӵ� �Ÿ� = �ӷ� * �������Ӵ�ð�
    private void Move()
    {
        transform position += Vector3. forward * speed + Time fixedDeltaTime; 
    }
}
