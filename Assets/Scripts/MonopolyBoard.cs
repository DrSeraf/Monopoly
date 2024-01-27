using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  ласс MonopolyBoard, который представл€ет собой игровое поле в игре монополии
public class MonopolyBoard : MonoBehaviour
{
    // —писок узлов на игровом поле
    public  List<MonopolyNode> route = new List<MonopolyNode>();

    // ћетод, который вызываетс€ при изменении значений в инспекторе Unity
    private void OnValidate()
    {
        // ќчистка списка узлов
        route.Clear();

        // ƒобавление всех узлов в список
        foreach (Transform node in transform.GetComponentInChildren<Transform>())
        {
            route.Add(node.GetComponent<MonopolyNode>());
        }
    }

    // ћетод, который рисует линии между узлами на игровом поле
    private void OnDrawGizmos()
    {
        // ѕроверка, что в списке есть более одного узла
        if (route.Count > 1) 
        {
            // ÷икл по всем узлам в списке
            for (int i = 0; i < route.Count; i++) 
            {
                // ѕолучение позиции текущего узла
                Vector3 current = route[i].transform.position;
                // ѕолучение позиции следующего узла, если он существует, иначе - позици€ текущего узла
                Vector3 next = (i + 1 < route.Count) ? route[i + 1].transform.position : current;

                // ”становка цвета линии
                Gizmos.color = Color.green;
                // –исование линии от текущего узла к следующему
                Gizmos.DrawLine(current, next);
            }
        }
    }

}
