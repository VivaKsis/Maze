using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallUnit : MonoBehaviour, IPoolable
{
    #region ObjectPool

    public GameObject GameObject { get; set; }

    public ObjectPool.Pool Pool { get; set; }

    public void OnAquire()
    {
        GameObject = this.gameObject;
        gameObject.SetActive(true);
    }

    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    #endregion
}
