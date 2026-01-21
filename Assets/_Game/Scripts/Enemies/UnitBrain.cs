using UnityEngine;

[RequireComponent(typeof(UnitController))]
public abstract class UnitBrain : MonoBehaviour
{
    protected UnitController unit;

    protected virtual void Awake()
    {
        unit = GetComponent<UnitController>();
    }

    protected virtual void Update()
    {
        Think();
    }

    protected abstract void Think();
}