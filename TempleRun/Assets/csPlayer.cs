using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csPlayer : MonoBehaviour
{
    GameObject manager;  //Bridge Manager 연결
    Animation anim;   //애니메이션 연결

    // Rigidbody rigid;

    public float speedForward=10.0f;  //전진 속도
    int speedSide=6;     //옆걸음 속도
    int jumpPower=300;   //점프

    bool canJump=true;  //점프 가능
    bool canTurn=false;  //회전 가능
    bool canLeft=true; //왼쪽 이동 가능
    bool canRight=true; //오른쪽 이동 가능
    bool isGround =true; //바닥에 있는지
    bool isDead=false;  //죽었는지

    float dirX=0;  //좌우 이동방향 ( -1 왼쪽, 1 오른쪽)
    float score=0;

    Vector3 touchStart;   //모바일에서의 터치 시작위치



    // void Awake()
    // {
    //     rigid=GetComponent<Rigidbody>();
    // }
    //Start
    void Start()
    {
        Screen.orientation=ScreenOrientation.Portrait;
        Screen.sleepTimeout=SleepTimeout.NeverSleep;

        manager=GameObject.Find("BridgeManager");
        anim=GetComponent<Animation>();
    }
    //게임 루프
    void Update()
    {
        if(isDead==true)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        CheckMove();  //이동 및 점프가능여부 체크 함수
        MoveHuman();  //플레이어 이동

        score+=Time.deltaTime*10;  //득점 처리
        speedForward+=Time.deltaTime*0.3f;
    }

    //이동 및 점프가능 여부 체크
    void CheckMove()
    {
        RaycastHit hit;

        //디버그용
        Debug.DrawRay(transform.position,Vector3.down*2f,Color.red);
        Debug.DrawRay(transform.position,Vector3.left*0.7f,Color.red);
        Debug.DrawRay(transform.position,Vector3.right*0.7f,Color.red);

        //레이캐스트 사용법  (기준점,방향,hitinfo,거리) > 가장 가까운 물체 탐색

        isGround=true;
        if(Physics.Raycast(transform.position,Vector3.down,out hit,0.1f))
        {
            if(hit.transform.tag=="BRIDGE")
            isGround=true;
        }

        canLeft=true;
        if(Physics.Raycast(transform.position,Vector3.left,out hit,0.7f))
        {
            if(hit.transform.tag=="GUARD")
            canLeft=false;
        }

        canRight=true;
        if(Physics.Raycast(transform.position,Vector3.right,out hit,0.7f))
        {
            if(hit.transform.tag=="GUARD")
            canRight=false;
        }
    }


    //플레이어 이동
    void MoveHuman()
    {
        dirX=0;

        if(Application.platform==RuntimePlatform.Android||
            Application.platform==RuntimePlatform.IPhonePlayer)
            {
                CheckMobile();
            }
            else
            {
                CheckKeyboard();
            }

            Vector3 moveDir=new Vector3(dirX*speedSide,0,speedForward);
            transform.Translate(moveDir*Time.deltaTime);
    }



    //모바일에서의 처리
    void CheckMobile() 
    {
        float x=Input.acceleration.x;
        if(canLeft&&isGround)
        {
            if(x<-0.2f)
            dirX=-0.6f;
            else if(x>0.2f)
            dirX=0.6f;
        }

        foreach(Touch tmp in Input.touches)
        {
            if(tmp.phase==TouchPhase.Began)
            {
                touchStart=tmp.position;
            }
            if(tmp.phase==TouchPhase.Moved)
            {
                Vector3 touchEnd=tmp.position;

                if(isGround&& canJump && touchEnd.y-touchStart.y>100)
                {
                    StartCoroutine("JumpHuman");
                }

                if(canTurn&&touchEnd.x-touchStart.x<-100)
                {
                    RotateHuman("LEFT");
                }

                if(canTurn&& touchEnd.x-touchStart.x>100)
                {
                    RotateHuman("RIGHT");
                }
            }
        }
    }


    //키보드 처리
    void CheckKeyboard() 
    {
        if(isGround)
        {
            dirX=Input.GetAxis("Horizontal");

            if(canJump&&Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine("JumpHuman");
            }
        }

        if(canTurn)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            RotateHuman("LEFT");
            if(Input.GetKeyDown(KeyCode.E))
            RotateHuman("RIGHT");
        }
    }
    //점프
    IEnumerator JumpHuman()
    {
        canJump=false;
        gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*jumpPower);
        //anim.Play("jump_pose");
        anim.Play("JUMP00");
        yield return new WaitForSeconds(1);
        //anim.Play("run");
        anim.Play("RUN00_F");
        canJump=true;
    }

    //플레이어의 회전
    void RotateHuman(string sDir)
    {
        canTurn=false;  //반복회전 금지
        Vector3 rot=transform.eulerAngles;   //현재의 회전각 구하기

        switch (sDir)
        {
            case "LEFT":
            rot.y-=90;
            break;

            case"RIGHT":
            rot.y+=90;
            break;
        }
        transform.eulerAngles=rot;
        //플레이어 방향으로 다리만들기 - 메세지보내기로 함수호출
        manager.SendMessage("MakeBridge",sDir,SendMessageOptions.DontRequireReceiver);
    }


    ///충돌 처리 - DeadZone
    void OnCollisionEnter(Collision col)
    {
        if(col.transform.tag=="DEAD")
        {
            isDead=true;
            //anim.Play("idle");
            anim.Play("WAIT01");
        }
    }

    //층돌시작- 기타
    void OnTriggerEnter(Collider col)
    {
        switch(col.transform.tag)
        {
            case "TURN":
            canTurn=true;
            break;

            case "COIN":
            score+=100;
            Destroy(col.gameObject);
            break;
        }
    }

    //충돌 끝

    void OnTriggerExit(Collider col)
    {
        if(col.tag=="TURN")
        {
            canTurn=false;
        }
    }


    //----------GUI 설정-------------
    void OnGUI()
    {
        string str= "<size=20><color=#000000>SCORE : ##</color></size>";

        GUI.Label(new Rect(50,50,1200,320),str.Replace("##",""+(int)score));

        if(!isDead)
        return;

        int w=Screen.width/2;
        int h=Screen.height/2;

        if(GUI.Button(new Rect(w-60,h-50,480,200),"Quit Game"))
        {
            Application.Quit();
        }
    }
}
