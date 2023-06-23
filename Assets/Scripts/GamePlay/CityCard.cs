using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New City Card", menuName = "Dream Builder/City Card")]
public class CityCard : ScriptableObject
{
    [SerializeField] private Sprite graphic;
    [TextArea(5,8)]
    [SerializeField] private string description;
    [SerializeField] private GameObject spawnedCityComponent;

}
