using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpriteDriver_Abstract : MonoBehaviour
{
    public I_SpriteController m_spriteController;

    public void SelfDestruct()
    {
        Destroy(this);
    }
}
