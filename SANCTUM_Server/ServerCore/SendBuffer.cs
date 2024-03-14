using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        // 나만 나의 스레드에서만 고유하게 사용할 수 있음 (쓰레드끼리의 경합을 없애기 위함)
        // 멀티쓰레드 문제 해켤
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChuckSize { get; set; } = 65535 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
            {
                CurrentBuffer.Value = new SendBuffer(ChuckSize);
            }

            if (CurrentBuffer.Value.FreeSize < reserveSize)
            {
                CurrentBuffer.Value = new SendBuffer(ChuckSize);
            }

            return CurrentBuffer.Value.Open(reserveSize);
        }

        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }

    public class SendBuffer
    {
        // [u][][][][][][][][][]
        // 여러 쓰레드에서 참조하는데 별 다른 멀티쓰레드 문제 조치없이
        // 문제가 안생기는 이유는 수정이 아니라 읽기만 하기 때문
        byte[] _buffer;
        int _usedSize = 0;  // RecvBuffer의 WritePos에 해당하는 것

        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chuckSize)    // chuckSize = 어마어마하게 큰 사이즈
        {
            _buffer = new byte[chuckSize];
        }

        public ArraySegment<byte> Open(int reserveSize)
        {
            if (reserveSize> FreeSize)
            {
                return null;
            }

            return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segment;
        }
    }
}
