using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LINQTest : MonoBehaviour
{
    void Start()
    {
        List<int> listOfNumber = new() { 1, 2, 3, 4 };

        listOfNumber.Where(n => n > 2).ToList();

        Debug.Log(string.Join(", ", listOfNumber));
    }

}
