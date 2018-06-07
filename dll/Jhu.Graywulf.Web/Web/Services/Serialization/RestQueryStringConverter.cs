using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Taken from https://stackoverflow.com/questions/1393359/can-a-wcf-service-contract-have-a-nullable-input-parameter
    /// </remarks>
    public class RestQueryStringConverter : QueryStringConverter
    {
        public override bool CanConvert(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
                return CanConvert(type);
            }
            else if (type.IsEnum)
            {
                return true;
            }
            else
            {
                return base.CanConvert(type);
            }
        }

        public override object ConvertStringToValue(string parameter, Type parameterType)
        {
            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (String.IsNullOrEmpty(parameter))
                {
                    return null;
                }
                else
                {
                    var type = parameterType.GetGenericArguments()[0];
                    return ConvertStringToValue(parameter, type);
                }
            }
            else if (parameterType.IsEnum)
            {
                return Enum.Parse(parameterType, parameter, true);
            }
            else
            {
                return base.ConvertStringToValue(parameter, parameterType);
            }
        }

        public override string ConvertValueToString(object parameter, Type parameterType)
        {
            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (parameter == null)
                {
                    return null;
                }
                else
                {
                    var type = parameterType.GetGenericArguments()[0];
                    return ConvertValueToString(parameter, type);
                }
            }
            else if (parameterType.IsEnum)
            {
                return Enum.Format(parameterType, parameter, "f").ToLower();
            }
            else
            {
                return base.ConvertValueToString(parameter, parameterType);
            }
        }
    }
}
