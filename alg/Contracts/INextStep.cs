namespace alg.Contracts{
   public interface INextStep : ICompilingStep{
       bool Completed();
       bool ExpectedComilationSucces();
       INextStep Uncomment();
   }
}
