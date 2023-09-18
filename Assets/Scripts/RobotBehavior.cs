using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBehavior : MonoBehaviour
{
    public Ability ability;                                 // bool, 능력 개방 여부
    public Status status;                                   // 능력사용에 적용되는 수차
    public Body body;                                       // 로봇 바디 게임 오브젝트 모음
    public GameObject obj;

    GameObject objHold;                                     // 들고있는 오브젝트

    // Start is called before the first frame update
    void Start()
    {
        objHold = null;
        LoadRobot();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Hold();
        Rot();
        ObjHoldUpdate();
        ResetPos();
        // gomugomu();
    }

    // 로봇 모델링 오브젝트 불러오기
    void LoadRobot(){
        body.arms = transform.Find("Arms").gameObject;
        body.right_shoulder = body.arms.transform.Find("Right_shoulder").gameObject;
        body.right_arm = body.right_shoulder.transform.Find("Right_arm").gameObject;
        body.left_shoulder = body.arms.transform.Find("Left_shoulder").gameObject;
        body.left_arm = body.left_shoulder.transform.Find("Left_arm").gameObject;
    } 

    // 들고있는 오브젝트의 위치 업데이트
    void ObjHoldUpdate(){
        if(objHold != null && transform.hasChanged ){
            objHold.transform.position = new Vector3(transform.position.x,transform.position.y,transform.position.z);
            objHold.transform.Translate(new Vector3(0,0.5f,1.2f), transform);
            transform.hasChanged=false;
        }
    }

    // 로봇 회전
    void Rot(){
        if (ability.abilRot){
            if( Input.GetKey( KeyCode.A ) ){                                                                    // 반시계 방향 회전
                transform.Rotate(0, -status.rotPower, 0, Space.Self);
            }
            if( Input.GetKey( KeyCode.D ) ){                                                                    // 시계 방향 회전
                transform.Rotate(0, status.rotPower, 0, Space.Self);
            }
        }
    }

    // 오브젝트 들고 내려놓기
    void Hold(){
        List<GameObject> FoundObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("Object"));      // "Object"태그 붙은 물체랑만 상호작용
        GameObject target = FoundObjects[0];                                                                    
        float shortDis = Vector3.Distance(gameObject.transform.position, target.transform.position);
        foreach (GameObject found in FoundObjects)                                                              // 젤 가까운 오브젝트 찾기
        {
            float dist = Vector3.Distance(gameObject.transform.position, found.transform.position);
 
            if (dist < shortDis)
            {
                target = found;
                shortDis = dist;
            }
        }

        if(ability.abilHold && Input.GetKeyDown( KeyCode.Space )){
            if(objHold != null){                                                                                // 내려놓기
                objHold.GetComponent<Collider>().enabled = true;
                objHold = null;
            }
            else if(shortDis < 2 && status.armPower > target.GetComponent<ObjectBehavior>().status.weight){         // 들기
                objHold = target;
                target.GetComponent<Collider>().enabled = false;
            }
        }
    }

    void Move(){
        if (ability.abilMove){
            if( Input.GetKey( KeyCode.W ) ){                                                                    // 앞으로 이동
                transform.Translate(new Vector3(0,0,status.velocity));
            }
            if( Input.GetKey( KeyCode.S ) ){                                                                    // 뒤로 이동
                transform.Translate(new Vector3(0,0,-status.velocity));
            }
        }
    }

    void ResetPos(){
        if( Input.GetKey( KeyCode.R ) ){
            transform.position = new Vector3(0f, 0.8f, 0.75f);
            transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }
    }
}
