using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
public class AStar_Setting:MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject plane;
    private Vector3 plane_position;
    private float width, height;
    private GameObject grid;
    Board board = new Board();
    class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> heap = new List<T>();
        public void Push(T data)
        {
            //heap의 맨 끝에 새로운 데이터를 삽입한다.
            heap.Add(data);

            int now = heap.Count - 1;
            //도장깨기를 시작
            while(now > 0)
            {
                int next = (now -1)/2;
                if(heap[now].CompareTo(heap[next]) < 0)
                    break;//실패

                //두 값을 교체한다.
                T temp = heap[now];
                heap[now] = heap[next];
                heap[next] = temp;

                now = next;
            }
        }

        public T Pop()
        {
            //반환할 데이터를 따로 저장
            T ret = heap[0];
            //마지막 데이터를 루트로 이동한다
            int last_index = heap.Count -1;
            heap[0] = heap[last_index];
            heap.RemoveAt(last_index);
            //역으로 내려가는 도장깨기 시작
            int now = 0;
            while(true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                //왼쪽값이 현재값보다 크면, 왼쪽으로 이동
                if(left <= last_index && heap[next].CompareTo(heap[left]) < 0)
                    next = left;
                //오른쪽값이 현재값보다 크면, 오른쪽으로 이동
                if(right <= last_index && heap[next].CompareTo(heap[right]) < 0)
                    next = right;
                //왼쪽/오른쪽 모두 현재값보다 작으면 종료
                if(next == now)
                    break;

                //두 값을 교체한다.
                T temp = heap[now];
                heap[now] = heap[next];
                heap[next] = temp;
                //검사 위치를 이동한다.
                now = next;
            }

            return ret;
        }

        public int Count{ get{ return heap.Count;}}
    }
    List<Pos> points = new List<Pos>();
    class Pos
    {
        public int Y;
        public int X;
        public Pos(int y, int x){Y = y; X = x;}
    }
    struct PQNode : IComparable<PQNode>
    {
        public int F;
        public int G;
        public int Y;
        public int X;

        public int CompareTo(PQNode other)
        {
            if(F == other.F)
                return 0;
            return F < other.F ? 1 : -1;
        }
    }
    public void AStar(Vector3 start,Vector3 dest,GameObject player)
    {
        board.SetDestination(dest);
        int PosX = (int)start.x;
        int PosY = (int)start.z;
        //U L D R UL DL DR UR
        int[] deltaY = new int[]{-1,0,1,0,-1,1,1,-1};
        int[] deltaX = new int[]{0,-1,0,1-1,-1,1,1};
        int[] cost = new int[]{10,10,10,10,14,14,14,14};
        //점수 매기기
        //F = G + H
        //F = 최종 점수(작을 수록 좋음, 경로에 따라 달라짐)
        //G = 시작점에서 해당 좌표까지 이동하는데 드는 비용(작을 수록 좋음, 경로에 따라 달라짐) 
        //H = 목적지에서 얼마나 가까운지 (작을 수록 좋음, 고정)

        //(y, x) 이미 방문했는지 여부 (방문 = closed 상태)
        bool[,] closed = new bool[board.Size_Z, board.Size_X];//ClosedList
        //(y, x) 가는 길을 한 번이라도 발견했는지
        //발견X => MaxValue
        //발견O => F= G + H
        int[,] open = new int[board.Size_Z, board.Size_X];//OpenList
        for(int y = 0; y < board.Size_Z; y++)
            for(int x = 0; x < board.Size_X; x++)
                open[y, x] = Int32.MaxValue;

        Pos[,] parent = new Pos[board.Size_Z,board.Size_X];

        //오픈 리스트에 있는 정보들 중에서, 가장 좋은 후보를 빠르게 뽑아오기 위한 도구
        PriorityQueue<PQNode> pq = new PriorityQueue<PQNode>();
        //시작점 발견(예약 진행)
        open[PosY, PosX] = 10*(Math.Abs(board.DestY - PosY) + Math.Abs(board.DestX - PosX));
        pq.Push(new PQNode(){F = 10*(Math.Abs(board.DestY - PosY) + Math.Abs(board.DestX - PosX)), G = 0, Y = PosY, X = PosX});
        parent[PosY, PosX] = new Pos(PosY,PosX);
        while(pq.Count > 0)
        {
            //제일 좋은 후보를 찾는다.
            PQNode node = pq.Pop();
            //동일한 좌표를 여러 경로로 찾아서, 더 빠른 경로로 인해서 이미 방문(closed)된 경우 스킵
            if(closed[node.Y,node.X])
                continue;

            //방문한다
            closed[node.Y,node.X] = true;
            //목적지 도착했으면 바로 종료
            if(node.Y == board.DestY && node.X == board.DestX)
                break;

            //상하좌우 등 이동할 수 있는 좌표인지 확인해서 예약(open)을 한다.
            for(int i = 0; i <  deltaY.Length; i++)
            {
                int nextY = node.Y + deltaY[i];
                int nextX = node.X + deltaX[i];

                //유효 범위를 벗어나면 스킵
                if(nextX < 0 || nextX >= board.Size_X || nextY < 0 || nextY >= board.Size_Z)
                    continue;
                //벽으로 막혀서 갈 수 없으면 스킵
                if(board.Tile[nextY,nextX] == Board.TileType.Wall)
                    continue;
                //이미 방문한 곳이면 스킵
                if(closed[nextY, nextX])
                    continue;

                //비용 계산
                int g = node.G + cost[i];
                int h = 10*(Math.Abs(board.DestY - nextY) + Math.Abs(board.DestX - nextX));
                //다른 경로에서 더 빠른 길 이미 찾았으면 스킵
                if(open[nextY, nextX] < g + h)
                    continue;
                
                //예약 진행
                open[nextY, nextX] = g + h;
                pq.Push(new PQNode(){F = g + h, G = g, Y = nextY, X = nextX});
                parent[nextY, nextX] = new Pos(node.Y,node.X);
            }
        }
        CalcPathFromParent(parent);
        int k = 0;
        while(k < points.Count)
        {
            Vector3 pos = GetComponent<Transform>().position;
            pos = new Vector3(pos.x + points[k].X,pos.y,pos.z + points[k].Y);
            k++;
        }
    }
         

    void CalcPathFromParent(Pos[,] parent)
    {
        int y = board.DestY;
        int x = board.DestX;
        while(parent[y, x].Y != y || parent[y, x].X != x)
        {
            points.Add(new Pos(y, x));
            Pos pos = parent[y, x];
            y = pos.Y;
            x = pos.X;
        }
        points.Add(new Pos(y,x));
        points.Reverse();
    }

    void Start()
    {
        get_plane_info("Plane");
        int width = (int)plane.GetComponent<Transform>().localScale.x;
        int height =(int)plane.GetComponent<Transform>().localScale.z;
        create_grid(width,height);

        board.Initialize(10*width,10*height);//플레인은 10배임
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void get_plane_info(string a)
    {
        plane = GameObject.Find(a);
        plane_position = plane.GetComponent<Transform>().position;
    }

    private void create_grid(int width, int height)
    {
        grid  = Resources.Load<GameObject>("Prefabs/grid");
        GameObject parent = new GameObject("A*_check");
        for(int x = -(int)(5 * width); x < (int)(5 * width); x++)
            for(int y = -(int)(5 * height); y < (int)(5 * height); y++)
            {
                Vector3 pos = new Vector3(0.5f + plane_position.x + x,plane_position.y + 0.01f,0.5f + plane_position.z + y);
                GameObject temp = Instantiate(grid);
                temp.GetComponent<Transform>().position = pos;
                temp.transform.SetParent(parent.transform,true);
            }
    }
}
