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
    List<Node> _points = new List<Node>();  // 길 탐색한 노드
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
       

        // 바라보는 방향 기준으로 좌표 변화를 나타냄.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
        int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

        int[] rightX = { 1, 0, -1, 0};  // 우회전 변경 위치점
        int[] rightY = { 0, 1, 0, -1};
        
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];
       
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                NodeArray[i, j] = new Node(false, i , j);
                // collider 판정, 반지름(0.4f).
                foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f)) //2차원 배열 타입(return)
                {
                    if (col.gameObject.layer == LayerMask.NameToLayer("Wall")) isWall = true;

                    NodeArray[i, j] = new Node(isWall, bottomLeft.x + i, bottomLeft.y + j);
                    Debug.Log(NodeArray[i, j]);
                }
            }
        }

        // 시작, 목표 노드 NodeArray[] 저장.
        StartNode = NodeArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = NodeArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<Node> { StartNode };
        ClosedList = new List<Node>();
        FinalNodeList = new List<Node>();

        CurNode = OpenList[0];
        int _dir = (int)Dir.Up; //좌측 기준 위로 up
        int cnt = 0;

        Debug.Log(TargetNode.X + " , " + TargetNode.Y);
        while (CurNode.X != TargetNode.X || CurNode.Y != TargetNode.Y)   // 갈림길에서 표식 종류 or 길이 끊기게 될 경우에 선택지 추가.
        {
            Debug.Log("새로 갱신된 CurNode는" + CurNode.X + " ," + CurNode.Y);
            if (NodeArray[CurNode.X + leftX[_dir], CurNode.Y + leftY[_dir]].isWall == true)
            {
                Debug.Log("벽이 있습니다.");
            }
           
            // 1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
            if (NodeArray[(CurNode.X + leftX[_dir]), (CurNode.Y + leftY[_dir])].isWall == false)
            {
                Debug.Log("좌 좌표");
                // 왼쪽 방향으로 90도 회전 
                _dir = (_dir + 1) % 4;  
                // 앞으로 전진
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                //FinalNodeList.Add(CurNode);
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                Debug.Log("좌로 이동한 현재 값은 : " + CurNode.X + ", " + CurNode.Y);
            }

            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (NodeArray[(CurNode.X + frontX[_dir]), (CurNode.Y + frontY[_dir])].isWall == false)
            {
                Debug.Log("직진 좌표");
                // 앞으로 전진
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                //FinalNodeList.Add(CurNode);
                
                Debug.Log("직진으로 이동한 현재 값은 : " + CurNode.X + ", " + CurNode.Y);
            }
            // 3. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는 지 확인.
            else if (NodeArray[(CurNode.X + rightX[_dir]), (CurNode.Y + rightY[_dir])].isWall == false)
            {
                 Debug.Log("우 좌표");
                // 우측 방향이라면 90도 회전
                _dir = (_dir - 1 + 4) % 4;
                // 앞으로 전진
                CurNode.X += frontX[_dir];
                CurNode.Y += frontY[_dir];
                FinalNodeList.Add(new Node(CurNode.isWall, CurNode.X, CurNode.Y));
                //FinalNodeList.Add(CurNode);
                
                Debug.Log("우로 이동한 현재 값은 : " + CurNode.X + ", " + CurNode.Y);
            }
            else
            {
                Debug.Log("탐색을 종료합니다.");
                return;
            }

            if((CurNode.X == TargetNode.X && CurNode.Y == TargetNode.Y))
            {
                FinalNodeList.Add(CurNode);
                FinalNodeList.Reverse();
                //for (int i = 0; i < FinalNodeList.Count; i++) print(i + "번째는 " + FinalNodeList[i].X + ", " + FinalNodeList[i].Y);
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
    
