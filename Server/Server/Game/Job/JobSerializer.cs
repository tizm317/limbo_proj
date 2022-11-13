using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    // Job 모아서 실행하는 클래스
    // Job 나열해서 실행

	// 식당 비유
	// 주문서를 받아서 주방까지는 감
	// 주방에 주방장이 없으면,  주방장이 돼서 직접 요리까지 함
	// 주방에 주방장이 있으면, 주문서만 주고 빠져 나옴

    public class JobSerializer
    {
		JobTimer _timer = new JobTimer();			// 예약 작업
		Queue<IJob> _jobQueue = new Queue<IJob>();	// 당장 실행 작업
		object _lock = new object(); // 큐만 보호
		bool _flush = false; // 실행중이냐

		#region PushAfter Helper 함수들
		// 1. Action을 Push하면 Job으로 바꿔서 Push해줌(일일히 job을 외부에서 안 만들어도 됨)
		public void PushAfter(int tickAfter, Action action) { PushAfter(tickAfter, new Job(action)); }
		public void PushAfter<T1>(int tickAfter, Action<T1> action, T1 t1) { PushAfter(tickAfter, new Job<T1>(action, t1)); }
		public void PushAfter<T1, T2>(int tickAfter, Action<T1, T2> action, T1 t1, T2 t2) { PushAfter(tickAfter, new Job<T1, T2>(action, t1, t2)); }
		public void PushAfter<T1, T2, T3>(int tickAfter, Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { PushAfter(tickAfter, new Job<T1, T2, T3>(action, t1, t2, t3)); }
		#endregion
		// 예약 작업
		public void PushAfter(int tickAfter, IJob job)
        {
			_timer.Push(job, tickAfter);
        }


		# region Push Helper 함수들
        // 1. Action을 Push하면 Job으로 바꿔서 Push해줌(일일히 job을 외부에서 안 만들어도 됨)
        public void Push(Action action) { Push(new Job(action)); }
		public void Push<T1>(Action<T1> action, T1 t1) { Push(new Job<T1>(action, t1)); }
		public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2) { Push(new Job<T1, T2>(action, t1, t2)); }
		public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3) { Push(new Job<T1, T2, T3>(action, t1, t2, t3)); }
		#endregion
		// 당장 실행 작업
        public void Push(IJob job)
		{
			lock (_lock)
			{
				_jobQueue.Enqueue(job);
			}
		}
		
		
		// 실행 : 모든 일감 처리
		public void Flush()
		{
			while (true)
			{
				_timer.Flush();

				IJob job = Pop();
				if (job == null)
					return;

				job.Excute();
			}
		}
		IJob Pop()
		{
			lock (_lock)
			{
				if (_jobQueue.Count == 0) // 더 이상 일감 없음
				{
					_flush = false; // false로 바꾸고 빠져나옴
					return null;
				}
				return _jobQueue.Dequeue();
			}
		}
	}
}
