using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    public bool IsClick();
    public bool IsHold();
    public void ButtonDownCheck();
}
