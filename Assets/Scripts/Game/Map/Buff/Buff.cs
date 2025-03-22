using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff 
{
    protected  Action<Piece> BuffContent;
    
    public void On(Piece piece)
    {
        Debug.Log("랜덤 버프 발동!");
        SetBuffContent();
        BuffContent?.Invoke(piece);
    }

    public abstract void SetBuffContent();

    private void Off() { 
        
    }
}
