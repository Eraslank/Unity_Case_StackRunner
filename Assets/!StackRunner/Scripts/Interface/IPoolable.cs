using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPoolable
{ 
    bool IsAvailable { get; set; }
    void Pool(UnityAction onPool = null);
    void DePool(UnityAction onDePool = null);
}
