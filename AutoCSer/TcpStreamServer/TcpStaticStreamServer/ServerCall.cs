﻿using System;
using System.Runtime.CompilerServices;

namespace AutoCSer.Net.TcpStaticStreamServer
{
    /// <summary>
    /// TCP 服务器端同步调用
    /// </summary>
    /// <typeparam name="callType">调用类型</typeparam>
    public abstract class ServerCall<callType> : TcpInternalStreamServer.ServerCall
        where callType : ServerCall<callType>
    {
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender">套接字</param>
        [MethodImpl(AutoCSer.MethodImpl.AggressiveInlining)]
        public void Set(TcpInternalStreamServer.ServerSocketSender sender)
        {
            this.Sender = sender;
            switch (sender.ServerTaskType)
            {
                case TcpStreamServer.ServerTaskType.TcpQueue: TcpServer.ServerCallQueue.Default.Add(this); return;
                case TcpStreamServer.ServerTaskType.Queue: sender.Server.CallQueue.Add(this); return;
            }
        }
        /// <summary>
        /// 获取服务器端调用
        /// </summary>
        /// <returns>服务器端调用</returns>
        [MethodImpl(AutoCSer.MethodImpl.AggressiveInlining)]
        public static callType Pop()
        {
            return AutoCSer.Threading.RingPool<callType>.Default.Pop();
        }
        /// <summary>
        /// 服务器端调用入池
        /// </summary>
        /// <param name="call">服务器端调用</param>
        [MethodImpl(AutoCSer.MethodImpl.AggressiveInlining)]
        protected void push(callType call)
        {
            Sender = null;
            AutoCSer.Threading.RingPool<callType>.Default.PushNotNull(call);
        }
    }
    /// <summary>
    /// TCP 服务器端同步调用
    /// </summary>
    /// <typeparam name="callType">调用类型</typeparam>
    /// <typeparam name="inputParameterType">输入参数类型</typeparam>
    public abstract class ServerCall<callType, inputParameterType> : ServerCall<callType>
        where callType : ServerCall<callType, inputParameterType>
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        protected inputParameterType inputParameter;
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="sender">套接字</param>
        /// <param name="inputParameter">输入参数</param>
        [MethodImpl(AutoCSer.MethodImpl.AggressiveInlining)]
        public void Set(TcpInternalStreamServer.ServerSocketSender sender, ref inputParameterType inputParameter)
        {
            this.Sender = sender;
            this.inputParameter = inputParameter;
            switch (sender.ServerTaskType)
            {
                case TcpStreamServer.ServerTaskType.TcpQueue: TcpServer.ServerCallQueue.Default.Add(this); return;
                case TcpStreamServer.ServerTaskType.Queue: sender.Server.CallQueue.Add(this); return;
            }
        }
        /// <summary>
        /// 服务器端调用入池
        /// </summary>
        /// <param name="call">服务器端调用</param>
        [MethodImpl(AutoCSer.MethodImpl.AggressiveInlining)]
        protected new void push(callType call)
        {
            Sender = null;
            inputParameter = default(inputParameterType);
            AutoCSer.Threading.RingPool<callType>.Default.PushNotNull(call);
        }
    }
}
