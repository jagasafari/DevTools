namespace alg.Logic{
    using System;
    using alg.Contracts;

    public class UncommentAllCs : NextStep,INextStep 
    {
        public bool Completed() => false;

        public bool ExpectedComilationSucces() => true;

        public INextStep Uncomment()
        {
            
            throw new NotImplementedException();
        }
    }
}