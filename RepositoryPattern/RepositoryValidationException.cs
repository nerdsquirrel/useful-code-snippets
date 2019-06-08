using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Text;

namespace DataModel.Exceptions
{
    [Serializable]
    public class RepositoryValidationException : Exception
    {
        public RepositoryValidationException(DbEntityValidationException dbEntityValidationException) : base(GetDbEntityValidationExceptionText(dbEntityValidationException), dbEntityValidationException.InnerException)
        {
        }

        public RepositoryValidationException(DbUpdateException dbUpdateException) : base(GetDbUpdateExceptionText(dbUpdateException), dbUpdateException.InnerException)
        {
        }
        private static string GetDbEntityValidationExceptionText(DbEntityValidationException exc)
        {
            var msg = string.Empty;
            foreach (var validationErrors in exc.EntityValidationErrors)
                foreach (var error in validationErrors.ValidationErrors)
                    msg += $"Property: {error.PropertyName} Error: {error.ErrorMessage}" + Environment.NewLine;
            return msg;
        }

        private static string GetDbUpdateExceptionText(DbUpdateException dbu)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");

            try
            {
                foreach (var result in dbu.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name);
                }
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e);
            }

            return builder.ToString();
        }
    }
}