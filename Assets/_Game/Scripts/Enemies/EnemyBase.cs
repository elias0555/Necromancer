using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour
{
	protected IEnemyTargetable target;
	protected SpriteRenderer spriteRenderer;
	protected Transform thisTransform;
	
	[SerializeField]
	protected float health;
	[SerializeField]
	protected float speed;
	
	protected Action DoAction;

	protected virtual void Awake()
	{
		thisTransform = transform;
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected virtual void Start()
	{
		DoAction = Move;
	}

	protected void Update()
	{
		DoAction?.Invoke();
	}

	protected abstract void Move();
	
	protected abstract void Attack();
	
	protected abstract void Death();
}
