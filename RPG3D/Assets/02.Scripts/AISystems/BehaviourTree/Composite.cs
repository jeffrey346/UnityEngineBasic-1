using System.Collections.Generic;

namespace RPG.AISystems.BehaviourTree
{
    public abstract class Composite : Node, IParentOFChildren
    {
        protected int currentIndex;

        protected Composite(BlackBoard blackBoard) : base(blackBoard) 
        { 
            children = new List<Node>();
        }

        public List<Node> children { get;  set; }
    }
}