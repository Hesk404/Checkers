using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Checkers.ViewModels
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(Expression<Func<object>> expression)
        {
            if (PropertyChanged != null)
            {
                var lambda = expression as LambdaExpression;
                MemberExpression memberExpression;
                if (lambda.Body is UnaryExpression)
                {
                    var unaryExpression = lambda.Body as UnaryExpression;
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
                else
                {
                    memberExpression = lambda.Body as MemberExpression;
                }

                if (memberExpression != null)
                {
                    var propertyInfo = memberExpression.Member as PropertyInfo;
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyInfo.Name));
                }

            }
        }
    }
}
