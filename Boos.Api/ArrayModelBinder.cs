using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Books.Api
{
    public class ArrayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            //This model only works for enumerable type
            if (!bindingContext.ModelMetadata.IsEnumerableType)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return Task.CompletedTask;
            }

            //Get the inputted value through the value provider
            var value = bindingContext.ValueProvider
                //Returns a string with ids separated with ','
                .GetValue(bindingContext.ModelName).ToString();

            //If the value is white or empty we return null
            if (string.IsNullOrWhiteSpace(value))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            /**
             * When the value isn't null or whitespace
             * and the type of the model is enumerable
             * Get the enumerable's type, and a convertor 
             */
            var elementType = bindingContext.ModelType.GetTypeInfo().GetGenericArguments()[0];
            var converter = TypeDescriptor.GetConverter(elementType);

            //Convert each item in the list to the enumerable type
            var values = value.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => converter.ConvertFromString(x.Trim()))
                .ToArray();

            //Create an array of that type, and set it as the Model value
            var typedValues = Array.CreateInstance(elementType, values.Length);
            values.CopyTo(typedValues, 0);
            //Set the model type to the array
            bindingContext.Model = typedValues;

            //return a successful result, passing in the model
            bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
            return Task.CompletedTask;
        }
    }
}
