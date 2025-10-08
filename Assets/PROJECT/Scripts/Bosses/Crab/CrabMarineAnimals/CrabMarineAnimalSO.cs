using System.Collections.Generic;
using UnityEngine;

public abstract class CrabMarineAnimalSO : ScriptableObject {

    public float Duration;
    public List<Tags> ListOfTags;
    public virtual void OnTrigger(Collider other, CrabMarineAnimal parent) { }

    public virtual void OnEnd() { } 
}
