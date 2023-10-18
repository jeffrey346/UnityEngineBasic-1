using UnityEngine;


namespace RPG.AISystems.BehaviourTree
{
    public class Sequence : Composite
    {
        /// <summary>
        /// �ڽĵ��� ���ʴ�� ��ȸ�ϸ鼭 �ڽ��� Failure/Running ��ȯ�ϸ�
        /// �ش� ��� ��ȯ. ��� ������ Success ��ȯ
        /// </summary>
        
        public Sequence(BlackBoard blackBoard) :
            base(blackBoard)
        {

        }

        public override Result Invoke()
        {
            Result result = Result.Success;

            for (int i = 0; i < children.Count; i++)
            {
                if (i <= currentIndex)
                    continue;
                Debug.Log($"[Tree] : Invoking ... {children[i]}");

                result = children[i].Invoke();
                Debug.Log($"[Tree] : invoked ... {children[i]} , result : {result}");
                switch (result)
                {
                    case Result.Failure:
                        currentIndex = 0;
                        return Result.Failure;
                    case Result.Success:
                        currentIndex++;
                        break;
                    case Result.Running:
                        owner.stack.Push(children[i]);
                        return Result.Running;
                    default:
                        break;
                }
            }
            currentIndex = 0;
            return Result.Failure;
        }
    }

}