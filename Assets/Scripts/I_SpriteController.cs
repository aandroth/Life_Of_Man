using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface I_SpriteController
{
    public void PushForward();
    public void PushBackward();
    public void Action();
    public void Idle();
    public virtual Transform ReturnSpecialTransform() { return null; }

    public void DestroySelf();
    public void GetOlder();
}
