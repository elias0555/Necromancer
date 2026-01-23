using UnityEngine;

public class EnemySkeletonBasic : EnemyBase
{
	

	protected override void Start()
	{
		base.Start();
		target = Player.Instance;		
	}

	protected override void Attack()
	{
		throw new System.NotImplementedException();
	}

	protected override void Death()
	{
		throw new System.NotImplementedException();
	}

	protected override void Move()
	{
		thisTransform.position += thisTransform.position.DirectionTo(target.Position()) * speed * TimeManager.deltaTime;
	}
	
	void OnTriggerEnter2D(Collider2D collision)
	{
		
	}
}
