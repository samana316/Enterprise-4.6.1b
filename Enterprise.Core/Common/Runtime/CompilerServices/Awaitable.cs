using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Enterprise.Core.Utilities;

namespace Enterprise.Core.Common.Runtime.CompilerServices
{
    public static class Awaitable
    {
        public static IAwaitable FromCompletedTask()
        {
            return Task.CompletedTask.FromTask();
        }

        public static IAwaitable FromTask(
            this Task task)
        {
            Check.NotNull(task, nameof(task));

            return new TaskAwaitable(task);
        }

        public static Task ToTask(
            this IAwaitable awaitable)
        {
            Check.NotNull(awaitable, nameof(awaitable));

            return awaitable.ToTaskImpl();
        }

        public static async Task ToTaskImpl(
            this IAwaitable awaitable)
        {
            await awaitable;
        }

        private sealed class TaskAwaitable : IAwaitable
        {
            private readonly Task task;

            public TaskAwaitable(
                Task task)
            {
                this.task = task;
            }

            public IAwaiter GetAwaiter()
            {
                return new Awaiter(this.task);
            }

            private sealed class Awaiter : IAwaiter
            {
                private readonly TaskAwaiter awaiter;

                public Awaiter(
                    Task task)
                {
                    this.awaiter = task.GetAwaiter();
                }

                public bool IsCompleted
                {
                    get
                    {
                        return this.awaiter.IsCompleted;
                    }
                }

                public void GetResult()
                {
                    this.awaiter.GetResult();
                }

                public void OnCompleted(
                    Action continuation)
                {
                    this.awaiter.OnCompleted(continuation);
                }

                public void UnsafeOnCompleted(
                    Action continuation)
                {
                    this.awaiter.UnsafeOnCompleted(continuation);
                }
            }
        }
    }
}
