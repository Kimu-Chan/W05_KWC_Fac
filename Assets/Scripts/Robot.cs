using UnityEngine;

// 어빌리티 사용 가능 여부
[System.Serializable]
public struct Ability{
    public bool abilMove;
    public bool abilJump;
    public bool abilHold;
    public bool abilRot;
}

// 어빌리티 사용시 계산되는 속성
[System.Serializable]
public struct Status{
    public float velocity;
    public float legPower;
    public float armPower;
    public float armLength;
    public float rotPower;
}

// 로봇 모델링 오브젝트들
[System.Serializable]
public struct Body{
    public GameObject arms;
    public GameObject right_shoulder;
    public GameObject right_arm;
    public GameObject left_shoulder;
    public GameObject left_arm;
}
