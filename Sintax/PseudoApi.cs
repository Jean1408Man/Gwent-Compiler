using System.Reflection;
namespace LogicalSide;

public static class Api
{
    public static object GetProperty<T>(T obj, string propertyName)
    {
        // Get the type of the object
        Type type = typeof(T);

        // Find the PropertyInfo by name
        PropertyInfo propertyInfo = type.GetProperty(propertyName);

        // Check if the property exists
        if (propertyInfo != null)
        {
            // Set the value of the property
            return propertyInfo.GetValue(obj);
        }
        else
        {
            throw new Exception("Problems with Api, property name not found");
        }
    }
    public static void SetProperty<T>(T obj, string propertyName, object value)
    {
        // Get the type of the object
        Type type = typeof(T);

        // Find the PropertyInfo by name
        PropertyInfo propertyInfo = type.GetProperty(propertyName);

        // Check if the property exists
        if (propertyInfo != null)
        {
            // Set the value of the property
            propertyInfo.SetValue(obj, value);
        }
        else
        {
            Console.WriteLine($"Property '{propertyName}' not found.");
        }
    }
    public static object InvokeMethodWithParameters<T>(T obj, string methodName, params object[] args)
    {
        // Get the type of the object
        Type type = obj.GetType();

        // Print the type of the object
        Console.WriteLine($"Invoking method '{methodName}' on object of type: {type.FullName}");

        // Find the MethodInfo by name
        MethodInfo methodInfo = type.GetMethod(methodName);

        // Check if the method exists
        if (methodInfo != null)
        {
            if (methodInfo.ReturnType == typeof(void))
            {//Is a void method
                methodInfo.Invoke(obj, args);
                return null;
            }
            else
            {
                // Invoke the method
                return methodInfo.Invoke(obj, args);
            }
        }
        else
        {
            throw new Exception($"Method '{methodName}' not found on type: {type.FullName}");
        }
    }
}
