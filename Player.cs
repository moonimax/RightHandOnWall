using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
 public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        X = _x;
        Y = _y;
        
    }  
    public bool isWall;
    public int X;
    public int Y;
    public int num;
 }



public class Player : MonoBehaviour
{
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public bool allowDiagnol, dontCrossCorner;
    public List<Node> FinalNodeList;
    public int PosX { get; private set; }
    public int PosY { get; private set; }
    public Transform startTR;
    public Transform target;
    int sizeX, sizeY;

    Node[,] NodeArray;
    Node StartNode, TargetNode, CurNode;
    List<Node> _points = new List<Node>();  // �� Ž���� ���
    List<Node> OpenList, ClosedList;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public void Initialize()
    {
        bool isWall = false;
        startPos = Vector2Int.RoundToInt(startTR.position);
        targetPos = Vector2Int.RoundToInt(target.position);
       

        // �ٶ󺸴� ���� �������� ��ǥ ��ȭ�� ��Ÿ��.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // ��ȸ�� ���� ��ġ��
        int[] leftY = { 0, -1, 0, 1 }; // ��ȸ�� ���Ŀ� ���� �ٶ󺸴� ������ �ݽð�������� 90�� ȸ��

        int[] rightX = { 1, 0, -1, 0};  // ��ȸ�� ���� ��ġ��
        int[] rightY = { 0, 1, 0, -1};
        
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];
       
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                NodeArray[i, j] = new Node(false, i , j);
                // collider ����, ������(0.4f).
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f)) //2���� �迭 Ÿ��(return)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                    NodeArray[i, j] = new Node(isWall, bottomLeft.x + i, bottomLeft.y + j);
                    Debug.Log(NodeArray[i, j]);
                }
            }
        }

        // ����, ��ǥ ��� NodeArray[] ����.
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node> { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        CurNode = OpenList[0];
        int _dir = (int)Dir.Up; //���� ���� ���� up
        int cnt = 0;

        Debug.Log(TargetNode.X + " , " + TargetNode.Y);
        while (CurNode.X != TargetNode.X || CurNode.Y != TargetNode.Y)   // �����濡�� ǥ�� ���� or ���� ����� �� ��쿡 ������ �߰�.
        {
            Debug.Log("���� ���ŵ� CurNode��" + CurNode.X + " ," + CurNode.Y);
            if (NodeArray[CurNode.X + leftX[_dir], CurNode.Y + leftY[_dir]].isWall == true)
            {
                Debug.Log("���� �ֽ��ϴ�.");
            }
           
            // 1. ���� �ٶ󺸴� �������� �������� �������� �� �� �ִ��� Ȯ��.
            if (NodeArray[(CurNode.X + leftX[_dir]), (CurNode.Y + leftY[_dir])].isWall == false)
            {
                Debug.Log("�� ��ǥ");
                // ���� �������� 90�� ȸ�� 
                _dir = (_dir + 1) % 4;  
                // ������ ����
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                //FinalNodeList.Add(CurNode);
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                Debug.Log("�·� �̵��� ���� ���� : " + CurNode.X + ", " + CurNode.Y);
            }

            // 2. ���� �ٶ󺸴� ������ �������� ������ �� �ִ��� Ȯ��.
            if (NodeArray[(CurNode.X + frontX[_dir]), (CurNode.Y + frontY[_dir])].isWall == false)
            {
                Debug.Log("���� ��ǥ");
                // ������ ����
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                //FinalNodeList.Add(CurNode);
                
                Debug.Log("�������� �̵��� ���� ���� : " + CurNode.X + ", " + CurNode.Y);
            }
            // 3. ���� �ٶ󺸴� ������ �������� ���������� �� �� �ִ� �� Ȯ��.
            else if (NodeArray[(CurNode.X + rightX[_dir]), (CurNode.Y + rightY[_dir])].isWall == false)
            {
                 Debug.Log("�� ��ǥ");
                // ���� �����̶�� 90�� ȸ��
                _dir = (_dir - 1 + 4) % 4;
                // ������ ����
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                //FinalNodeList.Add(CurNode);
                
                Debug.Log("��� �̵��� ���� ���� : " + CurNode.X + ", " + CurNode.Y);
            }
            else
            {
                Debug.Log("Ž���� �����մϴ�.");
                return;
            }

            if((CurNode.X == TargetNode.X && CurNode.Y == TargetNode.Y))
            {
                FinalNodeList.Add(CurNode);
                FinalNodeList.Reverse();
                //for (int i = 0; i < FinalNodeList.Count; i++) print(i + "��°�� " + FinalNodeList[i].X + ", " + FinalNodeList[i].Y);
            }
        }
    }
    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
       
    }

    int _lastIndex = 0;
    private void OnMove()
    {
    }

    void OnDrawGizmos()
    {
        if(FinalNodeList.Count != 0)
        {
            for(int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].X, FinalNodeList[i].Y), new Vector2(FinalNodeList[i + 1].X, FinalNodeList[i + 1].Y));
               // Debug.Log(FinalNodeList[i].X + " , " + FinalNodeList[i].Y);
            }
        }
    }
}
    
