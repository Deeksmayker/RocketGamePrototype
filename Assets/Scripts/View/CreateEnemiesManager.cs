using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesPrefabs;
    
    public List<EnemiesCreating> creatingEnemies;
    [Serializable]
    public struct EnemiesCreating
    {
        public int timeToCreate;
        public List<Enemies> chosenEnemy;
    }
    
    public enum Enemies
    {
        spider,
        enemy1,
        enemy2
    }


    public void CreateEnemy(int time, Enemies enemy)
    {
        
    }
}
