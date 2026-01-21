using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Corpse : MonoBehaviour
{
    public UnitProfileSO originalStats; // Les données à passer au minion
    private bool isResurrected = false;

    public void Setup(UnitProfileSO stats, Vector3 position)
    {
        originalStats = stats;
        transform.position = position;

        var sr = GetComponent<SpriteRenderer>();
        if (stats.corpseSprite != null) sr.sprite = stats.corpseSprite;
        else sr.sprite = stats.visualSprite; 

        sr.color = Color.gray; 
        transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public UnitProfileSO Consume()
    {
        if (isResurrected) return null;
        isResurrected = true;
        Destroy(gameObject); 
        return originalStats;
    }
}