using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Differentiation
{
    public static class Algebra
    {
        public static Expression<Func<double, double>> Differentiate
            (Expression<Func<double, double>> function)
        {
            var parameter = function.Parameters[0];
            return Expression.Lambda<Func<double, double>>(GetDerivative(function.Body).Reduce()
                , parameter);
        }

        static Expression GetDerivative(Expression tree)
        {
            if (tree is ConstantExpression)
                return Expression.Constant(0.0);
            if (tree is ParameterExpression)
                return Expression.Constant(1.0);
            if (tree is BinaryExpression)
                return DifferentiateBinaryExpression((BinaryExpression)tree);
            return DifferentiateMethodCallExpression((MethodCallExpression)tree);
        }

        static Expression DifferentiateMethodCallExpression(MethodCallExpression callExpression)
        {
            var argument = callExpression.Arguments[0];
            var newMethod = "Sin";
            var multipyerForSinCosDerivative = Expression.Constant(-1.0);
            if (callExpression.Method.Name == "Sin")
            {
                newMethod = "Cos";
                multipyerForSinCosDerivative = Expression.Constant(1.0);
            }
            return Expression.Multiply(
                Expression.Call(typeof(Math).GetMethod(newMethod), argument),
                Expression.Multiply(
                    GetDerivative(argument),
                    multipyerForSinCosDerivative));
        }

        static Expression DifferentiateBinaryExpression(BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Add:
                    return Expression.Add(
                        GetDerivative(expression.Left),
                        GetDerivative(expression.Right));
                case ExpressionType.Subtract:
                    return Expression.Subtract(
                        GetDerivative(expression.Left),
                        GetDerivative(expression.Right));
                default:
                    return DifferentiateMultiplyExpression(
                        expression.Left,
                        expression.Right);
            }
        }

        static Expression DifferentiateMultiplyExpression(Expression f, Expression g)=>      
             Expression.Add(
                Expression.Multiply(
                    GetDerivative(f)
                    , g),
                Expression.Multiply(
                    f,
                    GetDerivative(g)));        
    }
}
