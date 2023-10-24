using System;

namespace RPG.AISystems.BehaviourTree
{
    public class Attack : Node
    {
        public Attack(BlackBoard blackBoard) : base(blackBoard) 
        {
        }

        public override Result Invoke()
        {
            CharacterController controller = blackBoard.controller;
            
            //���� Transition ��ǥ�� Attach �� �ƴѰ��
            if (controller.next != State.Attack)
            {
                // Attach ���¿��� �ٸ� ���·� Transition �� �Ϸ��� ���̹Ƿ�
                // Attach �� �Ϸ��ϰ� ����Ǿ����Ƿ� ������ȯ
                if(controller.IsInState(State.Attack)) 
                {
                    return Result.Success;
                   
                }
                // Attach �� ������ ���� �����Ƿ� Attach ���� ������ȯ
                else
                {
                    controller.ChangeState(State.Attack);
                }
            }

            
            blackBoard.agent.isStopped = true;
            return Result.Running;
            
        }
    }
}