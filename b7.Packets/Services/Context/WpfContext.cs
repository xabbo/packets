using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace b7.Packets.Services
{
    public class WpfContext : IContext
    {
        private readonly Dispatcher _dispatcher;

        public WpfContext(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public bool IsSynchronized => _dispatcher.CheckAccess();

        public void Invoke(Action callback) => _dispatcher.Invoke(callback);

        public T Invoke<T>(Func<T> callback) => _dispatcher.Invoke(callback);

        public Task InvokeAsync(Action callback) => _dispatcher.InvokeAsync(callback).Task;

        public Task<T> InvokeAsync<T>(Func<T> callback) => _dispatcher.InvokeAsync(callback).Task;
    }
}
