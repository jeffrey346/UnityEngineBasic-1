using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RaceManager : MonoBehaviour
{
    // �̱��� ����
    // �ش� Ÿ���� ��ü�� �� �ϳ��� ������ �� Ŭ������ ���� ������ �����ϰ� ��������� ����.
    public static RaceManager instance;

    public Horse lead => horses.OrderByDescending(x => x._totalDistance).FirstOrDefault() ;

    public List<Horse> horses = new List<Horse>();

    public void Register(Horse horse)
    {
        horses.Add(horse);
    }

    private void Awake()
    {
        instance= this;
    }
}
