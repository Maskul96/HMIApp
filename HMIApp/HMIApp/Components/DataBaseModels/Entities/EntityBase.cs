using HMIApp.Components.DataBaseModels.Entities;

namespace HMIApp.Components.DataBase.Entities
{
    //Klasa abstrakcyjna jako baza do dziedziczenia
    public abstract class EntityBase :IEntity
    {
        public int Id { get; set; }
    }
}
