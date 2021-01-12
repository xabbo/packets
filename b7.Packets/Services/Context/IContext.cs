using System;
using System.Threading.Tasks;

namespace b7.Packets.Services
{
    public interface IContext
    {
        bool IsSynchronized { get; }
        void Invoke(Action callback);
        T Invoke<T>(Func<T> callback);
        Task InvokeAsync(Action callback);
        Task<T> InvokeAsync<T>(Func<T> callback);
    }
}
