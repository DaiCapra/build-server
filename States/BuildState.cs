using System.Threading.Tasks;

namespace Pipeline.States
{
    public class BuildState : State
    {
        private readonly BuildService _buildService;

        public BuildState(Logger logger, BuildService buildService) : base(logger)
        {
            _buildService = buildService;
        }

        protected override async Task<bool> Validation()
        {
            return true;
        }

        public override async Task Execute()
        {
            await base.Execute();
            _buildService.ClearBuildFolder();
            bool b = await _buildService.Build();
            if (b)
            {
                Complete();
            }
            else
            {
                Fail();
            }
        }
    }
}