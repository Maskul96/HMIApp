
namespace HMIApp.DataBasesAndDBContext_s
{
    public class BaseClassForDatabase
    {
        public BaseClassForDatabase()
        {

        }
        //Zmienna protected - widoczna w tej klasie i w klasach dziedziczących
        protected string ConnectionString { get; set; }


    }
}
