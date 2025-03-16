using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle :HaveHp
{
    //Hp만 지정해주면 되고 
    //스프라이트 넣어서 다르게 보여주기만 하면됨
    // 필요시 사운드랑, 에니메이션 추가
    [SerializeField] private int _hpSetting;
    private void Start()
    {
        Hp = _hpSetting;
    }
}
