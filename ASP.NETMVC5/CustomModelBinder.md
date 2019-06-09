```csharp
public class DecimalModelBinder : IModelBinder
{
    public object BindModel(ControllerContext controllerContext,
        ModelBindingContext bindingContext)
    {
        ValueProviderResult valueResult = bindingContext.ValueProvider
            .GetValue(bindingContext.ModelName);
        ModelState modelState = new ModelState { Value = valueResult };
        object actualValue = null;
        try
        {
            actualValue = Convert.ToDecimal(valueResult.AttemptedValue,
                CultureInfo.CurrentCulture);
        }
        catch (FormatException e)
        {
            modelState.Errors.Add(e);
        }

        bindingContext.ModelState.Add(bindingContext.ModelName, modelState);
        return actualValue;
    }
}
```
# registration on Global.asax
```csharp
ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
```
# helping context
https://www.hanselman.com/blog/SplittingDateTimeUnitTestingASPNETMVCCustomModelBinders.aspx