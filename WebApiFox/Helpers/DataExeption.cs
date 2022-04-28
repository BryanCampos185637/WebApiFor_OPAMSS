using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace WebApiFox.Helpers
{
    public class DataException : Exception
    {
        public DataException(string msg, Exception ex)
            : base(message: msg, innerException: ex)
        {

        }
        public DataException()
        {
            this.EntityValidationError = new List<EntityValidationError>();
        }

        public List<EntityValidationError> EntityValidationError { get; set; }
        public void AddEntityValidationError(DbEntityValidationException ex)
        {
            foreach (var ob in ex.EntityValidationErrors)
            {
                var entidad = new EntityValidationError()
                {
                    EntityName = ob.Entry.Entity.GetType().Name,
                    State = ob.Entry.State.ToString()
                };
                foreach (var obDetal in ob.ValidationErrors)
                {
                    var fiel = new FielEntityValidationError()
                    {
                        FieldName = obDetal.PropertyName,
                        ExceptionMessage = obDetal.ErrorMessage
                    };
                    entidad.FieldList.Add(fiel);
                }
                this.EntityValidationError.Add(entidad);
            }
        }
    }
    public class EntityValidationError
    {
        public EntityValidationError()
        {
            this.FieldList = new List<FielEntityValidationError>();
        }

        public string EntityName { get; set; }
        public string State { get; set; }
        public List<FielEntityValidationError> FieldList { get; set; }
    }
    public class FielEntityValidationError
    {
        public string FieldName { get; set; }
        public string ExceptionMessage { get; set; }

    }
}