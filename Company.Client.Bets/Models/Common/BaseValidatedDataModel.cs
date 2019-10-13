using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using FluentValidation;

namespace Company.Client.Bets.Models
{
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    abstract class BaseValidatedDataModel<TModel> : BaseValidatedModel
        where TModel : BaseValidatedDataModel<TModel>
    {
        static BaseValidatedDataModel()
        {
            var modelType = typeof(TModel);
            var instanseType = typeof(BaseValidatedDataModel<TModel>);

            if (!modelType.IsSubclassOf(typeof(BaseValidatedDataModel<TModel>)))
            {
                throw new Exception($"{modelType.FullName} must be inherited {instanseType.FullName}");
            }
        }

        /// <summary>
        /// Валидатор
        /// </summary>
        IValidator Validator;

        public BaseValidatedDataModel()
        {
            var inpc = this as INotifyPropertyChanged;
            if (inpc != null)
            {
                inpc.PropertyChanged += (sender, args) => InvalidateErrors();
            }

            InvalidateErrors();
        }

        /// <summary>
        /// Установить валидатор данных
        /// </summary>
        public void SetValidator(IValidator validator)
        {
            if (Validator != validator)
            {
                Validator = validator;

                if (validator == null)
                {
                    ClearErrors();
                }
                else
                {
                    InvalidateErrors();
                }
            }
        }

        protected void InvalidateErrors()
        {
            if (Validator != null)
            {
                var result = Validator.Validate(this);
                var notCorrectProperties = result.Errors.Select(x => x.PropertyName).Distinct().ToArray();
                var сorrectProperties = GetNotValidProperties().Except(notCorrectProperties);

                ClearErrors(сorrectProperties);

                foreach (var error in result.Errors)
                {
                    ReplaceErrors(error.PropertyName, error.ErrorMessage);
                }
            }
        }

        private void ClearErrors(IEnumerable<string> properties)
        {
            foreach (var property in properties)
            {
                RemoveErrors(property);
            }
        }
    }
}
