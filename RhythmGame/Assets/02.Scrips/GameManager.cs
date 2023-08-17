using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
  public enum State
    {
        idle,
        LoadSongData,
        WaitUntilSongDataLoaded,
        StartPlay,
        WaitUntilPlayFinished,
        DisplayScore,
        WaitForUser,
    }
    public State state;


    private void Awake()
    {
        if (Instance != null)  // �̹� GameManager �� �����ϸ�
        {
            Destroy(gameObject); // ���� ����ִ� �ı�
            return;
        }

        Instance = this;  // GameManager ������ �̱��� ���
        DontDestroyOnLoad(gameObject); // �� ��ȯ�ǵ� �ı��ȵǰ� ����
    }

    private void Update()
    {
        switch (state)
        {
            case State.idle:
                break;
            case State.LoadSongData:
                {
                    SongDataLoader.Load(SongSelectionUI.s_selected);
                    state = State.WaitUntilSongDataLoaded;
                }
                break;
            case State.WaitUntilSongDataLoaded:
                {
                    if (SongDataLoader.isLoaded)
                    {
                        SceneManager.LoadScene("MusicPlay");
                        state = State.StartPlay;
                    }
                }
                break;
            case State.StartPlay:
                {
                    if (MusicPlayManager.instance != null)
                    {
                        MusicPlayManager.instance.StartMusicPlay();
                        state = State.WaitUntilPlayFinished;
                    }
                }
                break;
            case State.WaitUntilPlayFinished:
                break;
            case State.DisplayScore:
                break;
            case State.WaitForUser:
                break;
            default:
                break;
        }
    }
}
