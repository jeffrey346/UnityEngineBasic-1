using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class RaceManager : MonoBehaviour
{
    // 싱글톤 패턴
    // 해당 타입의 객체가 단 하나만 존재할 때 클래스를 통해 접근을 용이하게 만들기위한 형태.
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
