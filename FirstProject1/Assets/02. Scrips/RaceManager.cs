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
    public List<Horse> arrived = new List<Horse>();


    [SerializeField] private Transform _standPoint1st;
    [SerializeField] private Transform _standPoint2nd;
    [SerializeField] private Transform _standPoint3rd;
    public void Register(Horse horse)
    {
        horses.Add(horse);
    }

    public void AddArrived(Horse horse)
    {
        arrived.Add(horse);

        if (arrived.Count == horses.Count)
            FinishRace();
    }

    private void Awake()
    {
        instance= this;
    }

    private void start()
    {
        StartRace();
    }


    public void StartRace()
    {
        foreach (Horse horse in horses)
        {
            horse.doMove = true;
        }
    }

    public void FinishRace()
    {
        arrived[0].transform.position = _standPoint1st.position;
        arrived[0].transform.position = _standPoint2nd.position;
        arrived[0].transform.position = _standPoint3rd.position;
    }
}
