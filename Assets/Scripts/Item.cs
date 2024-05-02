using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Size { small, large, tall };

    [SerializeField] public Size size;
}
