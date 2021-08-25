using System.Threading;

namespace Particles
{
    public partial class ParticleManager
    {
        const int MAX_THREAD_COUNT = 128;
        int THREAD_COUNT;
        int THREAD_COUNT_MINUS_ONE;


        void GetThreadCount()
        {
            ThreadPool.GetMinThreads(out int count, out _);
            THREAD_COUNT = count;
            THREAD_COUNT_MINUS_ONE = count - 1;
        }


        int _ctai;
        public int CurrentThreadAddIndex
        {
            get
            {
                if (_ctai == THREAD_COUNT_MINUS_ONE)
                    _ctai = 0;
                else
                    _ctai++;

                return _ctai;
            }
        }
    }
}
