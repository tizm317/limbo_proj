using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    // 클라에서 요청이 왔을 때, 요청을 캡슐화 해서 클래스 포함시키기 위함.
    // 일종의 '주문 내역' 만들기 위함.

    public interface IJob
    {
        void Excute();
    }
    
    // 인자 개수별 함수 담는 Job
    // 요청 함수 -> Action
    // 인자 저장
    public class Job : IJob
    {
        Action _action;
        public Job(Action action)
        {
            _action = action;
        }
        public void Excute()
        {
            _action.Invoke();
        }
    }
    public class Job<T1> : IJob
    {
        Action<T1> _action;
        T1 _t1;
        public Job(Action<T1> action, T1 t1)
        {
            _action = action;
            _t1 = t1;
        }
        public void Excute()
        {
            _action.Invoke(_t1);
        }
    }
    public class Job<T1, T2> : IJob
    {
        Action<T1, T2> _action;
        T1 _t1;
        T2 _t2;
        public Job(Action<T1, T2> action, T1 t1, T2 t2)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
        }
        public void Excute()
        {
            _action.Invoke(_t1, _t2);
        }
    }
    public class Job<T1, T2, T3> : IJob
    {
        Action<T1, T2, T3> _action;
        T1 _t1;
        T2 _t2;
        T3 _t3;
        public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
        }
        public void Excute()
        {
            _action.Invoke(_t1, _t2, _t3);
        }
    }
}
