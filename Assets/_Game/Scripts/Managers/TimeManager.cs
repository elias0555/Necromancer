using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
	public static Action OnUpdate;
	
	private static float _TimeScale = 1f;
	public static float timeScale => _TimeScale;
	public static float deltaTime => Time.deltaTime * timeScale;
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnUpdate?.Invoke();
    }
}
