using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff 
{
    protected  Action<Pc> BuffContent;
    
    public void On(Pc pc)
    {
        Debug.Log("랜덤 버프 발동!");
        SetBuffContent();
        BuffContent?.Invoke(pc);
    }

    public abstract void SetBuffContent();

    private void Off() { 
        
    }
}
