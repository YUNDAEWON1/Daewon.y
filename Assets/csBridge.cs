using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csBridge : MonoBehaviour
{
    public GameObject[] bridges;  //다리 프리팹
    public GameObject[] coins; //동전 프리팹
    public GameObject bridgeTurn; //교차로

    //다리 전체를 담는 부모 오브젝트.
    GameObject newBridge;    //새로만들다리 -parent

    //각각의 다리(또는 동전) newBridge의 하위 오브젝트
    GameObject childBridge;   //각각의 다리(동전) = child

    //player가 교차로에서 회전하면 그 방향으로 다리를 다시 만들고 예전의 다리는 삭제한다.
    GameObject oldBridge;  //삭제할 예전의 다리

    //다리의종류(다리 1~5)를 설정할 변수이다.
    GameObject bridge;   //다리 종류 - 프리팹 만들기용
    GameObject coin;    //동전셋 종류 - 프리팹 만들기용

    bool canCoin=false;   //동전 만들기

    int dir=0;   //캐릭터의 회전 방향 (0~3)
    Quaternion quatAng; //캐릭터의 회전각


    //---------------------게임 초기화 부분 -------------------------

    void Start()
    {
        newBridge=GameObject.Find("StartBridge");
        oldBridge=GameObject.Find("oldBridge");
        childBridge=newBridge;  //자식의 위치를 부모와 일치시킴

        MakeBridge("FORWARD");  //전방으로 다리만들기
    }

    //다리를 만드는 함수
    void MakeBridge(string sDir)
    {
        DeleteOldBridge();    //플레이어가 지나간다리
        CalcRotation(sDir);   //캐릭터의 회전각 계산
        MakeNewBridge();      //회전 방향으로 다리 만들기
    }

    //플레이어가 지니간 다리는 삭제하고 새로운 다리의 시작점을 만든다.
    void DeleteOldBridge()
    {
        Destroy(oldBridge);  //예전 다리 삭제
        oldBridge=newBridge;  //현재 다리 저장

        //다리 시작점 만들기
        newBridge=new GameObject("StartBridge");   //스테이지에 있는 StartBridge를 새로 만들어서 newBridge에 저장한다.
    }

    //캐릭터의 회전각 계산 ( 플레이어의 회전 방향과 각도)
    void CalcRotation(string sDir)
    {
        switch(sDir) {
            case "LEFT":
            dir--;  //왼쪽이동
            break;

            case "RIGHT":
            dir++;   //오른쪽 이동
            break;
        }

        //회전 방향을 0~3으로 제한
        if(dir<0) dir=3;
        if(dir>3) dir=0;

        //회전각을 쿼터니언으로 변환
        quatAng.eulerAngles=new Vector3(0,dir*90,0);
    }

    //새로운 다리 만들기

    //이 게임의 가장 핵심적인 부분 = 다리는 한번에 10개씩 생성, 플레이어의 회전방향에 따라 다리의 위치가 달라져야함.
    //회전 방향에 따른 좌표계산이 핵심. 다리를 만든 후 newBridge의 하위 오브젝트로 설정할 것이므로 다리는 localPosition을 기준으로 만든다.

    void MakeNewBridge()
    {
        for(int i=0;i<10;i++)
        {
            bridge=bridges[0];  //기본다리 (다리의 시작점은 기본다리1)
            coin=coins[Random.Range(0,3)]; //동전셋팅 (동전은 랜덤하게 셋팅한다)
            canCoin=false;

            SelectBridge(i);  //다리 종류 설정 (랜덤한 다리가 나오도록 한다)

            Vector3 pos=Vector3.zero; // 다리의 position

            Vector3 localPos=childBridge.transform.localPosition;  // 다리의 로컬포지션 (맨 마지막으로 만들어진 다리의 로컬 포지션으로 이 위치를 기준으로 새로운 다리 생성)

            switch(dir) //플레이어의 회전 방향에 따라 각각의 위치에 다리를 만든다.
            {
                case 0:
                pos=new Vector3(localPos.x,0,localPos.z+10);
                break;
                
                case 1:
                pos=new Vector3 (localPos.x+10,0,localPos.z);
                break;

                case 2:
                pos=new Vector3 (localPos.x,0,localPos.z-10);
                break;

                case 3:
                pos=new Vector3 (localPos.x-10,0,localPos.z);
                break;

            }  

            //새로운 다리를 만들고 부모설정
            childBridge=Instantiate(bridge,pos,quatAng) as GameObject;
            childBridge.transform.parent=newBridge.transform;  //새로 만든 다리의 부모를 설정한다.

            if(canCoin)
            {
                childBridge=Instantiate(coin,pos,quatAng) as GameObject;
                childBridge.transform.parent=newBridge.transform;
            }
        }
    }


    //다리 종류 설정하기
    void SelectBridge(int n)
    {
        switch(n)
        {
            case 9:    //마지막 다리는 교차로
            bridge=bridgeTurn;
            break;

            case 1:          //홀수 번째 다리는 장애물이 있는 다리
            case 3:
            case 5:
            case 7:
            bridge=bridges[Random.Range(0,bridges.Length)];
            break;

            default:   //짝수 번째 다리에는 동전을 만든다.
            if(Random.Range(0,100)>50)
            {
                canCoin=true;
            }
            break;
        }
    }
}
