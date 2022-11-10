using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketMessage
{
	public ushort Id { get; set; }
	public IMessage Message { get; set; }
}

// 유니티는 별도 쓰레드에서 컨텐츠 코드 바로 건드리면 안됨 -> 패킷큐에 넣고, 유니티 메인쓰레드에서 실행
public class PacketQueue
{
	public static PacketQueue Instance { get; } = new PacketQueue();

	Queue<PacketMessage> _packetQueue = new Queue<PacketMessage>();
	object _lock = new object();

	public void Push(ushort id, IMessage packet)
	{
		lock (_lock)
		{
			_packetQueue.Enqueue(new PacketMessage() { Id = id, Message = packet });
		}
	}

	public PacketMessage Pop()
	{
		lock (_lock)
		{
			if (_packetQueue.Count == 0)
				return null;

			return _packetQueue.Dequeue();
		}
	}

	public List<PacketMessage> PopAll()
	{
		List<PacketMessage> list = new List<PacketMessage>();

		lock (_lock)
		{
			while (_packetQueue.Count > 0)
				list.Add(_packetQueue.Dequeue());
		}

		return list;
	}
}