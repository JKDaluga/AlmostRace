using UnityEngine;

public interface IPooledObject
{
    void OnObjectActivate();
    void OnObjectDeactivate();
}
