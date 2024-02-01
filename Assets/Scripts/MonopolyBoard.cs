using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// ����� MonopolyBoard, ������� ������������ ����� ������� ���� � ���� ���������
public class MonopolyBoard : MonoBehaviour
{
    public static MonopolyBoard Instance;
    // ������ ����� �� ������� ����
    public  List<MonopolyNode> route = new List<MonopolyNode>();

    [System.Serializable]
    public class NodeSet
    {
        public Color setColor = Color.white;
        public List<MonopolyNode> nodesInSetList = new List<MonopolyNode>();   
    }

    public List<NodeSet> nodeSetList = new List<NodeSet>();

    private void Awake()
    {
        Instance = this;
    }

    // �����, ������� ���������� ��� ��������� �������� � ���������� Unity
    private void OnValidate()
    {
        // ������� ������ �����
        route.Clear();

        // ���������� ���� ����� � ������
        foreach (Transform node in transform.GetComponentInChildren<Transform>())
        {
            route.Add(node.GetComponent<MonopolyNode>());
        }
    }

    // �����, ������� ������ ����� ����� ������ �� ������� ����
    private void OnDrawGizmos()
    {
        // ��������, ��� � ������ ���� ����� ������ ����
        if (route.Count > 1) 
        {
            // ���� �� ���� ����� � ������
            for (int i = 0; i < route.Count; i++) 
            {
                // ��������� ������� �������� ����
                Vector3 current = route[i].transform.position;
                // ��������� ������� ���������� ����, ���� �� ����������, ����� - ������� �������� ����
                Vector3 next = (i + 1 < route.Count) ? route[i + 1].transform.position : current;

                // ��������� ����� �����
                Gizmos.color = Color.green;
                // ��������� ����� �� �������� ���� � ����������
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
        bool isMovingForward = steps > 0;
        if(isMovingForward)
        {
            while (stepsLeft > 0)
            {
                indexOnBoard++;
                //is this over go?
                if (indexOnBoard > route.Count - 1)
                {
                    indexOnBoard = 0;
                    moveOverGoal = true;
                }
                //Get start and end position
                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                //Perform the move
                while (MoveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft--;
            }
        }
        else
        {
            while (stepsLeft < 0)
            {
                indexOnBoard--;
                //is this over go?
                if (indexOnBoard < 0)
                {
                    indexOnBoard = route.Count - 1;
                }
                //Get start and end position
                //Vector3 startPos = tokenToMove.transform.position;
                Vector3 endPos = route[indexOnBoard].transform.position;
                //Perform the move
                while (MoveToNextNode(tokenToMove, endPos, 20))
                {
                    yield return null;
                }
                stepsLeft++;
            }
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


    public (List<MonopolyNode> list, bool allSame) PlayerHasAllNodesOfSet(MonopolyNode node)
    {
            bool allSame = false;

        foreach (var nodeSet in nodeSetList)
        {
            if(nodeSet.nodesInSetList.Contains(node)) 
            {
                    
                //Linq
                allSame = nodeSet.nodesInSetList.All(_node => _node.Owner == node.Owner);
                return (nodeSet.nodesInSetList,  allSame);
                
            }
        }
        return (null, allSame);

    }

}
