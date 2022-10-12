using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReadyPlayerMe
{
    public class OperationExecutor<T> where T : Context
    {
        public event Action<float, string> ProgressChanged;
        public bool IsCancelled => ctxSource.IsCancellationRequested;
        public int Timeout;

        private CancellationTokenSource ctxSource;

        private readonly IOperation<T>[] operations;
        private readonly int operationsCount;

        private float currentProgress;
        private int currentIndex;

        public OperationExecutor(IOperation<T>[] operations)
        {
            this.operations = operations;
            operationsCount = operations.Length;
        }

        public async Task<T> Execute(T context)
        {
            ctxSource = new CancellationTokenSource();
            foreach (var operation in operations)
            {
                operation.ProgressChanged += OnProgressChanged;
                operation.Timeout = Timeout;
                try
                {
                    context = await operation.Execute(context, ctxSource.Token);
                    currentIndex++;
                }
                catch (CustomException exception)
                {
                    if (ctxSource.IsCancellationRequested)
                    {
                        ctxSource.Dispose();
                        throw new CustomException(FailureType.OperationCancelled, exception.Message);
                    }

                    throw new CustomException(exception.FailureType, exception.Message);
                }
                operation.ProgressChanged -= OnProgressChanged;
            }

            return context;
        }

        public void Cancel()
        {
            if (!ctxSource.IsCancellationRequested)
            {
                ctxSource?.Cancel();
            }
        }

        private void OnProgressChanged(float progress)
        {
            currentProgress = 1f / operationsCount * (currentIndex + progress);
            ProgressChanged?.Invoke(currentProgress, operations[currentIndex].GetType().Name);
        }
    }
}
