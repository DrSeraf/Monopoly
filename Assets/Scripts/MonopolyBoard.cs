using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  ласс MonopolyBoard, который представл€ет собой игровое поле в игре монополии
public class MonopolyBoard : MonoBehaviour
{
    // —писок узлов на игровом поле
    public  List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class NodeSet
    {
        public Color setColor = Color.white;
        public List<MonopolyNode> nodesInSetList = new List<MonopolyNode>();   
    }

    [SerializeField] List<NodeSet> nodeSetList = new List<NodeSet>(); 

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

        //Update all node colors
        for (int i = 0; i < nodeSetList.Count; i++)
        {
            for (int j = 0; j < nodeSetList[i].nodesInSetList.Count; j++)
            {
                nodeSetList[i].nodesInSetList[j].UpdateColofield(nodeSetList[i].setColor);
            }
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

    public void MovePlayerToken(int steps, Player player)
    {
        StartCoroutine(MovePlayerInSteps(steps, player));
    }

    IEnumerator MovePlayerInSteps(int steps, Player player) 
    {
        int stepsLeft = steps;
        GameObject tokenToMove = player.MyToken;
        int indexOnBoard = route.IndexOf(player.MyCurrentMonopolyNode);
        bool moveOverGoal = false;
        while(stepsLeft  > 0) 
        {
            indexOnBoard++;
            //is this over go?
            if(indexOnBoard > route.Count-1) 
            {
                indexOnBoard = 0;
                moveOverGoal = true;
            }
            //Get start and end position
            Vector3 startPos = tokenToMove.transform.position;
            Vector3 endPos = route[indexOnBoard].transform.position;
            //Perform the move
            while(MoveToNextNode(tokenToMove, endPos, 20))
            {
                yield return null;
            }

            stepsLeft--;
        }
        //Get go money
        if(moveOverGoal) 
        {
            //Collect money on the player
            player.CollectMoney(GameManager.instance.GetGoMoney);
        }
        //Set new node on the current player
        player.SetMyCurrentNode(route[indexOnBoard]);
    }


    bool MoveToNextNode(GameObject tokenToMove, Vector3 endPos, float speed)
    {
        return endPos != (tokenToMove.transform.position = Vector3.MoveTowards(tokenToMove.transform.position, endPos, speed * Time.deltaTime)) ;
    }




}
