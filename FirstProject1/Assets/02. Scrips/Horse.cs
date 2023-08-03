using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Horse : MonoBehaviour
{
    public float _totalDistance => Vector3.Distance(transform.position, _startPos);


    //[SerializeField] : �ش� �ʵ带 ����Ƽ�������� �ν�����â�� �����Ű������ Attribute.
    public bool doMove;
    [SerializeField] private float speed = 0.5f;
    [Range(0.0f, 1.0f)] [SerializeField] private float _stability;
    private float _speedModified;
    private float _speedmodifyingDistance = 1f;
    private float _speedModifyedDistanceMark;
    private Vector3 _startPos;



    private void Awake()
    {
        _startPos = transform.position;
        _speedModified = speed * Random.Range(_stability, 1.0f);
    }

    private void Start()
    {
        RaceManager.instance.Register(this);
    }
    private void FixedUpdate()
    {
        if (doMove)
        {
            if (_totalDistance - _speedModifyedDistanceMark > _speedmodifyingDistance)
            {
                _speedModified = speed * Random.Range(_stability, 1.0f);
                _speedModifyedDistanceMark = _totalDistance;
            }



            Move();
        }
    }


    // �Ÿ�  = �ӷ� * �ð�
    // �������Ӵ� �Ÿ� = �ӷ� * �������Ӵ�ð�
    private void Move()
    {
        transform.position += Vector3.forward * _speedModified * Time.fixedDeltaTime; 
    }
}
