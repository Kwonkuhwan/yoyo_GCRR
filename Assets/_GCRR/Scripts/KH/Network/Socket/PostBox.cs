using System.Collections.Generic;

public class Postbox
{
    //데이타를 담을 큐
    private Queue<byte[]> messageQueue;

    public Postbox()
    {   //큐 초기화
        messageQueue = new Queue<byte[]>();
    }

    //큐에 데이타 삽입
    public void PushData(byte[] data)
    {
        messageQueue.Enqueue(data);
    }

    //큐에있는 데이타 꺼내서 반환
    public byte[] GetData()
    {
        //데이타가 1개라도 있을 경우 꺼내서 반환
        if (messageQueue.Count > 0)
            return messageQueue.Dequeue();
        else
            return null;    //없으면 빈값을 반환
    }
}