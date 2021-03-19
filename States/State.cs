using System.Threading.Tasks;

namespace Pipeline.States
{
    public abstract class State
    {
        public State Next { get; set; }
        public bool HasNext => Next != null;
        public ProcessState CurrentState { get; set; }
        protected Logger Logger { get; }

        protected State(Logger logger)
        {
            CurrentState = ProcessState.Inactive;
            Logger = logger;
        }

        protected virtual async Task<bool> Validation()
        {
            return true;
        }

        protected async Task<bool> Validate()
        {
            var b = await Validation();
            if (!b)
            {
                Fail();
            }

            return b;
        }

        public virtual async Task Execute()
        {
            CurrentState = ProcessState.Executing;
        }

        protected virtual void Fail()
        {
            CurrentState = ProcessState.Failed;
        }

        protected virtual void Complete()
        {
            CurrentState = ProcessState.Complete;
        }

        public void Reset()
        {
            CurrentState = ProcessState.Inactive;
        }
    }
}