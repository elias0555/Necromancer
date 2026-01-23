using UnityEngine;

public static class ExtensionMethods
{
	public static Vector3 DirectionTo(this Vector3 self, Vector3 other) {
		return (other - self).normalized;
	}
}

