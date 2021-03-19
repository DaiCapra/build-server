using System.Threading.Tasks;
using SimpleInjector;

namespace Pipeline.States
{
    public class StateMachine
    {
        private bool _isRunning;
        private State _current;

        private readonly State _start;
        private readonly State _reset;

        public StateMachine(Container container)
        {
            var validateConfigs = container.GetInstance<ValidateConfigsState>();
            var queryBranchUpdate = container.GetInstance<QueryBranchUpdateState>();
            var updateProject = container.GetInstance<UpdateProjectState>();
            var build = container.GetInstance<BuildState>();
            var zip = container.GetInstance<ZipState>();
            var updateManifest = container.GetInstance<UpdateManifestState>();
            
            _start = validateConfigs;
            _reset = queryBranchUpdate;
            
            validateConfigs.Next = queryBranchUpdate;
            queryBranchUpdate.Next = updateProject;
            updateProject.Next = build;
            build.Next = zip;
            zip.Next = updateManifest;
        }

        public async void Update()
        {
            while (_isRunning)
            {
                if (_current == null ||
                    _current.CurrentState == ProcessState.Failed)
                {
                    _current?.Reset();
                    _current = _start;
                }

                if (_current.CurrentState == ProcessState.Complete)
                {
                    _current.Reset();
                    _current = _current.HasNext ? _current.Next : _reset;
                }

                if (_current.CurrentState == ProcessState.Inactive)
                {
                    await _current.Execute();
                }

                await Task.Delay(1000);
            }
        }

        public void Start()
        {
            _isRunning = true;
            Update();
        }
    }
}