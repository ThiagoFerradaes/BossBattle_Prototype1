using UnityEngine;

public abstract class CrabMarineAnimalSO : ScriptableObject {

    public float duration;
    public virtual void OnTrigger(Collider other, CrabMarineAnimal parent) { }

    public virtual void OnEnd() { }

    
}
