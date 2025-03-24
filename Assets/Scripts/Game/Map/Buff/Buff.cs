using System;
using UnityEngine;

public abstract class Buff 
{
    protected  Action<Piece> BuffContent;
    
    public void On(Piece pc)
    {
        Debug.Log("랜덤 버프 발동!");
        SetBuffContent();
        BuffContent?.Invoke(pc);
    }

    public abstract void SetBuffContent();

    private void Off() { 
        
    }
}
