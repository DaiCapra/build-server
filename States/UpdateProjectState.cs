using System;
using System.Threading.Tasks;

namespace Pipeline.States
{
    public class UpdateProjectState : State
    {
        private readonly BuildService _buildService;

        public UpdateProjectState(Logger logger, BuildService buildService) : base(logger)
        {
            _buildService = buildService;
        }

        public override async Task Execute()
        {
            await base.Execute();
            bool b =_buildService.UpdateLocalProject();
            if (b)
            {
                Logger.Log("Updated repository!");
                Complete();
            }
            else
            {
                Fail();
            }
        }

        protected override async Task<bool> Validation()
        {
            return true;
        }
    }
}