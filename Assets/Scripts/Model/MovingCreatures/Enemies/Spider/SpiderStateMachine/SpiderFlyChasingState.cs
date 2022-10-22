using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using DefaultNamespace.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Spider.SpiderStateMachine
{
    public class SpiderFlyChasingState : IState
    {
        private SpiderStateManager _spider;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager)manager;

            _spider.SetSpeed(_spider.flyChasingStateSpeed);
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
