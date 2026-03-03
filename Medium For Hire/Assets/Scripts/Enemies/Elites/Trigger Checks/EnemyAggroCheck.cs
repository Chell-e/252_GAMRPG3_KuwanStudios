//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EnemyAggroCheck : MonoBehaviour
//{
//    public GameObject Player { get; set; }
//    private Enemy _enemy;

//    private void Awake()
//    {
//        Player = PlayerController.Instance.gameObject;
//        _enemy = GetComponentInParent<Enemy>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.gameObject == Player)
//        {
//            _enemy.SetAggroStatus(true);
//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.gameObject == Player)
//        {
//            _enemy.SetAggroStatus(false);
//        }
//    }
//}
