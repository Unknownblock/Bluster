using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float explodeTime;
    public float timer;
    public float radius;
    public float force;
    
    public void Update()
    {
        timer += Time.deltaTime;

        if (timer >= explodeTime)
        {
            Explode();
        }
    }

    private void Explode()
    {
        Explosion hitExplosion = Instantiate(PrefabManager.Instance.explosion, transform.position, Quaternion.identity).GetComponent<Explosion>();
        hitExplosion.radius = radius;
        hitExplosion.force = force;
        Destroy(gameObject);
    }
}
