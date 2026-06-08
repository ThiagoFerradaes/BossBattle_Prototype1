using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class VFXPreFabProjectile : MonoBehaviour
{
    VFXAtributes _vfxAtributes;
    Coroutine _moveRoutine, _collisionRoutine;

    bool _hasCollided;

    //we will have two types of effects: VFX Graph and Particle System
    private VisualEffect myVFX;
    private ParticleSystem myParticle;

    private bool isVFX;
    private bool isParticle;

    private Collider myCollider;

    void Start()
    {
        isVFX = TryGetComponent<VisualEffect>(out myVFX);
        isParticle = TryGetComponent<ParticleSystem>(out myParticle);
        myCollider = GetComponent<Collider>();
    }

    public void Initialize(VFXAtributes atributes)
    {
        _vfxAtributes = atributes;
        gameObject.SetActive(true);
        _moveRoutine ??= StartCoroutine(ProjectileMoveRoutine());
    }

    public void TurnOff()
    {
        _hasCollided = false;
        _moveRoutine = null;
        PoolingManager.Instance.ReturnObjectToPool(this.gameObject, TypeOfSkillPrefab.VFX);
    }

    IEnumerator ColisionTimer()
    {
        _hasCollided = true;
        
        //disable collider, wait for "x" seconds, turn collider on for next obj in pool, then turn off obj
        myCollider.enabled = false;
        yield return new WaitForSeconds(_vfxAtributes.VFXPosCollisionDuration);
        myCollider.enabled = true;
        TurnOff();
    }

    IEnumerator ProjectileMoveRoutine()
    {
        float duration =
            _vfxAtributes.Distance / _vfxAtributes.VFXSpeed;
        float timer = 0;

        while (timer < duration && !_hasCollided)
        {
            transform.position += _vfxAtributes.VFXSpeed * Time.deltaTime * transform.forward;
            timer += Time.deltaTime;
            yield return null;
        }
        
        //--effects logic --
        if(isVFX) {
            myVFX.SendEvent("MyStopEvent");
        }
        if(isParticle)
        {
            //do something if vfx is particle system based
        }

        //disable collider, wait for "x" seconds, turn collider on for next obj in pool, then turn off obj
        myCollider.enabled = false;
        yield return new WaitForSeconds(_vfxAtributes.VFXPosCollisionDuration);
        myCollider.enabled = true;
        TurnOff();
    }


    //FOR THE FOLLOWING 2 FUNCTIONS: NEED TO ADD A CHECK ON WHAT TYPE OF LAYER THE THE OBJECT BEING COLLIDED WITH HAS!!
    private void OnTriggerEnter(Collider other)
    {
        if (!_vfxAtributes.UnitsToHit.ContainsLayer(other.gameObject.layer)) return;

        if (!_vfxAtributes.CrossEnemy) /*_collisionRoutine ??= */StartCoroutine(ColisionTimer());

        //--effects logic--
        if(isVFX) {
            //Debug.Log("Trigger Enter with: " + other.gameObject.name);
            myVFX.SendEvent("MyTriggerEnterEvent");
        }
        if(isParticle)
        {
            //do something if vfx is particle system based
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //--effects logic--
        if(isVFX) {
            //Debug.Log("Trigger Exit from: " + other.gameObject.name);
            myVFX.SendEvent("MyTriggerExitEvent");
        }
        if (isParticle)
        {
            
        }
        
    }

    /*
    // N�o funcionou
    private void OnParticleCollision(GameObject other) {
        Debug.Log("Collision");
        if (!_vfxAtributes.UnitsToHit.ContainsLayer(other.layer)) return;

        if (!_vfxAtributes.CrossEnemy) _collisionRoutine ??= StartCoroutine(ColisionTimer());
    }
    */
}
